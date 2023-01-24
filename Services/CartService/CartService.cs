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
        private readonly IMapper _mapper;
        public CartService(DataContext context, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;

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
                    if (_context.CartItems.ToList().Find(c => c.CartId == cart.Id && c.ItemId == itemId) is not null)
                    {
                        response.Success = false;
                        response.Message = "This item is already in the cart";
                        return response;
                    }
                } 
            }
            bool VariantExists;
            int TotalVariantsPrice = 0;
            Kit? Kit = null;
            List<Variant> FoundVariants = new();
            if(Variants is not null)
            {
                Kit = _context.Kits.ToList().Find(k => k.ItemId == itemId);
                bool isArrayEqual = true;
                if(Kit?.Variants?.ToArray().Length == Variants.Length)
                {
                    for (int i = 0; i < Variants.Length; i++)
                    {
                        if ((Kit?.Variants?.ToArray()[i] != Variants[i])) { isArrayEqual = false; break; }
                    }
                } else isArrayEqual = false;
                if (isArrayEqual)
                {
                    FoundVariants?.Add(_mapper.Map<Variant>(Kit));
                }
                else
                {
                    Kit = null;
                    foreach (var variant in Variants)
                    {
                        if (variant > 0)
                        {
                            FoundVariants?.Add(_context.Variants.ToList().Find(v => v.VariantId == variant && v.ItemId == item.Id)!);
                            if (!FoundVariants!.Any())
                            {
                                response.Success = false;
                                response.Message = "This option is not";
                                return response;
                            }
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "This option is not";
                            return response;
                        }
                    }
                }
                
                foreach (var variant in FoundVariants!)
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
            };
            if(Kit is not null)
            {
                cartItem.Kit = Kit.KitId;
            }
            else
            {
                cartItem.Variants = Variants;
            }

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
                response.Message = "The user has no products";
                return response;
            }
            var cart = _context.Carts.ToList().Find(c => c.Id == user.CartId);
            if(cart is null)
            {
                response.Success = false;
                response.Message = "The user has no products";
                return response;
            }
            var CartItems = _context.CartItems.ToList().FindAll(c => c.CartId == cart.Id);
            if(CartItems is null || CartItems.Count == 0)
            {
                response.Success = false;
                response.Message = "The user has no products";
                return response;
            }
            List<CartItemDto> itemDtos = new List<CartItemDto>();
            foreach ( var item in CartItems)
            {
                var FoundItem = _context.Items.ToList().Find(i => i.Id == item.ItemId);
                if (FoundItem is null) continue;
                List<Variant>? FoundVariants = new ();
                Kit? FoundKit = null;
                if (item.Variants is not null)
                {
                    foreach (var variant in item.Variants)
                    {
                        FoundVariants.Add(_context.Variants.ToList().Find(v => v.VariantId == variant && v.ItemId == item.ItemId)!);
                    }
                    if (!FoundVariants.Any())
                    {
                        FoundVariants = null;
                    }

                }
                else FoundVariants = null;

                if (FoundVariants is null && item.Variants is not null && item.Variants.Length > 0)
                {
                    response.Success = false;
                    response.Message = "Error finding product variant";
                    return response;
                }

                if(item.Kit is not null)
                {
                    FoundKit = _context.Kits.ToList().Find(k => k.KitId== item.Kit && k.ItemId == item.ItemId);
                };

                if (FoundKit is null && item.Kit is not null)
                {
                    response.Success = false;
                    response.Message = "Error finding product kit";
                    return response;
                }
                CartItemDto itemDto = new CartItemDto()
                {
                    Item = FoundItem,
                    Amount = item.Amount,
                    Variants = FoundVariants switch
                    {
                       null => null,
                       _ => FoundVariants.ToArray(),
                    },
                    Kit = FoundKit,
                };
                itemDtos.Add(itemDto);
                
            }
            if (itemDtos.Count == 0)
            {
                response.Success = false;
                response.Message = "Product search error";
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

        public async Task<ServiceResponse<string>> Delete(int Id, int[] Variants, HttpRequest request)
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
            if (user is null)
            {
                response.Success = false;
                return response;
            }
            if (user.CartId is null)
            {
                response.Success = false;
                response.Message = "The user has no products";
                return response;
            }
            var cart = _context.Carts.ToList().Find(c => c.Id == user.CartId);
            if (cart is null)
            {
                response.Success = false;
                response.Message = "The user has no products";
                return response;
            }
            var CartItem = _context.CartItems.ToList().Find(c => c.CartId == cart.Id && c.ItemId == Id);
            if (CartItem is null)
            {
                response.Success = false;
                response.Message = "The user does not have a product with this id";
                return response;
            }
            if(Variants is not null)
            {
                if(CartItem?.Variants?.Length > 0) 
                {
                    int OriginalVariantsLength = CartItem.Variants.Length;
                    foreach (var variant in CartItem?.Variants!)
                    {
                        if(Variants.Contains(variant))
                        {
                            cart.TotalPrice -= (int)_context.Variants.ToList().Find(v => v.VariantId == variant && CartItem?.ItemId == Id)?.Price! * CartItem?.Amount;
                            CartItem!.Variants = CartItem?.Variants!.Where(v => v != variant).ToArray();
                            
                        }
                        if(CartItem?.Variants is null || CartItem?.Variants.Length == 0)
                        {
                            cart.TotalPrice += _context.Items.ToList().Find(i => i.Id == CartItem?.ItemId)?.Price * CartItem?.Amount;
                        }
                    }
                    if(OriginalVariantsLength != CartItem.Variants.Length)
                    {
                        _context.CartItems.Update(CartItem);
                        _context.Carts.Update(cart);
                        _context.SaveChanges();
                        return response;
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "The product does not have such options";
                        return response;
                    }
                }
                else if(CartItem?.Kit is not null)
                {
                    List<int> NewVariants = new();
                    var Kit = _context.Kits.ToList().Find(k => k.KitId == CartItem.Kit && k.ItemId == CartItem.ItemId);
                    if(Kit is not null)
                    {
                        foreach (var variant in Kit?.Variants!)
                        {
                            if (!Variants.Contains(variant))
                            {
                                NewVariants.Add(variant);
                            }
                        }
                        if(NewVariants.Count > 0)
                        {
                            int TotalPrice = 0;
                            foreach (var variant in NewVariants)
                            {
                                TotalPrice += (int)_context.Variants.ToList().Find(v => v.VariantId == variant && v.ItemId == CartItem.ItemId)?.Price! * CartItem.Amount;
                            }
                            cart.TotalPrice -= Kit.Price * CartItem.Amount;
                            cart.TotalPrice += TotalPrice;
                            CartItem.Kit = null;
                            CartItem.Variants = NewVariants.ToArray();
                            _context.CartItems.Update(CartItem);
                            _context.Carts.Update(cart);
                            _context.SaveChanges();
                            return response;
                        } 
                        else if(NewVariants.Count == 0)
                        {
                            cart.TotalPrice -= Kit.Price * CartItem.Amount;
                            cart.TotalPrice += _context.Items.ToList().Find(i => i.Id == CartItem.ItemId)?.Price * CartItem.Amount;
                            CartItem.Kit = null;
                            CartItem.Variants = null;
                            _context.CartItems.Update(CartItem);
                            _context.Carts.Update(cart);
                            _context.SaveChanges();
                            return response;
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "Error";
                            return response;
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Error in DataBase";
                        return response;
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "Product doesn't have such variants";
                    return response;
                }
            }
            else
            {
                cart.TotalPrice -= _context.Items.ToList().Find(i => i.Id == CartItem.ItemId)?.Price * CartItem.Amount;
                _context.CartItems.Remove(CartItem);
                _context.Carts.Update(cart);    
                _context.SaveChanges();
                return response;
            }
            /*response.Success = false;
            response.Message = "Œ¯Ë·Í‡";
            return response;*/

        }

        public Task<ServiceResponse<string>> Update(HttpRequest request)
        {
            throw new NotImplementedException();
        }
    }
}