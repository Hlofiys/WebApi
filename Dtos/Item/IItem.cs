namespace WebApi.Dtos.Item
{
    public class IItem
    {
        public string Name {get; set;} = string.Empty;
        public List<string> Description {get; set;} = new List<string>();
        public int? Price {get; set;} = null;
        public List<string>? Icon {get; set;} = null;
        public Dictionary<string, string?>? Sizes { get; set;} = null;
    }
}