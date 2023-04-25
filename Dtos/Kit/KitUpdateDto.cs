namespace WebApi.Dtos.Kit
{
    public class KitUpdateDto : IKit
    {
        public int? ItemId { get; set; } = null;
        public int? KitId { get; set; } = null;
    }
}
