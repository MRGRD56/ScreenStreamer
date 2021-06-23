using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ScreenStreamer.Extensions;

namespace ScreenStreamer.Services
{
    public class FullScreenScreenshotService : IScreenshotService
    {
        public Bitmap GetScreenshot(bool captureMouse)
        {
            // var primaryScreen = Screen.PrimaryScreen;
            // var screenshotBitmap = new Bitmap(primaryScreen.Bounds.Width, primaryScreen.Bounds.Height,
            //     PixelFormat.Format32bppArgb);
            // var screenshotGraphics = Graphics.FromImage(screenshotBitmap);
            // screenshotGraphics.CopyFromScreen(primaryScreen.Bounds.X, primaryScreen.Bounds.Y,
            //     0, 0, primaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            // return screenshotBitmap;
            return Screenshot.CaptureScreen(captureMouse);
        }
    }
}