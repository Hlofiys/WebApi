namespace WebApi.Models
{
    public class Item
    {
        public int Id {get ; set;}
        public string Name {get; set;} = string.Empty;
        public string Description {get; set;} = string.Empty;
        public int? Price {get; set;} = null;
        public List<string>? Icon {get; set;} = null;
        public string Video {get; set;} = string.Empty;
        public Dictionary<string, string?>? Sizes { get; set;} = null;
    }
}