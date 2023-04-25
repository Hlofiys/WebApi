namespace WebApi.Dtos.Variant
{
    public class IVariant
    {
        public string Name { get; set; } = string.Empty;
        public List<string>? Description { get; set; } = null;
        public int? Price { get; set; } = null;
        public List<string>? Icon { get; set; } = null;
    }
}
