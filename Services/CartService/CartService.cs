using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
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
        public async Task<ServiceResponse<string>> Add(string ReqId, string ReqAmount, HttpRequest request)
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
            if(user is null){
                response.Success = false;
                return response;
            }
            var IsSuccessItemId = int.TryParse(ReqId, out var itemId);
            var IsSuccessItemAmount = int.TryParse(ReqAmount, out var itemAmount);
            
            if(!IsSuccessItemId || !IsSuccessItemAmount)
            {
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
                cartExists = true;
                cart = _context.Carts.ToList().Find(c => c.Id == user.CartId)!;
                if(_context.CartItems.ToList().Find(c => c.CartId == cart.Id) is not null)
                {
                    response.Success = false;
                    response.Message = "Этот товар уже есть в корзине";
                    return response;
                }
            }
            CartItem cartItem = new CartItem()
            {
                Amount = itemAmount,
                CartId = cart.Id,
                ItemId = item.Id,
            };
            cart.TotalPrice += item.Price * itemAmount;

            if(cartExists)
            _context.Update(cart);
            else
            {
                _context.Carts.Add(cart);
                _context.SaveChanges();
                user.CartId= cart.Id;
                _context.Update(user);
            }

            _context.CartItems.Add(cartItem);
            
            _context.SaveChanges();

            response.Data = "";
            return response;
        }

        public Task<ServiceResponse<string>> All(HttpRequest request)
        {
            throw new NotImplementedException();
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