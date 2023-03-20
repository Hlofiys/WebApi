using WebApi.Dtos.Cart;

namespace WebApi.Dtos.Order
{
    public class OrderAllDto
    {
        public int Id { get; set; }
        public bool? Shipping { get; set; } = null;
        public string? Address { get; set; } = string.Empty;
        public string FIO { get; set; } = string.Empty;
        public string PhoneNubmer { get; set; } = string.Empty;
        public string? Contact { get; set; } = string.Empty;
        public string? ZipCode { get; set; } = null;
        public string? City { get; set; } = string.Empty;
        public Dictionary<string, string>? FullDate { get; set; } = null;
        public List<CartItemDto>? CartItems { get; set; } = null;
        public int TotalPrice { get; set; } = 0;
    }
}
