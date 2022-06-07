using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xstrat.Core
{
    public static class Logger
    {
        static readonly string LogFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/xstrat/logs.txt";
        static readonly string LogFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/xstrat";
        public static void Log(string Message)
        {
            if (!Directory.Exists(LogFolder))
            {
                Directory.CreateDirectory(LogFolder);
            }
            if (!File.Exists(LogFile))
            {
                File.Create(LogFile);
            }
            TextWriter tw = new StreamWriter(LogFile, true);
            tw.WriteLine(DateTime.Now.ToString() + " | " + Message);
            tw.Close();
        }
    }
}
