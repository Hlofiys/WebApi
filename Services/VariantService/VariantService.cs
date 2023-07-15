using WebApi.Dtos.Cart;
using WebApi.Dtos.Item;

namespace WebApi.Services
{
    public class VariantService : IVariantService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly TokenService _tokenService;
        public VariantService(DataContext context, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
            _tokenService = new TokenService(_context, _configuration);

        }
        public async Task<ServiceResponse<Variant>> Add(Variant addVariant, string token)
        {
            var response = new ServiceResponse<Variant>();
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
            if (!_context.Items.Any(i => i.Id == addVariant.ItemId))
            {
                response.Success = false;
                response.Message = "Item does not exists";
                return response;
            }
            if (_context.Variants.Any(v => v.ItemId == addVariant.ItemId && v.Name == addVariant.Name))
            {
                response.Success = false;
                response.Message = "Variant with this name already exists";
                return response;
            }
            int maxId = _context.Variants.Where(v => v.ItemId == addVariant.ItemId).Max(v => (int?)v.VariantId) ?? 0;
            addVariant.VariantId = ++maxId;
            _context.Variants.Add(addVariant);
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<ServiceResponse<string>> Delete(VariantDeleteDto variantInfo, string token)
        {
            var response = new ServiceResponse<string>();
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
            var variant = _context.Variants.FirstOrDefault(v => v.ItemId == variantInfo.ItemId && v.VariantId == variantInfo.VariantId);
            if (variant == null || variant?.Id == null)
            {
                response.Success = false;
                response.Message = "Variant or item with this id does not exists";
                return response;
            }
            _context.Variants.Remove(variant);
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<ServiceResponse<Variant>> Update(VariantUpdateDto variantInfo, string token)
        {
            var response = new ServiceResponse<Variant>();
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
            var variant = _context.Variants.DefaultIfEmpty().FirstOrDefault(v => v.ItemId == variantInfo.ItemId && v.VariantId == variantInfo.VariantId);
            if (variant == null)
            {
                response.Success = false;
                response.Message = "Variant or item with this id does not exists";
                return response;
            }
            if (variant.Name == variantInfo.Name)
            {
                response.Success = false;
                response.Message = "Variant already has this name";
                return response;
            }
            if (_context.Variants.Any(v => v.ItemId == variantInfo.ItemId && v.Name == variantInfo.Name))
            {
                response.Success = false;
                response.Message = "Variant with this name already exists";
                return response;
            }
            foreach (var value in variantInfo.GetType().GetProperties())
            {
                if (value.GetValue(variantInfo) is not null && value.Name != "ItemId" && value.Name != "VariantId")
                {
                    var property = variant.GetType().GetProperty(value.Name) ?? null;
                    if (property != null)
                    {
                        var propValue = property.GetValue(variant);
                        property.SetValue(variant, value.GetValue(variantInfo), null);

                    }
                }
            }
            _context.Variants.Update(variant);
            await _context.SaveChangesAsync();
            response.Data = variant;
            return response;
        }
    }
}