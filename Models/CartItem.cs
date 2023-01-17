namespace WebApi.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public Item? Item { get; set; } = null;
        public int Amount { get; set; }
        public string CartId { get; set; } = string.Empty;

    }
}
