namespace WebApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int? UserId { get; set; } = null;
        public bool? Shipping { get; set; } = null;
        public string? Address { get; set; } = null;
        public string FIO { get; set; } = string.Empty;
        public string PhoneNubmer { get; set; } = string.Empty;
        public string? ZipCode { get; set; } = null;
        public string? City { get; set; } = null;
        public string? Contact { get; set; } = null;
        public Dictionary<string, string>? FullDate { get; set; } = null;
        public int TotalPrice { get; set; } = 0;

        

    }
}
