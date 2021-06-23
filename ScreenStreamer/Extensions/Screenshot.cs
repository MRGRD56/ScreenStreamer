using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScreenStreamer.Extensions
{
    public static class Screenshot
    {
        public static Bitmap CaptureScreen(bool captureCursor)
        {
            var primaryScreen = Screen.PrimaryScreen;
            var bitmap = new Bitmap(primaryScreen.Bounds.Width, primaryScreen.Bounds.Height, PixelFormat.Format24bppRgb);
            using var g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(0, 0, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);

            if (captureCursor)
            {
                User32.CURSORINFO cursorInfo;
                cursorInfo.cbSize = Marshal.SizeOf(typeof(User32.CURSORINFO));

                if (User32.GetCursorInfo(out cursorInfo))
                {
                    // if the cursor is showing draw it on the screen shot
                    if (cursorInfo.flags == User32.CURSOR_SHOWING)
                    {
                        // we need to get hotspot so we can draw the cursor in the correct position
                        var iconPointer = User32.CopyIcon(cursorInfo.hCursor);
                        User32.ICONINFO iconInfo;
                        int iconX, iconY;

                        if (User32.GetIconInfo(iconPointer, out iconInfo))
                        {
                            // calculate the correct position of the cursor
                            iconX = cursorInfo.ptScreenPos.x - iconInfo.xHotspot;
                            iconY = cursorInfo.ptScreenPos.y - iconInfo.yHotspot;

                            // draw the cursor icon on top of the captured screen image
                            User32.DrawIcon(g.GetHdc(), iconX, iconY, cursorInfo.hCursor);

                            // release the handle created by call to g.GetHdc()
                            g.ReleaseHdc();
                        }
                    }
                }
            }

            return bitmap;
        }
    }
}