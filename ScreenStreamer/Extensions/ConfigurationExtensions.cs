using System;
using System.Drawing;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace ScreenStreamer.Extensions
{
    public static class ConfigurationExtensions
    {
        public static Size GetSizeValue(this IConfiguration configuration, string key)
        {
            // Possible values:
            // "1920x1080", "720p", "480p 3:4", "1080p16:9"
            
            var resolutionString = configuration[key];

            var fullResolutionMatch = Regex.Match(resolutionString, @"^(\d+)[x|х|\*|\s|×](\d+)p?$");
            if (fullResolutionMatch.Success)
            {
                return new Size(int.Parse(fullResolutionMatch.Groups[1].Value), int.Parse(fullResolutionMatch.Groups[2].Value));
            }

            var heightMatch = Regex.Match(resolutionString, @"^(\d+)[p|р]\s*((\d+)[:|\/](\d+))?$");
            if (heightMatch.Success)
            {
                var height = int.Parse(heightMatch.Groups[1].Value);
                var aspectRatio = heightMatch.Groups[2].Success
                    ? (double) int.Parse(heightMatch.Groups[3].Value) / int.Parse(heightMatch.Groups[4].Value)
                    : 16D / 9; // default aspect ratio is 16:9
                var width = (int) Math.Round(height * aspectRatio);
                return new Size(width, height);
            }

            throw new Exception($"Unable to parse a size from value \"{resolutionString}\"");
        }
    }
}