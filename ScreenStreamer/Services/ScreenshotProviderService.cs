using System;
using System.Drawing;
using Microsoft.Extensions.Configuration;

namespace ScreenStreamer.Services
{
    public class ScreenshotProviderService
    {
        private readonly IScreenshotService _screenshotService;

        private readonly IConfiguration _configuration;

        // public Bitmap Screenshot { get; private set; }
        public Bitmap Screenshot => _screenshotService.GetScreenshot(true);

        public static int Fps { get; private set; }
        public TimeSpan DelayTime { get; private set; }

        public ScreenshotProviderService(IScreenshotService screenshotService, IConfiguration configuration)
        {
            _screenshotService = screenshotService;
            _configuration = configuration;
            Fps = _configuration.GetValue<int>("Streaming:Fps");
            DelayTime = TimeSpan.FromMilliseconds(1000D / Fps);
            //StartUpdate();
        }

        // private async void StartUpdate()
        // {
        //     while (true)
        //     {
        //         Screenshot = _screenshotService.GetScreenshot(true);
        //         // Debug.WriteLine($"{DateTime.Now:yyyy-MM-ddTHH:mm:ss} Скриншот обновлён");
        //         await Task.Delay(DelayTime);
        //     }
        // }
    }
}