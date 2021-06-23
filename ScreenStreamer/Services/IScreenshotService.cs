using System.Drawing;

namespace ScreenStreamer.Services
{
    public interface IScreenshotService
    {
        Bitmap GetScreenshot(bool captureMouse);
    }
}