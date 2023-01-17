using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos.Cart;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;

        public CartController(ICartService cartService, IMapper mapper)
        {
            _cartService = cartService;
            _mapper = mapper;
        }
        [HttpPost("Add")]
         public async Task<ActionResult<ServiceResponse<string>>> Add(CartAddDto request)
        {
            var Id = request.Id;
            var Amount = request.Amount;
            var response = await _cartService.Add(Id, Amount, Request);
            if(response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}