using WebApi.Dtos.Cart;
using WebApi.Dtos.Item;

namespace WebApi.Services.ItemService
{
    public class ItemService : IItemService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public ItemService(DataContext context, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;

        }
        public ServiceResponse<Item[]> GetAll()
        {
            var response = new ServiceResponse<Item[]>();
            Item[] items = _context.Items.ToArray();
            response.Data = items;
            return response;
        }

        public async Task<ServiceResponse<ItemById>> GetById(int id)
        {
            var response = new ServiceResponse<ItemById>();
            ItemById itemById = new ItemById();
            Item item = _context.Items.ToList().Find(i => i.Id == id)!;
            if (item is not null)
                itemById.Item = item;
            else response.Success = false;
            List<Variant> variants = _context.Variants.ToList().FindAll(v => v.ItemId == id);
            
            if(variants.Any()) {
                List<VariantDto> variantsDto = new List<VariantDto>();
                foreach (var var in variants)
                {
                    variantsDto.Add(_mapper.Map<VariantDto>(var));
                }
                itemById.Variants = variantsDto;
            }

            List<Kit> kits = _context.Kits.ToList().FindAll(k => k.ItemId == id);

            if (kits.Any())
            {
                itemById.Kits = kits;
            }
            response.Data = itemById;
            return response;
        }
    }
}