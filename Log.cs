using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace Benz.log
{
    public class Log


    {
        public void lodwrite(string message)
        {

            // Get the base directory of your application
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Specify the folder name where you want to create the log files
            string logFolderName = "Logs";

            // Combine the base directory and log folder name to create the full log folder path
            string logFolderPath = Path.Combine(baseDirectory, logFolderName);

            // Create the log folder if it doesn't exist
            Directory.CreateDirectory(logFolderPath);

            // Get the current date
            DateTime currentDate = DateTime.Now;

            // Format the date as a string (e.g., "2023-09-08")
            string logFileName = currentDate.ToString("yyyy-MM-dd") + ".txt";

            // Combine the log folder path and log file name to create the full log file path
            string logFilePath = Path.Combine(logFolderPath, logFileName);

            // Specify the log text for this hit
            string logText = message.ToString();

            // Append the log text to the log file with a line break
            File.AppendAllText(logFilePath, $"{currentDate}: {logText}" + Environment.NewLine);

            Console.WriteLine($"Log entry added to: {logFilePath}");

            return;
        }

    }
}