namespace WebApi.Dtos.Kit
{
    public class KitAddDto
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Description { get; set; } = new List<string>();
        public List<int>? Variants { get; set; } = null;
        public int? Price { get; set; } = null;
        public List<string>? Icon { get; set; } = null;
        public int? ItemId { get; set; } = null;
    }
}
