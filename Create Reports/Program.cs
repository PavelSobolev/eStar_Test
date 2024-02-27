using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

// app.config contains connection string and paths to the files of reports

namespace Create_Reports
{
    internal class Program
    {
        private const double _price = 200;
        private const double _percent = 25;

        static void Main()
        {
            Console.WriteLine("Creating reports as per Step 2");

            bool res1 = CreateReport1(fileName: ConfigurationManager.AppSettings["report_1"]);
            bool res2 = CreateReport2(fileName: ConfigurationManager.AppSettings["report_2"]);

            Console.WriteLine("Press any key to finish " + (res1 || res2 ? "and open reports" : "."));
            
            Console.ReadKey();


            if (res1 && File.Exists(ConfigurationManager.AppSettings["report_1"])) 
            {
                Process.Start(ConfigurationManager.AppSettings["report_1"]);
            }

            if (res2 && File.Exists(ConfigurationManager.AppSettings["report_2"]))
            {
                Process.Start(ConfigurationManager.AppSettings["report_2"]);
            }
        }

        private static bool CreateReport1(string fileName)
        {
            try
            {
                Console.WriteLine("Creating reports as per Step 2.1");
                SaveToCSVReport(GetProductIformation(), fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }

            Console.WriteLine("Reports for Step 2.1 is ready.");
            return true;
        }

        private static bool CreateReport2(string fileName)
        {
            try
            {
                Console.WriteLine("Creating reports as per Step 2.2");
                SaveToCSVReport(GetGroupByStyleCode(_price, _percent), fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }

            Console.WriteLine("Reports for Step 2.2 is ready.");
            return true;
        }

        /// <summary>
        /// Builds report for Step 2.1
        /// </summary>
        private static DataTable GetProductIformation()
        {            
            using(var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["eStarTest"]?.ToString()))
            {            
                conn.Open();
                var cmd = new SqlCommand(Properties.Resources.Report_1, conn);
                var adapter = new SqlDataAdapter(cmd);
                var resultTable = new DataTable();

                adapter.Fill(resultTable);
                conn.Close();

                return resultTable;
            }            
        }

        /// <summary>
        /// Builds report for Step 2.2
        /// </summary>
        private static DataTable GetGroupByStyleCode(double price, double percent)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["eStarTest"]?.ToString()))
            {
                conn.Open();

                // text of the query is in the project's resources
                var cmd = new SqlCommand(Properties.Resources.Report_2, conn);

                cmd.Parameters.Add("@P0", SqlDbType.Float).Value = price;
                cmd.Parameters.Add("@P1", SqlDbType.Float).Value = percent <= 100 ? percent : 0;


                var adapter = new SqlDataAdapter(cmd);
                var resultTable = new DataTable();

                adapter.Fill(resultTable);

                conn.Close();

                return resultTable;
            }
        }

        private static void SaveToCSVReport(DataTable dataTable, string fileName)
        {
            using(StreamWriter sw = new StreamWriter(fileName, false)) 
            {
                sw.WriteLine(GetCsvHeader(dataTable));
                dataTable.Rows
                    .Cast<DataRow>()
                    .ToList()
                    .ForEach(row => sw.WriteLine(GetCsvDataRow(row)));
            }
        }

        private static string GetCsvHeader(DataTable dataTable) 
        {
            return dataTable.Columns
                .Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .Aggregate("", (a, b) => $"{a}{b},");
        }

        private static string GetCsvDataRow(DataRow dataRow)
        {
            return dataRow.ItemArray
                .ToList()
                .Aggregate("", (a, b) =>
                {
                    return double.TryParse(b.ToString(), out _) ? $"{a}{b:f2}," : $"{a}\"{b}\",";
                });
        }
    }
}
