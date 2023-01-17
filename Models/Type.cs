namespace WebApi.Models
{
    public class Type
    {
        public int Id {get ; set;}
        public string Name {get; set;} = string.Empty;
        public string Description {get; set;} = string.Empty;
        public int? Price {get; set;} = null;
        public string Icon {get; set;} = string.Empty;
    }
}