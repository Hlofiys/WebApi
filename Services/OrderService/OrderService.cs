using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Dtos.Cart;
using WebApi.Models;

namespace WebApi.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public OrderService(DataContext context, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;

        }

        public async Task<ServiceResponse<List<OrderAllDto>>> All(HttpRequest request)
        {
            var response = new ServiceResponse<List<OrderAllDto>>();
            var token = request.Headers["x-access-token"].ToString();
            var UserSearchResult = UserSearch(token);
            if (UserSearchResult!.StatusCode == 401)
            {
                response.StatusCode = 401;
                response.Success = false;
                return response;
            }
            if (UserSearchResult!.Success == false)
            {
                response.Success = false;
                return response;
            }
            var user = UserSearchResult.Data!;
            var orders = _context.Orders.ToList().FindAll(o => o.UserId == user.Id);
            if (!orders.Any())
            {
                response.Success = false;
                response.Message = "The user has no orders";
                return response;
            }
            List<OrderAllDto> OrderAllDtos = new List<OrderAllDto>();
            foreach (var order in orders)
            {
                var OrderItems = _context.OrderItems.ToList().FindAll(o => o.OrderId == order.Id);
                if (OrderItems is null || OrderItems.Count == 0)
                {
                    response.Success = false;
                    response.Message = "The user has no products";
                    return response;
                }
                List<CartItemDto> itemDtos = new List<CartItemDto>();
                foreach (var item in OrderItems)
                {
                    var FoundItem = _context.Items.ToList().Find(i => i.Id == item.ItemId);
                    if (FoundItem is null) continue;
                    List<Variant>? FoundVariants = new();
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

                    if (item.Kit is not null)
                    {
                        FoundKit = _context.Kits.ToList().Find(k => k.KitId == item.Kit && k.ItemId == item.ItemId);
                    };

                    if (FoundKit is null && item.Kit is not null)
                    {
                        response.Success = false;
                        response.Message = "Error finding product kit";
                        return response;
                    }
                    List<VariantDto>? variantDto = new();
                    if (FoundVariants is not null)
                    {
                        foreach (var var in FoundVariants)
                        {
                            variantDto.Add(_mapper.Map<VariantDto>(var));
                        }
                    }
                    else
                    {
                        variantDto = null;
                    }
                    CartItemDto itemDto = new CartItemDto()
                    {
                        Item = FoundItem,
                        Amount = item.Amount,
                        Price = item.Price,
                        Variants = variantDto switch
                        {
                            null => null,
                            _ => variantDto.ToArray(),
                        },
                        Kit = _mapper.Map<KitDto>(FoundKit),
                    };
                    itemDtos.Add(itemDto);

                }
                if (itemDtos.Count == 0)
                {
                    response.Success = false;
                    response.Message = "Product search error";
                    return response;
                }
                OrderAllDto cartAll = new OrderAllDto()
                {
                    CartItems = itemDtos,
                    TotalPrice = order.TotalPrice,
                    FIO = order.FIO,
                    Address= order.Address,
                    Shipping = order.Shipping,
                    Contact = order.Contact!,
                    PhoneNubmer = order.PhoneNubmer,
                    Id = order.Id,
                    ZipCode= order.ZipCode,
                    City= order.City,
                };
                OrderAllDtos.Add(cartAll);
            }
            if (!OrderAllDtos.Any())
            {
                response.Success = false;
                return response;
            }
            response.Data = OrderAllDtos;
            return response;
            
        }

        public async Task<ServiceResponse<string>> Create(Order order, HttpRequest request)
        {
            var response = new ServiceResponse<string>();
            var token = request.Headers["x-access-token"].ToString();
            var UserSearchResult = UserSearch(token);
            if (UserSearchResult!.StatusCode == 401)
            {
                response.StatusCode = 401;
                response.Success = false;
                return response;
            }
            if (UserSearchResult!.Success == false)
            {
                response.Success = false;
                return response;
            }
            var user = UserSearchResult.Data!;
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
            var CartItems = _context.CartItems.ToList().FindAll(c => c.CartId == cart.Id);
            if (CartItems is null || CartItems.Count == 0)
            {
                response.Success = false;
                response.Message = "The user has no products";
                return response;
            }
            order.UserId = user.Id;
            order.TotalPrice = cart.TotalPrice;
            _context.Orders.Add(order);
            _context.SaveChanges();
            /*List<OrderItem> orderItems = new List<OrderItem>();*/
            foreach (var cartItem in CartItems)
            {
                OrderItem orderItem = (OrderItem)cartItem;
                orderItem.OrderId = order.Id;
                /*orderItems.Add(orderItem);*/
                _context.OrderItems.Add(orderItem);
                _context.CartItems.Remove(cartItem);
            }
            /*if (!orderItems.Any())
            {
                response.Success = false;
                return response;
            }*/
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return response;
        }
        public ServiceResponse<User>? UserSearch(string token)
        {
            var response = new ServiceResponse<User>();
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
            response.Data = user;
            return response;
        }
    }
}
