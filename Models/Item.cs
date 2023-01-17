namespace WebApi.Models
{
    public class Item
    {
        public int Id {get ; set;}
        public string Name {get; set;} = String.Empty;
        public string Description {get; set;} = String.Empty;
        public int? Price {get; set;} = null;
        public List<string>? Icon {get; set;} = null;
        public string Video {get; set;} = String.Empty;
        public string TypeId {get; set;} = string.Empty;
    }
}