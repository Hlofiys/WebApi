namespace WebApi.Dtos.Item
{
    public class ItemUpdateDto : IItem
    {
        public int? Id {get; set;}
        public new List<string>? Description {get; set;} = null;
    }
}