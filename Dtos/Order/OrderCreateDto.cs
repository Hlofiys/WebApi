namespace WebApi.Dtos.Order
{
    public class OrderCreateDto
    {
        public string Shipping { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string FIO { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Contact { get; set; } = string.Empty;
    }
}
