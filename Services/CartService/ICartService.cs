using WebApi.Dtos.Cart;

namespace WebApi.Services.CartService
{
    public interface ICartService
    {
        Task<ServiceResponse<string>> Add(int Id, int Amount,int[] Variants , HttpRequest request);
        Task<ServiceResponse<CartAllDto>> All(HttpRequest request);
        Task<ServiceResponse<string>> Count(HttpRequest request);
        Task<ServiceResponse<string>> Delete(int Id, int[] Variants, HttpRequest request);
        Task<ServiceResponse<string>> Update(int Id, int[]? Variants, int? Amount, HttpRequest request);

    }
}