

namespace WebApi.Services.OrderService
{
    public interface IOrderService
    {
        Task<ServiceResponse<string>> Create(Order order , HttpRequest request);
        Task<ServiceResponse<List<OrderAllDto>>> All(HttpRequest request);
        Task<ServiceResponse<string>> StatusUpdate(int OrderId, string token, Order.ShippingStatuses shippingStatus);
    }
}
