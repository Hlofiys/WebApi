namespace WebApi.Dtos.Cart
{
    public class ItemDto
    {
        public Item? Item { get; set; } = null;
        public Variant? Variant { get; set; } = null;
        public int? Amount { get; set; } = null;
    }
}
