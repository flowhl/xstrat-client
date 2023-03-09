using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using Container = YoutubeExplode.Videos.Streams.Container;

namespace xstrat.Core
{
    public class YoutubeAPIHandler
    {
        public static string videoURL { get; set; }

        public static YoutubeClient youtubeClient { get; private set; }

        public static void Init()
        {
            youtubeClient = new YoutubeClient();
        }

        public static async Task DownloadVideoAsMP4Async(string path)
        {
            var youtube = new YoutubeClient();
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoURL);
            var stream = streamManifest.GetVideoStreams().Where(x => x.Container == Container.Mp4 && x.VideoQuality.Label == "720p").GetWithHighestBitrate();
            string dl = stream.Url;
            GetFramesFromVideo(dl, path);
        }

        private static void GetFramesFromVideo(string stream, string path)
        {
            // Use FFmpeg to extract the current frame at the specified time
            //1s per Minute of VOD
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = @"ffmpeg.exe",
                Arguments = $"-i {stream} -vf fps=1/10 export_%03d.jpg",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = path
            };
            Process process = Process.Start(startInfo);
            process.WaitForExit();
        }
    }

}
