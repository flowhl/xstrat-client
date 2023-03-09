using System.IO;
using System;

public static class Logger
{
    private static readonly object lockObject = new object();
    private static readonly string LogFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/xstrat/logs.txt";
    private static readonly string LogFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/xstrat";

    public static void Log(string message)
    {
        lock (lockObject)
        {
            if (!Directory.Exists(LogFolder))
            {
                Directory.CreateDirectory(LogFolder);
            }
            if (!File.Exists(LogFile))
            {
                File.Create(LogFile);
            }
            using (TextWriter tw = new StreamWriter(LogFile, true))
            {
                tw.WriteLine(DateTime.Now.ToString() + " | " + message);
            }
        }
    }
}