using System.Linq;
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
                        if (propValue.GetType() == typeof(Dictionary<string, string?>))
                        {
                            var propertyDictionary = new Dictionary<string, string?>((Dictionary<string, string?>)propValue);
                            foreach (var key in itemInfo.Sizes.Keys)
                            {
                                if (propertyDictionary.ContainsKey(key))
                                {
                                    propertyDictionary[key] = itemInfo.Sizes[key];
                                }
                                else
                                {
                                    response.Success = false;
                                    response.Message = "Size with this key has not found";
                                    return response;
                                }
                            }
                            property.SetValue(item, propertyDictionary, null);
                        }
                        else
                        {
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

        public async Task<ServiceResponse<List<ItemGetAllCombinations>>> GetAllCombinations(int id)
        {
            ServiceResponse<List<ItemGetAllCombinations>> ListItemGetAllCombinations = new ServiceResponse<List<ItemGetAllCombinations>>();
            var item = _context.Items.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                ListItemGetAllCombinations.Success = false;
                ListItemGetAllCombinations.Message = "Item with this id has not found";
                return ListItemGetAllCombinations;
            }
            int[] arr = _context.Variants.Where(v => v.ItemId == id).Select(v => v.VariantId).ToArray();
            int n = arr.Length;
            for (int i = 0; i < (1 << n); i++)
            {
                ItemGetAllCombinations itemGetAllCombinations = new ItemGetAllCombinations();
                itemGetAllCombinations.Variants = new VariantDto[] {};
                for (int j = 0; j < n; j++)
                {
                    if ((i & (1 << j)) > 0)
                        itemGetAllCombinations.Variants = itemGetAllCombinations.Variants.Append(_mapper.Map<VariantDto>(_context.Variants.FirstOrDefault(v => v.VariantId == arr[j] && v.ItemId == id))).ToArray();
                }
                Console.WriteLine(itemGetAllCombinations.Variants.Count());
                itemGetAllCombinations.Description = item.Description;
                itemGetAllCombinations.Icon = item.Icon;
                itemGetAllCombinations.Name = item.Name;
                itemGetAllCombinations.Sizes = item.Sizes;
                var variantsIds = itemGetAllCombinations.Variants.Select(v => v.Id).ToList();
                variantsIds = variantsIds.OrderBy(i => i).ToList();
                var kit = _context.Kits.ToList().Find(k => k.ItemId == id && k.Variants.SequenceEqual(variantsIds));
                if(kit != null) itemGetAllCombinations.Kit = _mapper.Map<KitDto>(kit);
                if(ListItemGetAllCombinations.Data == null)
                {
                    ListItemGetAllCombinations.Data = new List<ItemGetAllCombinations> ();
                }
                itemGetAllCombinations.Variants = itemGetAllCombinations.Variants.OrderBy(v => v.Id).ToArray();
                ListItemGetAllCombinations.Data.Add(itemGetAllCombinations);
            }
            ListItemGetAllCombinations.Data = ListItemGetAllCombinations.Data.OrderBy(i => i.Variants.Count()).ToList();
            return ListItemGetAllCombinations;
        }
    }
}