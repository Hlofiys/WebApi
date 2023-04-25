namespace WebApi.Models
{
    public class Variant
    {
        public int Id { get; set; }
        public int VariantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Description {get; set;} = new List<string>();
        public int? Price { get; set; } = null;
        public List<string>? Icon { get; set; } = null;
        public int? ItemId { get; set; } = null;
    }
}
