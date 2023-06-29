using System.Linq;
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
        public ActionResult<ServiceResponse<List<Item>>> GetAll()
        {
            var response = _itemService.GetAll();
            var MapperResponse = _mapper.Map<ServiceResponseDto<List<Item>>>(response);
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
            var preFlightResponse = new ServiceResponse<string>();
            string[] keys = new string[] { "Depth", "Diameter", "Height", "Length", "Material", "Weight", "Width" };
            if (itemInfo.Sizes is null)
            {
                preFlightResponse.Success = false;
                preFlightResponse.Message = "Sizes are null";
                return BadRequest(preFlightResponse);
            }
            itemInfo.Sizes = itemInfo.Sizes.OrderBy(obj => obj.Key).ToDictionary(obj => obj.Key, obj => obj.Value);
            if (!itemInfo.Sizes.Keys.ToArray().SequenceEqual(keys))
            {
                preFlightResponse.Success = false;
                preFlightResponse.Message = "Sizes are not mach";
                return BadRequest(preFlightResponse);
            }
            Item item = new()
            {
                Name = itemInfo.Name,
                Description = itemInfo.Description,
                Sizes = itemInfo.Sizes,
                Icon = itemInfo.Icon,
                Price = itemInfo.Price,
                IsAKit = itemInfo.IsAKit,
                ParentId = itemInfo.ParentId,
                Type = itemInfo.Type,
            };
            string token = Request.Headers["x-access-token"].ToString();
            if (token is null)
            {
                preFlightResponse.Success = false;
                preFlightResponse.Message = "Token is null";
                return BadRequest(preFlightResponse);
            }
            var response = await _itemService.Add(item, token);
            if (response.StatusCode == 401) return Unauthorized(response);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
        [HttpPost("Update")]
        public async Task<ActionResult<ServiceResponse<string>>> Update(ItemUpdateDto itemInfo)
        {
            var preFlightResponse = new ServiceResponse<string>();
            if (itemInfo.Id == null)
            {
                preFlightResponse.Success = false;
                preFlightResponse.Message = "Id is null";
                return BadRequest(preFlightResponse);
            }
            string token = Request.Headers["x-access-token"].ToString();
            if (token is null)
            {
                preFlightResponse.Success = false;
                preFlightResponse.Message = "Token is null";
                return BadRequest(preFlightResponse);
            }
            var response = await _itemService.Update(itemInfo, token);
            if (response.StatusCode == 401) return Unauthorized(response);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
        [HttpGet("GetAllCombinations")]
        public async Task<ActionResult<ServiceResponse<List<ItemGetAllCombinations>>>> GetAllCombinations(int id)
        {
            var response = await _itemService.GetAllCombinations(id);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
    }
}