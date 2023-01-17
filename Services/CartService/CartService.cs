namespace WebApi.Services.CartService
{
    public class CartService : ICartService
    {
        public ServiceResponse<string> CheckToken(HttpRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> Delete(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> Login(string username, string password, HttpResponse response)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> Refresh(HttpRequest request, HttpResponse response)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<string>> Register(User user, string password, HttpResponse response)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExists(string username)
        {
            throw new NotImplementedException();
        }
    }
}