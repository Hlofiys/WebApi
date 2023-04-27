using WebApi.Dtos.Cart;
using WebApi.Dtos.Item;

namespace WebApi.Services
{
    public class ItemService : IItemService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly TokenService _tokenService;
        public ItemService(DataContext context, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
            _tokenService = new TokenService(_context, _configuration);

        }
        public ServiceResponse<List<Item>> GetAll()
        {
            var response = new ServiceResponse<List<Item>>();
            List<Item> items = _context.Items.ToList();
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

            if (variants.Any())
            {
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
        public async Task<ServiceResponse<Item>> Add(Item addItem, string token)
        {
            var response = new ServiceResponse<Item>();
            var UserSearchResult = _tokenService.UserSearch(token);
            if (UserSearchResult!.StatusCode == 401)
            {
                response.StatusCode = 401;
                response.Success = false;
                return response;
            }
            if (UserSearchResult!.Success == false)
            {
                response.Success = false;
                return response;
            }
            var user = UserSearchResult.Data!;
            if (!user.IsAdmin)
            {
                response.Success = false;
                response.Message = "You are not an admin!";
                return response;
            }

            _context.Items.Add(addItem);
            await _context.SaveChangesAsync();
            return response;
        }
        public async Task<ServiceResponse<Item>> Update(ItemUpdateDto itemInfo, string token)
        {
            var response = new ServiceResponse<Item>();
            var UserSearchResult = _tokenService.UserSearch(token);
            if (UserSearchResult!.StatusCode == 401)
            {
                response.StatusCode = 401;
                response.Success = false;
                return response;
            }
            if (UserSearchResult!.Success == false)
            {
                response.Success = false;
                return response;
            }
            var user = UserSearchResult.Data!;
            if (!user.IsAdmin)
            {
                response.Success = false;
                response.Message = "You are not an admin!";
                return response;
            }
            
            var item = _context.Items.DefaultIfEmpty().First(i => i.Id == itemInfo.Id);
            if (item == null)
            {
                response.Success = false;
                response.Message = "Item with this id does not exists";
                return response;
            }
            foreach (var value in itemInfo.GetType().GetProperties())
            {
                if (value.GetValue(itemInfo) is not null && value.Name != "Id")
                {
                    var property = item.GetType().GetProperty(value.Name) ?? null;
                    if (property != null)
                    {
                        var propValue = property.GetValue(item);
                        if(propValue.GetType() == typeof(Dictionary<string, string?>)){
                            var propertyDictionary = new Dictionary<string, string?>((Dictionary<string, string?>)propValue);
                            foreach (var key in propertyDictionary.Keys)
                            {
                                if(itemInfo.Sizes.ContainsKey(key)){
                                    propertyDictionary[key] = itemInfo.Sizes[key];
                                } else{
                                    response.Success = false;
                                    response.Message = "Size with this key has not found";
                                    return response;
                                }
                            }
                            property.SetValue(item, propertyDictionary, null);
                        } else{
                            property.SetValue(item, value.GetValue(itemInfo), null);
                        }
                        
                    }
                }
            }
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            response.Data = item;
            return response;
        }
    }
}