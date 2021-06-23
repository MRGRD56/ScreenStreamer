using System;
using System.Drawing;

namespace ScreenStreamer.Services
{
    public class ScreenshotProviderService
    {
        private readonly IScreenshotService _screenshotService;
        // public Bitmap Screenshot { get; private set; }
        public Bitmap Screenshot => _screenshotService.GetScreenshot(true);

        public const int Fps = 30;
        public readonly TimeSpan DelayTime = TimeSpan.FromMilliseconds(1000D / Fps);
        
        public ScreenshotProviderService(IScreenshotService screenshotService)
        {
            _screenshotService = screenshotService;
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