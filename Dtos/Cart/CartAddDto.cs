namespace WebApi.Dtos.Cart
{
    public class CartAddDto
    {
        public int? Id { get; set; } = null;
        public int? Amount { get; set; } = null;
        public int Variant { get; set; } = 0;
    }
}
