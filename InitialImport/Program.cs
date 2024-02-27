using DBProcessing;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

//Please configure connection strings as per description in App.config

namespace InitialImport
{
    internal static partial class Program
    {
        // very simple error logger tool
        private static readonly ErrorInfo err = new ErrorInfo(ConfigurationManager.AppSettings["error"]);

        static void Main()
        {                     
            Console.Write("Enter number of needed operation:\n1 to execute initial import, 2 to execute update ->");            

            switch(Console.ReadLine())            
            {
                case "1":
                    ExecuteCreationScripts();
                    ExecuteImport();
                    break;
                    case "2":
                    ExecuteUpdating();
                    break;
                default:
                    Console.WriteLine("Program terminated as no appropriate choice has been provided");
                    break;
            }


            Console.WriteLine("The chosen task completed. Press any key to exit.");

            Console.ReadKey();

            if (File.Exists(ConfigurationManager.AppSettings["error"]))
            {
                Process.Start(ConfigurationManager.AppSettings["error"]);
            }

        }
    }    
}
