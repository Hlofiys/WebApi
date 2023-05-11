using WebApi.Dtos.Cart;
using WebApi.Dtos.Item;

namespace WebApi.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly TokenService _tokenService;
        public UserService(DataContext context, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
            _tokenService = new TokenService(_context, _configuration);

        }

        public async Task<ServiceResponse<string>> SetInfo(UserSetInfoDto setInfo, string token)
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
            foreach (var value in setInfo.GetType().GetProperties())
            {
                if (value.GetValue(setInfo) is not null)
                {
                    if(user.UserInfo == null)
                    {
                        user.UserInfo = setInfo;
                    }
                    else 
                    {
                    if (user.UserInfo.Keys.Any(v => v == value.Name))
                    {
                        user.UserInfo[value.Name] = (string?)value.GetValue(setInfo);

                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "This key does not exists";
                        return response;
                    }
                    }
                }
            }
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return response;
        }
        public async Task<ServiceResponse<UserGetInfoDto>> GetInfo(string token)
        {
            var response = new ServiceResponse<UserGetInfoDto>();
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
            if(user.UserInfo == null)
            {
                response.Data = null;
                return response;
            }
            response.Data = new UserGetInfoDto();
            foreach (var prop in user.UserInfo)
            {
                var field = response.Data.GetType().GetProperty(prop.Key);
                if(field == null) continue;
                var value = prop.Value;
                field.SetValue(response.Data, value);
            }
            return response;
        }
    }
}