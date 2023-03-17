namespace WebApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int? UserId { get; set; } = null;
        public bool? Shipping { get; set; } = null;
        public string Address { get; set; } = string.Empty;
        public string FIO { get; set; } = string.Empty;
        public string PhoneNubmer { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? Contact { get; set; } = string.Empty;
        public int TotalPrice { get; set; } = 0;
        

    }
}
