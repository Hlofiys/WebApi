namespace WebApi.Dtos.Cart
{
    public class CartAllDto
    {
        public List<ItemDto>? CartItems { get; set; } = null;
        public int TotalPrice { get; set; } = 0;
    }
}
