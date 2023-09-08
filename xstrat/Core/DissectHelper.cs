using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xstrat.Dissect;

namespace xstrat.Core
{
    public static class DissectHelper
    {
        public static Dissect.MatchReplay GetReplay(string path)
        {
            try
            {

            if (path.IsNullOrEmpty()) return null;
            if (!Directory.Exists(path)) return null;

            string exe = Path.Combine(Globals.XStratInstallPath, "External/r6-dissect.exe");
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "r6-dissect.exe",
                Arguments = $" -x stdout \"{path}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = Path.Combine(Globals.XStratInstallPath, "External"),
                CreateNoWindow = true
            };
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            MatchReplay matchReplay = JsonConvert.DeserializeObject<MatchReplay>(output);
            return matchReplay;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message + ex.StackTrace);
                return null;
            }
        }
    }
}
