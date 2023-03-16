

namespace WebApi.Services.OrderService
{
    public interface IOrderService
    {
        Task<ServiceResponse<string>> Create(Order order , HttpRequest request);
        Task<ServiceResponse<List<OrderAllDto>>> All(HttpRequest request);
    }
}
