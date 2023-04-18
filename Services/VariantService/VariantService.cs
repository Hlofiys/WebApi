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
        public async Task<ServiceResponse<Variant>> Add(Variant addVariant, string token){
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
            if(!user.IsAdmin){
                response.Success = false;
                response.Message = "You are not an admin!";
                return response;
            }
            if(!_context.Items.Any(i => i.Id == addVariant.ItemId)){
                response.Success = false;
                response.Message = "Item does not exists";
                return response;
            }
            int maxId = _context.Variants.Where(v => v.ItemId == addVariant.ItemId).Max(v => (int?)v.VariantId) ?? 0;
            addVariant.VariantId = ++maxId;
            _context.Variants.Add(addVariant);
            await _context.SaveChangesAsync();
            return response;
        }
    }
}