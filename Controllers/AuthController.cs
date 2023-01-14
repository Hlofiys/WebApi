using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Dtos.User;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository authRepo, IMapper mapper)
        {
            _authRepo = authRepo;
            _mapper = mapper;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<string>>> Register(UserRegisterDto request)
        {
            var response = await _authRepo.Register(
                new User { Username = request.Username }, request.Password, Response
            );
            var MapperResponse = _mapper.Map<ServiceResponseDto<string>>(response);
            if(!response.Success)
            {
                return BadRequest(MapperResponse);
            }
            return Ok(MapperResponse);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDto request)
        {
            var response = await _authRepo.Login(request.Username, request.Password, Response);
            var MapperResponse = _mapper.Map<ServiceResponseDto<string>>(response);
            if(!response.Success)
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
            if(!response.Success)
            {
                return BadRequest(MapperResponse);
            }
            return Ok(MapperResponse);
        }

        [HttpGet("checkToken")]
        public ActionResult<ServiceResponse<string>> checkToken()
        {
            var response =   _authRepo.CheckToken(Request);
            var MapperResponse = _mapper.Map<ServiceResponseDto<String>>(response);
            if(response.StatusCode == 401) return Unauthorized(MapperResponse);
            if(!response.Success)
            {
                return BadRequest(MapperResponse);
            }
            return Ok(MapperResponse);
        }
        [HttpGet("Refresh")]
        public async Task<ActionResult<ServiceResponse<string>>> Refresh()
        {
            var response =  await _authRepo.Refresh(Request, Response);
            var MapperResponse = _mapper.Map<ServiceResponseDto<String>>(response);
            if(response.StatusCode == 401) return Unauthorized(MapperResponse);
            if(response.StatusCode == 403) return Unauthorized(MapperResponse);
            if(!response.Success)
            {
                return BadRequest(MapperResponse);
            }
            return Ok(MapperResponse);
        }
    }
}