namespace WebApi.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int? ItemId { get; set; } = null;
        public int[]? Variants { get; set; } = null;
        public int? Kit { get; set; } = null;
        public int Amount { get; set; }
        public int? CartId { get; set; } = null;

    }
}
