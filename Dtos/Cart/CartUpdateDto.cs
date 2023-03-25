namespace WebApi.Dtos.Cart
{
    public class CartUpdateDto
    {
        public int? Id { get; set; } = null;
        public int[]? Variants { get; set; } = null;
        public int? Amount { get; set; } = null;
    }
}
