namespace WebApi.Dtos.Item
{
    public class IItem
    {
        public string Name {get; set;} = string.Empty;
        public string Description {get; set;} = string.Empty;
        public int? Price {get; set;} = null;
        public List<string>? Icon {get; set;} = null;
        public Dictionary<string, string?>? Sizes { get; set;} = null;
        public bool IsAKit {get; set;} = false;
        public int? ParentId {get; set;} = null;
        public string Type {get; set;} = "Item";
    }
}