using WebApi.Dtos.Cart;

namespace WebApi.Dtos.Order
{
    public class OrderUpdateStatusDto
    {
        public int Id { get; set; }
        public Models.Order.ShippingStatuses shippingStatuses {get; set;}
    }
}
