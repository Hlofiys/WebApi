namespace WebApi.Dtos.Variant
{
    public class VariantAddDto : IVariant
    {
        public new List<string> Description { get; set; } = new List<string>();
        public int? ItemId { get; set; } = null;
    }
}
