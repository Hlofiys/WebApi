using Microsoft.AspNetCore.Mvc;
using WebApi.Services.CartService;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService itemService, IMapper mapper)
        {
            _userService = itemService;
            _mapper = mapper;
        }
        [HttpPost("SetInfo")]
        public async Task<ActionResult<ServiceResponse<string>>> SetInfo(UserSetInfoDto setInfo)
        {
            var token = Request.Headers["x-access-token"].ToString();
            if(token is null)
            {
                return BadRequest();
            }
            var response = await _userService.SetInfo(setInfo, token);
            if(response.StatusCode == 401) return Unauthorized(response);
            if(!response.Success) return BadRequest(response);
            var MapperResponse = _mapper.Map<ServiceResponseDto<string>>(response);
            return Ok(MapperResponse);
        }
        [HttpPost("GetInfo")]
        public async Task<ActionResult<ServiceResponse<UserGetInfoDto>>> GetInfo()
        {
            var token = Request.Headers["x-access-token"].ToString();
            if(token is null)
            {
                return BadRequest();
            }
            var response = await _userService.GetInfo(token);
            if(response.StatusCode == 401) return response;
            if(!response.Success) return response;
            var MapperResponse = _mapper.Map<ServiceResponseDto<UserGetInfoDto>>(response);
            return Ok(MapperResponse);
        }
    }
}