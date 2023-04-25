using WebApi.Dtos.Cart;
using WebApi.Dtos.Item;

namespace WebApi.Services
{
    public class KitService : IKitService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly TokenService _tokenService;
        public KitService(DataContext context, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
            _tokenService = new TokenService(_context, _configuration);

        }
        public async Task<ServiceResponse<Kit>> Add(Kit addKit, string token)
        {
            var response = new ServiceResponse<Kit>();
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
            if (!_context.Items.Any(i => i.Id == addKit.ItemId))
            {
                response.Success = false;
                response.Message = "Item with this id does not exists";
                return response;
            }
            foreach (var variant in addKit.Variants)
            {
                if (!_context.Variants.Any(v => v.ItemId == addKit.ItemId && v.VariantId == variant))
                {
                    response.Success = false;
                    response.Message = "Variant with this id does not exists";
                    return response;
                }
            }
            addKit.Variants.Sort();
            if (_context.Kits.ToList().Any(k => k.ItemId == addKit.ItemId && k.Variants.SequenceEqual(addKit.Variants)))
            {
                response.Success = false;
                response.Message = "Kit with this variants already exists";
                return response;
            }
            int maxId = _context.Kits.Where(v => v.ItemId == addKit.ItemId).Max(v => (int?)v.KitId) ?? 0;
            addKit.KitId = ++maxId;
            _context.Kits.Add(addKit);
            await _context.SaveChangesAsync();
            return response;
        }
        public async Task<ServiceResponse<Kit>> Update(KitUpdateDto kitInfo, string token)
        {
            var response = new ServiceResponse<Kit>();
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
            var kit = _context.Kits.DefaultIfEmpty().First(v => v.ItemId == kitInfo.ItemId && v.KitId == kitInfo.KitId);
            if (kit == null)
            {
                response.Success = false;
                response.Message = "Variant or item with this id does not exists";
                return response;
            }
            foreach (var value in kitInfo.GetType().GetProperties())
            {
                if (value.GetValue(kitInfo) is not null && value.Name != "ItemId" && value.Name != "VariantId")
                {
                    var property = kit.GetType().GetProperty(value.Name) ?? null;
                    if (property != null)
                    {
                        var propValue = property.GetValue(kit);
                        if (property.Name == "Variants")
                        {
                            var propertyList = new List<int>((List<int>)propValue!);
                            if (propertyList.Count() == 0)
                            {
                                response.Success = false;
                                response.Message = "Variants amount cannot be zero";
                                return response;
                            }
                            propertyList.Sort();
                            property.SetValue(kit, propertyList, null);
                        }
                        else
                        {
                            property.SetValue(kit, value.GetValue(kitInfo), null);
                        }

                    }
                }
            }
            _context.Kits.Update(kit);
            await _context.SaveChangesAsync();
            response.Data = kit;
            return response;
        }
    }
}