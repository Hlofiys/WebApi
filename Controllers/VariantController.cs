using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VariantController : ControllerBase
    {
        private readonly IVariantService _variantService;
        private readonly IMapper _mapper;

        public VariantController(IVariantService variantService, IMapper mapper)
        {
            _variantService = variantService;
            _mapper = mapper;
        }
        [HttpPost("Add")]
        public async Task<ActionResult<ServiceResponse<Variant>>> Add(VariantAddDto variantInfo)
        {
            Variant variant = new Variant { Name = variantInfo.Name, Description = variantInfo.Description, Icon = variantInfo.Icon, Price = variantInfo.Price, ItemId = variantInfo.ItemId };
            string token = Request.Headers["x-access-token"].ToString();
            if (token is null) return BadRequest();
            var response = await _variantService.Add(variant, token);
            if (response.StatusCode == 401) return Unauthorized(response);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("Update")]
        public async Task<ActionResult<ServiceResponse<Variant>>> Update(VariantUpdateDto variantInfo)
        {
            if (variantInfo.ItemId == null || variantInfo.VariantId == null)
            {
                return BadRequest();
            }
            string token = Request.Headers["x-access-token"].ToString();
            if (token is null) return BadRequest();
            var response = await _variantService.Update(variantInfo, token);
            if (response.StatusCode == 401) return Unauthorized(response);
            if (!response.Success) return BadRequest();
            return Ok(response);
        }
    }
}