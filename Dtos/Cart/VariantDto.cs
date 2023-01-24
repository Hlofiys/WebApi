namespace WebApi.Dtos.Cart
{
    public class VariantDto
    {
        public int Id { get; set; }
        public int VariantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? Price { get; set; } = null;
        public List<string>? Icon { get; set; } = null;
        public int? ItemId { get; set; } = null;
        public string Video { get; set; } = string.Empty;
    }
}
