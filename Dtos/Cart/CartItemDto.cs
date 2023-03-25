namespace WebApi.Dtos.Cart
{
    public class CartItemDto
    {
        public Models.Item? Item { get; set; } = null;
        public VariantDto[]? Variants { get; set; } = null;
        public KitDto? Kit { get; set; } = null;
        public int? Amount { get; set; } = null;
        public int? Price { get; set; } = null;
    }
}
