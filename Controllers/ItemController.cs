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
          return Ok(response);
        }
    }
}