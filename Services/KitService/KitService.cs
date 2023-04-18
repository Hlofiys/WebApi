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
        public async Task<ServiceResponse<Kit>> Add(Kit addKit, string token){
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
            if(!user.IsAdmin){
                response.Success = false;
                response.Message = "You are not an admin!";
                return response;
            }
            if(!_context.Items.Any(i => i.Id == addKit.ItemId)) {
                response.Success = false;
                response.Message = "Item with this id does not exists";
                return response;
            }
            foreach (var variant in addKit.Variants)
            {
                if(!_context.Variants.Any(v => v.ItemId == addKit.ItemId && v.VariantId == variant)){
                    response.Success = false;
                    response.Message = "Variant with this id does not exists";
                    return response;
                }
            }
            addKit.Variants.Sort();
            if(_context.Kits.ToList().Any(k => k.ItemId == addKit.ItemId && k.Variants.SequenceEqual(addKit.Variants))){
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
    }
}