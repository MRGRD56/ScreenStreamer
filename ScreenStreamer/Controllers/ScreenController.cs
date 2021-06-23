using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScreenStreamer.Services;

namespace ScreenStreamer.Controllers
{
    [Route("api/[controller]")]
    public class ScreenController : ControllerBase
    {
        private readonly ScreenshotProviderService _screenshotProviderService;

        public ScreenController(ScreenshotProviderService screenshotProviderService)
        {
            _screenshotProviderService = screenshotProviderService;
        }
        
        // GET
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var screenshot = _screenshotProviderService.Screenshot;
            await using var screenshotStream = new MemoryStream();
            screenshot.Save(screenshotStream, ImageFormat.Jpeg);
            return File(screenshotStream.ToArray(), "image/jpeg", true);
        }
    }
}