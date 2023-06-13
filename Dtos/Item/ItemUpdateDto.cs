namespace WebApi.Dtos.Item
{
    public class ItemUpdateDto : IItem
    {
        public new string? Name {get; set;} = null;
        public int? Id {get; set;}
        public new string? Description {get; set;} = null;
        public new bool? IsAKit {get; set;} = null;
    }
}