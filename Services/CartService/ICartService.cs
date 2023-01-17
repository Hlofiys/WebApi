namespace WebApi.Services.CartService
{
    public interface ICartService
    {
        Task<ServiceResponse<string>> Add(string Id, string Amount, HttpRequest request);
        Task<ServiceResponse<string>> All(HttpRequest request);
        Task<ServiceResponse<string>> Count(HttpRequest request);
        Task<ServiceResponse<string>> Delete(HttpRequest request);
        Task<ServiceResponse<string>> Update(HttpRequest request);

    }
}