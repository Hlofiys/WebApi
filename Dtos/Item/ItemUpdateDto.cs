namespace WebApi.Dtos.Item
{
    public class ItemUpdateDto : IItem
    {
        public new string? Name {get; set;} = null;
        public int? Id {get; set;}
        public new List<string>? Description {get; set;} = null;
    }
}