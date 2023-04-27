using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Dtos.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IMapper _mapper;
        private readonly TokenService _tokenService;

        public AuthController(IAuthRepository authRepo, IMapper mapper, TokenService tokenService)
        {
            _authRepo = authRepo;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [EnableCors("corssus")]
        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<string>>> Register(UserRegisterDto request)
        {
            var response = await _authRepo.Register(
                new User { Username = request.Username }, request.Password, Response
            );
            var MapperResponse = _mapper.Map<ServiceResponseDto<string>>(response);
            if (!response.Success)
            {
                return BadRequest(MapperResponse);
            }
            return Ok(MapperResponse);
        }
        [EnableCors("corssus")]
        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDto request)
        {
            var response = await _authRepo.Login(request.Username, request.Password, Response, Request);
            var MapperResponse = _mapper.Map<ServiceResponseDto<string>>(response);
            if (response.StatusCode == 403)
            {
                return UnprocessableEntity(MapperResponse);
            }
            if (!response.Success)
            {
                return BadRequest(MapperResponse);
            }
            return Ok(MapperResponse);
        }
        [EnableCors("corssus")]
        [HttpPost("Logout")]
        public async Task<ActionResult<ServiceResponse<string>>> Loginout()
        {
            var token = Request.Headers["x-access-token"].ToString();
            var UserSearchResult = _tokenService.UserSearch(token);
            if (UserSearchResult!.StatusCode == 401)
            {
                return Unauthorized();
            }
            if (UserSearchResult!.Success == false)
            {
                return BadRequest();
            }
            var user = UserSearchResult.Data!;
            var response = await _authRepo.Logout(user, Response);
            var MapperResponse = _mapper.Map<ServiceResponseDto<string>>(response);
            if (!response.Success)
            {
                return BadRequest(MapperResponse);
            }
            return Ok(MapperResponse);
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<ServiceResponse<int>>> Delete(UserLoginDto request)
        {
            var response = await _authRepo.Delete(request.Username, request.Password);
            var MapperResponse = _mapper.Map<ServiceResponseDto<String>>(response);
            if (!response.Success)
            {
                return BadRequest(MapperResponse);
            }
            return Ok(MapperResponse);
        }

        [HttpGet("checkToken")]
        public ActionResult<ServiceResponse<string>> checkToken()
        {
            var response = _authRepo.CheckToken(Request);
            var MapperResponse = _mapper.Map<ServiceResponseDto<string>>(response);
            if (response.StatusCode == 401) return Unauthorized(MapperResponse);
            if (!response.Success)
            {
                return BadRequest(MapperResponse);
            }
            return Ok(MapperResponse);
        }
        [EnableCors("corssus")]
        [HttpGet("Refresh")]
        public async Task<ActionResult<ServiceResponse<string>>> Refresh()
        {
            var response = await _authRepo.Refresh(Request, Response);
            var MapperResponse = _mapper.Map<ServiceResponseDto<string>>(response);
            if (response.StatusCode == 401) return Unauthorized(MapperResponse);
            if (response.StatusCode == 403) return Unauthorized(MapperResponse);
            if (!response.Success)
            {
                return NotFound(MapperResponse);
            }
            return Ok(MapperResponse);
        }
        [EnableCors("corssus")]
        [HttpPost("Activate/{id}")]
        public async Task<ActionResult<ServiceResponse<string>>> Activate(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var response = await _authRepo.Activate(id, Response, Request);
            var MapperResponse = _mapper.Map<ServiceResponseDto<string>>(response);
            if (!response.Success && response.StatusCode == 1)
            {
                return BadRequest(MapperResponse);
            }
            if (!response.Success && response.StatusCode == 2)
            {
                return NotFound(MapperResponse);
            }
            return Ok(MapperResponse);
        }
        [HttpGet("DeleteAll")]
        public async Task<ActionResult<ServiceResponse<string>>> DeleteAll()
        {
            var response = await _authRepo.DeleteAll();
            var MapperResponse = _mapper.Map<ServiceResponseDto<string>>(response);
            if (!response.Success)
            {
                return BadRequest(MapperResponse);
            }
            return Ok(MapperResponse);
        }
        [HttpGet("IsAdmin")]
        public async Task<ActionResult<ServiceResponse<bool>>> IsAdmin()
        {
            var response = await _authRepo.IsAdmin(Request.Headers["x-access-token"].ToString());
            var MapperResponse = _mapper.Map<ServiceResponseDto<bool>>(response);
            if (response.StatusCode == 401) return Unauthorized(MapperResponse);
            if (!response.Success) return BadRequest(MapperResponse);
            return Ok(MapperResponse);
        }



    }
}