using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using BitmapTools;
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
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var screenshotStopwatch = new Stopwatch();
                screenshotStopwatch.Start();
                
                var screenshot = _screenshotService.GetScreenshot(true);
                
                screenshotStopwatch.Stop();
                LogTime("Screenshot taken", screenshotStopwatch.Elapsed);
                
                await using var screenshotStream = new MemoryStream();
                var resolution = _configuration.GetSizeValue("Streaming:Resolution");
                if (screenshot.Size != resolution)
                {
                    screenshot = screenshot.Resize(resolution);
                }

                screenshot.SaveCompressed(screenshotStream, _configuration.GetValue<int>("Streaming:Quality"));
                var screenshotBytes = screenshotStream.ToArray();
                var base64 = Convert.ToBase64String(screenshotBytes);
                screenshot.Dispose();
                var base64Source = $"data:image/jpeg;base64, {base64}";
                yield return base64Source;

                stopwatch.Stop();
                LogTime("> Image sent", stopwatch.Elapsed);
                
                await Task.Delay(_screenshotProviderService.DelayTime, cancellationToken);
            }
        }

        private void LogTime(string action, TimeSpan time)
        {
            var elapsedSeconds = time.TotalSeconds;
            Debug.WriteLine($"{action} in {elapsedSeconds:N3} sec - " +
                            $"1/{Math.Round(1 / elapsedSeconds)} sec");
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