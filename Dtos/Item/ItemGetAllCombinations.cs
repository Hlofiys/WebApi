namespace WebApi.Dtos.Item
{
    public class ItemGetAllCombinations
    {
        public string Name {get; set;} = string.Empty;
        public string Description {get; set;} = string.Empty;
        public int? Price {get; set;} = null;
        public List<string>? Icon {get; set;} = null;
        public Dictionary<string, string?>? Sizes { get; set;} = null;
        public VariantDto[]? Variants { get; set; } = null;
        public KitDto? Kit { get; set; } = null;
    }
}