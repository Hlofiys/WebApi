namespace WebApi.Dtos.Variant
{
    public class VariantUpdateDto : IVariant
    {
        public int? ItemId {get; set;}
        public int? VariantId {get; set;}
    }
}