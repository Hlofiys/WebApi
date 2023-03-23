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
            var Variants = request.Variants;
            var response = await _cartService.Add((int)Id!, (int)Amount!, Variants!, Request);
            if(response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [HttpPost("All")]
        public async Task<ActionResult<ServiceResponse<CartAllDto>>> All()
        {
            var response = await _cartService.All(Request);
            if (response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<ServiceResponse<string>>> Delete(CartDeleteDto request)
        {
            var Id = request.Id;
            if(Id == null)
            {
                return BadRequest();
            }
            var Variants = request.Variants;
            var response = await _cartService.Delete((int)Id!, Variants!, Request);
            if (response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [HttpPost("Update")]
        public async Task<ActionResult<ServiceResponse<CartAllDto>>> Update(CartUpdateDto request)
        {
            var Id = request.Id;
            if (Id == null)
            {
                return BadRequest();
            }
            var Variants = request.Variants;
            var Amount = request.Amount;
            var response = await _cartService.Update((int)Id!, Variants, Amount, Request);
            if (response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [HttpPost("Count")]
        public async Task<ActionResult<ServiceResponse<int>>> Count()
        {
            var req = Request;
            if (req == null)
            {
                return BadRequest();
            }
            var response = await _cartService.Count(req);
            if (response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [HttpPost("PriceGet")]
        public async Task<ActionResult<ServiceResponse<int>>> PriceGet(CartAddDto request)
        {
            var Id = request.Id;
            var Amount = request.Amount;
            var Variants = request.Variants;
            var response = await _cartService.PriceGet((int)Id!, (int)Amount!, Variants!);
            if (response.Success == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}