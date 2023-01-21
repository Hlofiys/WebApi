namespace WebApi.Dtos.Cart
{
    public class ItemDto
    {
        public Item? Item { get; set; } = null;
        public Variant[]? Variants { get; set; } = null;
        public Kit? Kit { get; set; } = null;
        public int? Amount { get; set; } = null;
    }
}
