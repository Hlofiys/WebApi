using Microsoft.AspNetCore.Mvc;
using WebApi.Services.CartService;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IMapper _mapper;

        public ItemController(IItemService itemService, IMapper mapper)
        {
            _itemService = itemService;
            _mapper = mapper;
        }
        [HttpGet("GetAll")]
         public ActionResult<ServiceResponse<Item[]>> GetAll()
        {
            var response = _itemService.GetAll();
            var MapperResponse = _mapper.Map<ServiceResponseDto<Item[]>>(response);
            return Ok(MapperResponse);
        }
        [HttpGet("GetById")]
        public async Task<ActionResult<ServiceResponse<Item>>> GetById(int id)
        {
            var response = await _itemService.GetById(id);
            var MapperResponse = _mapper.Map<ServiceResponseDto<Item>>(response);
            if (response.Data == null && response.Success == false) return NotFound(MapperResponse);
            return Ok(MapperResponse);
        }
    }
}