namespace WebApi.Dtos.Cart
{
    public class CartDeleteDto
    {
        public int? Id { get; set; } = null;
        public int[]? Variants { get; set; } = null;
    }
}
