namespace WebApi.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int? ItemId { get; set; } = null;
        public int[]? Variants { get; set; } = null;
        public int? Kit { get; set; } = null;
        public int Amount { get; set; }
        public int Price { get; set; }
        public int? OrderId { get; set; } = null;

        public static explicit operator OrderItem(CartItem v)
        {
            return new OrderItem { Amount = v.Amount, Price = v.Price, Variants = v.Variants, Kit = v.Kit, ItemId = v.ItemId };
        }
    }
}
