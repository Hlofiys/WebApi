using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Reflection;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("media")]
    public class MediaController : Controller
    {
        private readonly IMapper _mapper;

        public MediaController(IMapper mapper)
        {
            _mapper = mapper;
        }
        [HttpGet("get/{CategoryName}")]
        public ActionResult GetFile(string CategoryName)
        {
            string FileName = Request.Query["name"]!;
            string FilePath = $"images/{CategoryName}/{FileName}";
            FilePath = Path.Combine(Assembly.GetEntryAssembly()!.Location, FilePath);
            if (System.IO.File.Exists(FilePath))
                return PhysicalFile(FilePath, "image/png");
            else
                return NotFound();
        }
    }
}
