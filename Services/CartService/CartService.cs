using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using WebApi.Dtos.Cart;
using WebApi.Models;

namespace WebApi.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public CartService(DataContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;

        }
        public async Task<ServiceResponse<string>> Add(int itemId, int itemAmount, int[] Variants, HttpRequest request)
        {
            var response = new ServiceResponse<string>();
            var token = request.Headers["x-access-token"].ToString();
            JwtSecurityToken? jwttoken;
            if (token is not null)
            {
                jwttoken = TokenService.TokenService.ValidateToken(token, _configuration);
            }
            else
            {
                response.Success = false;
                return response;
            }
            if (jwttoken is null)
            {
                response.Success = false;
                response.StatusCode = 401;
                return response;
            }
            var userIdString = jwttoken.Claims.First(x => x.Type == "nameid").Value;
            int userId = int.Parse(userIdString);
            var user = _context.Users.ToList().Find(u => u.Id == userId);
            if (user is null) {
                response.Success = false;
                return response;
            }

            var item = _context.Items.ToList().Find(u => u.Id == itemId);

            if(item is null)
            {
                response.Success = false;
                return response;
            }
            Cart cart;
            bool cartExists;
            if (user.CartId is null)
            {
                cartExists = false;
                cart = new Cart();
            }
            else
            {
                cart = _context.Carts.ToList().Find(c => c.Id == user.CartId)!;
                if(cart is null){
                    cartExists = false;
                    cart = new Cart();
                } else
                {
                    cartExists = true;
                    if (_context.CartItems.ToList().Find(c => c.CartId == cart.Id) is not null)
                    {
                        response.Success = false;
                        response.Message = "Этот товар уже есть в корзине";
                        return response;
                    }
                } 
            }
            bool VariantExists;
            int TotalVariantsPrice = 0;
            List<Variant> FoundVariants = new();
            if(Variants is not null)
            {
                foreach (var variant in Variants)
                {
                    if (variant > 0)
                    {
                        FoundVariants?.Add(_context.Variants.ToList().Find(v => v.Id == variant && v.ItemId == item.Id)!);
                        if (!FoundVariants!.Any())
                        {
                            response.Success = false;
                            response.Message = "Этого варианта нет";
                            return response;
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Этого варианта нет";
                        return response;
                    }
                }
                foreach (var variant in FoundVariants)
                {
                    TotalVariantsPrice += (int)variant.Price!;
                }
                VariantExists = true;
            }
            else
            {
                VariantExists = false;
            }
            
            
           
            



            if (cartExists)
            {
                if(!VariantExists)
                {
                    if (cart.TotalPrice is null) cart.TotalPrice = item.Price * itemAmount;
                    else cart.TotalPrice += item.Price * itemAmount;
                }else
                {
                    if (cart.TotalPrice is null) cart.TotalPrice = TotalVariantsPrice * itemAmount;
                    else cart.TotalPrice += TotalVariantsPrice * itemAmount;
                }
                

                _context.Update(cart);
            }
            else
            {
                _context.Carts.Add(cart);
                _context.SaveChanges();
                if (!VariantExists)
                {
                    if (cart.TotalPrice is null) cart.TotalPrice = item.Price * itemAmount;
                    else cart.TotalPrice += item.Price * itemAmount;
                }
                else
                {
                    if (cart.TotalPrice is null) cart.TotalPrice = TotalVariantsPrice * itemAmount;
                    else cart.TotalPrice += TotalVariantsPrice * itemAmount;
                }
                _context.Update(cart);
                user.CartId = cart.Id;
                _context.Update(user);
            }

            CartItem cartItem = new CartItem()
            {
                Amount = itemAmount,
                CartId = cart.Id,
                ItemId = item.Id,
                Variants = Variants,
            };

            _context.CartItems.Add(cartItem);
            
            _context.SaveChanges();

            response.Data = "";
            return response;
        }

        public async Task<ServiceResponse<CartAllDto>> All(HttpRequest request)
        {
            var response = new ServiceResponse<CartAllDto>();
            var token = request.Headers["x-access-token"].ToString();
            JwtSecurityToken? jwttoken;
            if (token is not null)
            {
                jwttoken = TokenService.TokenService.ValidateToken(token, _configuration);
            }
            else
            {
                response.Success = false;
                return response;
            }
            if (jwttoken is null)
            {
                response.Success = false;
                response.StatusCode = 401;
                return response;
            }
            var userIdString = jwttoken.Claims.First(x => x.Type == "nameid").Value;
            int userId = int.Parse(userIdString);
            var user = _context.Users.ToList().Find(u => u.Id == userId);
            if (user is null)
            {
                response.Success = false;
                return response;
            }
            if(user.CartId is null)
            {
                response.Success = false;
                response.Message = "У пользователя нет товаров";
                return response;
            }
            var cart = _context.Carts.ToList().Find(c => c.Id == user.CartId);
            if(cart is null)
            {
                response.Success = false;
                response.Message = "У пользователя нет товаров";
                return response;
            }
            var CartItems = _context.CartItems.ToList().FindAll(c => c.CartId == cart.Id);
            if(CartItems is null)
            {
                response.Success = false;
                response.Message = "У пользователя нет товаров";
                return response;
            }
            List<ItemDto> itemDtos = new List<ItemDto>();
            foreach ( var item in CartItems)
            {
                var FoundItem = _context.Items.ToList().Find(i => i.Id == item.ItemId);
                if (FoundItem is null) continue;
                List<Variant>? FoundVariants = new ();
                if(item.Variants is not null)
                {
                    foreach (var variant in item.Variants)
                    {
                        FoundVariants.Add(_context.Variants.ToList().Find(v => v.Id == variant)!);
                    }
                    if (!FoundVariants.Any())
                    {
                        FoundVariants = null;
                    }
                    
                }
                else FoundVariants = null;



                if (FoundVariants is null && item.Variants is not null)
                {
                    response.Success = false;
                    response.Message = "Ошибка нахождения варианты товара";
                    return response;
                }
                ItemDto itemDto = new ItemDto()
                {
                    Item = FoundItem,
                    Amount = item.Amount,
                    Variants = FoundVariants switch
                    {
                       null => null,
                       _ => FoundVariants.ToArray(),
                    }
                };
                itemDtos.Add(itemDto);
                
            }
            if (itemDtos.Count == 0)
            {
                response.Success = false;
                response.Message = "Ошибка поиска товаров";
                return response;
            }
            CartAllDto cartAll = new CartAllDto()
            {
                CartItems = itemDtos,
                TotalPrice = (int)cart.TotalPrice!,
            };
            response.Data = cartAll;
            return response;
        }

        public Task<ServiceResponse<string>> Count(HttpRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> Delete(HttpRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> Update(HttpRequest request)
        {
            throw new NotImplementedException();
        }
    }
}