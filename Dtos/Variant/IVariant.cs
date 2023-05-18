namespace WebApi.Dtos.Variant
{
    public class IVariant
    {
        public string Name { get; set; } = string.Empty;
        public string Description {get; set;} = string.Empty;
        public int? Price { get; set; } = null;
        public List<string>? Icon { get; set; } = null;
    }
}
