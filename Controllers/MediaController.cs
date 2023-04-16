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
            if (OperatingSystem.IsWindows())
            {
                string FilePath = $"images/{CategoryName}/{FileName}";
                FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FilePath);
                Console.WriteLine(FilePath);
                if (System.IO.File.Exists(FilePath))
                    return PhysicalFile(FilePath, "image/png");
                else
                    return NotFound();
            }
            else if (OperatingSystem.IsLinux())
            {
                string FilePath = $"images/{CategoryName}/{FileName}";
                FilePath = Path.Combine("/home/ubuntu/Projects/WebApi/bin/Debug/net7.0/", FilePath);
                if (System.IO.File.Exists(FilePath))
                    return PhysicalFile(FilePath, "image/png");
                else
                    return NotFound();
            }
            else
            {
                return NotFound();
            }
            
        }
    }
}
