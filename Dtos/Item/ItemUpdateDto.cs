namespace WebApi.Dtos.Item
{
    public class ItemUpdateDto
    {
        public int? Id {get; set;}
        public string? Name {get; set;} = null;
        public List<string>? Description {get; set;} = null;
        public int? Price {get; set;} = null;
        public List<string>? Icon {get; set;} = null;
        public Dictionary<string, string?>? Sizes { get; set;} = null;
    }
}