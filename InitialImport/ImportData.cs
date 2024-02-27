using DataModel;
using DBProcessing;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace InitialImport
{
    // this part contains methods for importing data (using updating or inserting commands)
    internal partial class Program 
    {
        private static void ExecuteImport()
        {
            Console.WriteLine("Importing data ... \n");

            // all file paths are in app.config

            bool res = ImportProducts(fileName: ConfigurationManager.AppSettings["products"]);

            if (res)
            {
                Console.WriteLine("Products: success.\n");
                res = ImportPricing(fileName: ConfigurationManager.AppSettings["Pricing"]);
            }

            if (res)
            {
                Console.WriteLine("Pricing: success.\n");
                res = ImportStock(fileName: ConfigurationManager.AppSettings["Stock"]);
            }

            if (res)
            {
                Console.WriteLine("Stock: success.\n");
            }
        }

        private static void ExecuteUpdating()
        {
            Console.WriteLine("Updating data ... ");

            // all file paths are in app.config

            bool res = UpdatePricing(fileName: ConfigurationManager.AppSettings["price_update"]);

            if (res)
            {
                Console.WriteLine("Pricing: success.\n");
                res = UpdateStock(fileName: ConfigurationManager.AppSettings["stock_update"]);
            }

            if (res)
            {
                Console.WriteLine("Stock: success.\n");              
            }
        }

        private static bool ImportStock(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File ({fileName}) has not been found. Import of Stock file was canceled.\n");
                return false;
            }

            var stockImporter = new CsvImporter<ProductStock>(err);

            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["eStarTest"]?.ToString()))
                    stockImporter.InsertFromCsv(fileName, cn);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Stock import was canceled due to errors.\n{ex.Message}\n");
                return false;
            }

            return true;
        }

        private static bool ImportPricing(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File ({fileName}) has not been found. Import of Pricing file was canceled.\n");
                return false;
            }

            var priceImporter = new CsvImporter<ProductPrice>(err);

            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["eStarTest"]?.ToString()))
                    priceImporter.InsertFromCsv(fileName, cn);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pricing import was canceled due to errors.\n{ex.Message}\n");
                return false;
            }

            return true;
        }

        private static bool ImportProducts(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File ({fileName}) has not been found. Products import was canceled.\n");
                return false;
            }

            var productImporter = new CsvImporter<Product>(err);


            productImporter.ValidationActions.Add(product => // check format of SKU before inserting into db
            {
                string skuPattern = @"^[A-Z]{2}-[A-Z0-9]{4}(-[A-Z0-9]{1,2})?$";
                bool res = Regex.IsMatch(product.SKU, skuPattern);
                return (res, res ? "" : $"Incorrect value of SKU '{product.SKU}' was detected. Line is ignored.");
            });


            productImporter.TransformActions.Add(product => // set value of StyleCode es per the requirement Step-1/4a
            {
                var skuParts = product.SKU.Split('-');
                if (skuParts.Length >= 2)
                {
                    product.StyleCode = $"{skuParts[0]}-{skuParts[1]}";
                }
            });

            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["eStarTest"]?.ToString()))
                    productImporter.InsertFromCsv(fileName, cn);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Products import was canceled due to errors.\n{ex.Message}\n");
                return false;
            }

            return true;
        }

        private static bool UpdatePricing(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File ({fileName}) has not been found. Updating of Pricing was canceled.\n");
                return false;
            }

            var priceImporter = new CsvImporter<ProductPrice>(err);

            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["eStarTest"]?.ToString()))
                    priceImporter.UpdateFromCsv(fileName, cn);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pricing update was canceled due to errors.\n{ex.Message}\n");
                return false;
            }

            return true;
        }

        private static bool UpdateStock(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File ({fileName}) has not been found. Updating of Stock was canceled.\n");
                return false;
            }

            var stockImporter = new CsvImporter<ProductStock>(err);

            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["eStarTest"]?.ToString()))
                    stockImporter.UpdateFromCsv(fileName, cn);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Stock update was canceled due to errors.\n{ex.Message}\n");
                return false;
            }

            return true;
        }
    }
}
