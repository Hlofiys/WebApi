using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KitController : ControllerBase
    {
        private readonly IKitService _kitService;
        private readonly IMapper _mapper;

        public KitController(IKitService kitService, IMapper mapper)
        {
            _kitService = kitService;
            _mapper = mapper;
        }
        [HttpPost("Add")]
        public async Task<ActionResult<ServiceResponse<Kit>>> Add(KitAddDto kitInfo)
        {
            Kit kit = new Kit {Name = kitInfo.Name, Icon = kitInfo.Icon, Price = kitInfo.Price, ItemId = kitInfo.ItemId, Variants = kitInfo.Variants};
            string token = Request.Headers["x-access-token"].ToString();
            if(token is null) return BadRequest();
            var response = await _kitService.Add(kit, token);
            if(response.StatusCode == 401) return Unauthorized(response);
            if(!response.Success) return BadRequest(response);
            return Ok(response);
        }
    }
}