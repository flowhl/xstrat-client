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
                    FileName = exe,
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
                Notify.sendError(ex.Message + ex.StackTrace);
                return null;
            }
        }

        public static void ExportReplay(string path, string exportFile)
        {
            try
            {
                if(path.IsNullOrEmpty())
                {
                    Notify.sendError("Error: replay path cannot be empty");
                    return;
                }
                if(exportFile.IsNullOrEmpty())
                {
                    Notify.sendError("Error: export path cannot be empty");
                    return;
                }

                string exe = Path.Combine(Globals.XStratInstallPath, "External/r6-dissect.exe");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = $" \"{path}\" -x {Path.GetFileName(exportFile)}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(exportFile),
                    CreateNoWindow = true
                };
                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                if(process.ExitCode != 0)
                {
                    Notify.sendError($"Error exporting file: {process.StandardOutput}");
                    return;
                }
                Notify.sendSuccess($"Exported into {exportFile}");

            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message + ex.StackTrace);
                return;
            }
        }

    }
}
