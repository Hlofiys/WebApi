namespace WebApi.Dtos.Order
{
    public class OrderCreateDto
    {
        public string Shipping { get; set; } = string.Empty;
        public string? Address { get; set; } = null;
        public string FIO { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Contact { get; set; } = null;
        public string? ZipCode { get; set; } = null;
        public string? City { get; set; } = null;
        public Dictionary<string, string>? FullDate { get; set; } = null;
    }
}
