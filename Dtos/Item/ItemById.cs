using WebApi.Dtos.Cart;

namespace WebApi.Dtos.Item
{
    public class ItemById
    {
        public Models.Item Item { get; set; } = new Models.Item();
        public List<VariantDto> Variants { get; set; } = new List<VariantDto>();
        public List<Kit> Kits { get; set; } = new List<Kit>();

    }
}