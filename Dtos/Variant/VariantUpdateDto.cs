namespace WebApi.Dtos.Variant
{
    public class VariantUpdateDto : IVariant
    {
        public int? ItemId {get; set;}
        public int? VariantId {get; set;}
        public new string? Name { get; set; } = null;
        public new string? Description {get; set;} = null;
    }
}