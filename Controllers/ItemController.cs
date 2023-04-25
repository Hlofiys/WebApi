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
        public async Task<ActionResult<ServiceResponse<ItemById>>> GetById(int id)
        {
            var response = await _itemService.GetById(id);
            var MapperResponse = _mapper.Map<ServiceResponseDto<ItemById>>(response);
            if (response.Data == null && response.Success == false) return NotFound(MapperResponse);
            return Ok(MapperResponse);
        }
        [HttpPost("Add")]
        public async Task<ActionResult<ServiceResponse<Item>>> Add(ItemAddDto itemInfo)
        {
            string[] keys = new string[] { "Material", "Weigth", "Height", "Depth", "Diameter", "Width", "Length" };
            if (itemInfo.Sizes is null || !itemInfo.Sizes.Keys.ToArray().SequenceEqual(keys)) return BadRequest();
            Item item = new Item { Name = itemInfo.Name, Description = itemInfo.Description, Sizes = itemInfo.Sizes, Icon = itemInfo.Icon, Price = itemInfo.Price };
            string token = Request.Headers["x-access-token"].ToString();
            if (token is null) return BadRequest();
            var response = await _itemService.Add(item, token);
            if (response.StatusCode == 401) return Unauthorized(response);
            if (!response.Success) return BadRequest();
            return Ok(response);
        }
        [HttpPost("Update")]
        public async Task<ActionResult<ServiceResponse<Item>>> Update(ItemUpdateDto itemInfo)
        {
            if (itemInfo.Id == null)
            {
                return BadRequest();
            }
            string token = Request.Headers["x-access-token"].ToString();
            if (token is null) return BadRequest();
            var response = await _itemService.Update(itemInfo, token);
            if (response.StatusCode == 401) return Unauthorized(response);
            if (!response.Success) return BadRequest();
            return Ok(response);
        }
    }
}