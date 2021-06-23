using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ScreenStreamer.Extensions
{
    public static class BitmapExtensions
    {
        private static ImageCodecInfo GetEncoder(ImageFormat format)  
        {  
            var codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        } 
        
        public static void SaveAsJpeg(this Bitmap bitmap, Stream stream, long quality = 100)
        {
            bitmap.Save(stream, GetEncoder(ImageFormat.Jpeg), new EncoderParameters
            {
                Param = new[]
                {
                    new EncoderParameter(Encoder.Quality, quality)
                }
            });
        }
    }
}