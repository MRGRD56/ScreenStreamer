using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using ScreenStreamer.Extensions;
using ScreenStreamer.Services;

namespace ScreenStreamer.Hubs
{
    public class ScreenStreamHub : Hub
    {
        private readonly ScreenshotProviderService _screenshotProviderService;
        private readonly IScreenshotService _screenshotService;
        private readonly IConfiguration _configuration;

        public ScreenStreamHub(
            ScreenshotProviderService screenshotProviderService, 
            IScreenshotService screenshotService, 
            IConfiguration configuration)
        {
            _screenshotProviderService = screenshotProviderService;
            _screenshotService = screenshotService;
            _configuration = configuration;
        }

        private async IAsyncEnumerable<string> GetScreenshots([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var screenshot = _screenshotService.GetScreenshot(true);
                await using var screenshotStream = new MemoryStream();
                screenshot.SaveAsJpeg(screenshotStream, _configuration.GetValue<int>("Streaming:Quality"));
                var screenshotBytes = screenshotStream.ToArray();
                var base64 = Convert.ToBase64String(screenshotBytes);
                var base64Source = $"data:image/jpeg;base64, {base64}";
                yield return base64Source;

                await Task.Delay(_screenshotProviderService.DelayTime, cancellationToken);
            }
        }

        public ChannelReader<string> GetScreenshotsReader(CancellationToken cancellationToken)
        {
            var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(_configuration.GetValue<int>("Streaming:BufferSize")));

            _ = WriteItemsAsync(channel.Writer, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteItemsAsync(ChannelWriter<string> writer, CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
            {
                await foreach (var screenshot in GetScreenshots(cancellationToken))
                {
                    await writer.WriteAsync(screenshot, cancellationToken);
                }
            }
            catch (Exception exception)
            {
                localException = exception;
            }
            finally
            {
                writer.Complete(localException);
            }
        }
    }
}