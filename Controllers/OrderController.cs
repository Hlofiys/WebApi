using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using WebApi.Dtos.Cart;
using WebApi.Dtos.Order;
using WebApi.Dtos.User;
using WebApi.Services.OrderService;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }
        [HttpPost("Create")]
        public async Task<ActionResult<ServiceResponse<string>>> Register(OrderCreateDto request)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            bool Shipping = false;
            try
            {
                Shipping = Convert.ToBoolean(request.Shipping);
            }
            catch
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Shipping field should be either true or false!";
                return BadRequest(serviceResponse);
            }
            var response = await _orderService.Create(
                new Order { FIO = request.FIO, Address = request.Address, Contact = request.Contact, PhoneNubmer = request.PhoneNumber, Shipping = Shipping , ZipCode = request.ZipCode, City = request.City, FullDate = request.FullDate }, Request);
            var MapperResponse = _mapper.Map<ServiceResponseDto<string>>(response);
            if (!response.Success)
            {
                return BadRequest(MapperResponse);
            }
            return Ok(MapperResponse);
        }
        [HttpGet("All")]
        public async Task<ActionResult<ServiceResponse<List<OrderAllDto>>>> All(){
            var serviceResponse = await _orderService.All(Request);
            var MapperResponse = _mapper.Map<ServiceResponseDto<List<OrderAllDto>>>(serviceResponse);
            if (!serviceResponse.Success)
            {
                return BadRequest(MapperResponse);
            }
            return Ok(MapperResponse);
        }

    }
}
