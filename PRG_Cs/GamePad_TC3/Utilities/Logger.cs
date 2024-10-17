using System;
using System.IO;

namespace TwinCATUsbControllerApp.Utilities
{
    public static class Logger
    {
        private const string LogFileName = "application.log";

        public static void Log(string message)
        {
            string logMessage = $"{DateTime.Now}: {message}";
            Console.WriteLine(logMessage);

            try
            {
                using (StreamWriter writer = File.AppendText(LogFileName))
                {
                    writer.WriteLine(logMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }
}