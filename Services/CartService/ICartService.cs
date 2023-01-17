namespace WebApi.Services.CartService
{
    public interface ICartService
    {
        Task<ServiceResponse<string>> Register(User user, string password, HttpResponse response);
        Task<ServiceResponse<string>> Login(string username, string password, HttpResponse response);
        Task<ServiceResponse<bool>> Delete(string username, string password);

        Task<bool> UserExists(string username);

        ServiceResponse<string> CheckToken(HttpRequest request);
        Task<ServiceResponse<string>> Refresh(HttpRequest request, HttpResponse response);
    }
}