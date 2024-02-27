using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace DBProcessing
{
    /// <summary>
    /// Write error messages to the text file
    /// </summary>
    public class ErrorInfo
    {
        private string _logFile;

        public ErrorInfo(string logFile)
        {
            _logFile = logFile;
        }

        public void WriteException(Exception exception, [CallerMemberName] string callerName = "")
        {
            WriteMessage(exception.Message, callerName);            
        }

        public void WriteMessage(string message, [CallerMemberName] string callerName = "")
        {
            using (StreamWriter writer = new StreamWriter(_logFile, true))
            {
                string logMessage = $"Error {DateTime.Now:dd-MMM-yy hh:mm:ss} from '{callerName}' method.{Environment.NewLine}{message}";
                writer.WriteLine(logMessage);
            }            
        }
    }
}
