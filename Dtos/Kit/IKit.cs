namespace WebApi.Dtos.Kit
{
    public class IKit
    {
        public string Name { get; set; } = string.Empty;
        public List<int>? Variants { get; set; } = null;
        public int? Price { get; set; } = null;
        public List<string>? Icon { get; set; } = null;
    }
}
