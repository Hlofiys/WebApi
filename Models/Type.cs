namespace WebApi.Models
{
    public class Type
    {
        public int Id {get ; set;}
        public string Name {get; set;} = String.Empty;
        public string Description {get; set;} = String.Empty;
        public int? Price {get; set;} = null;
        public List<string>? Icon {get; set;} = null;
    }
}