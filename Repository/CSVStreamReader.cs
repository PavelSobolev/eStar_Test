using DataModel.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DBProcessing
{
    /// <summary>
    /// Extension of StreamReader class to convert a line being red to custom data type
    /// </summary>
    public static class CsvStreamReader
    {
        /// <summary>
        /// Reads line of comma separated values and constructs new object (of type T) and assigns these values 
        /// to the corresponding properties of the object
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="stream">stream reader</param>
        /// <returns></returns>
        public static T ReadCSVLineOfType<T>(this StreamReader stream) where T : class, new()
        {
            var lineValues = ParseCsv(stream.ReadLine());

            var newT = new T();

            AssingValues<T>(lineValues, newT);
            
            return newT;
        }


        #region Helper methods
        private static List<string> ParseCsv(string input)
        {
            var result = new List<string>();
            var inQuotes = false;
            var currentPart = new StringBuilder();


            foreach (char c in input)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes; // Toggle inQuotes flag when current char is "
                }
                else if (c == ',' && !inQuotes)
                {
                    // If not in " and comma is encountered, add the current part to the result
                    result.Add(currentPart.ToString().Trim());
                    currentPart = new StringBuilder(); // Reset currentPart
                }
                else
                {                    
                    currentPart.Append(c); // Append the character to currentPart
                }
            }

            // Add the last part to the result
            result.Add(currentPart.ToString().Trim());

            return result;
        }

        private static void AssingValues<T>(IEnumerable<string> values, T objectT) where T : class
        {
            // takes value (of values array) with index derived from Order attribute
            // of each property of the object T  (if such attribute is set) otherwise default value will be left 
            // for the property
            typeof(T)
                .GetProperties()
                .ToList()
                .ForEach(prop =>
                {
                    var propOrder = prop.GetCustomAttribute<OrderAttribute>();


                    if (propOrder != null && propOrder.Order < values.Count())
                    {
                        SetValue(prop, objectT, values.ElementAt(propOrder.Order));
                    }
                });
        }

        private static void SetValue<T>(PropertyInfo prop, T objectT, string value) where T : class
        {

            if (IsDecimalType(prop.PropertyType))
            {
                double numericValue;
                var res = double.TryParse(value, out numericValue);
                prop.SetValue(objectT, res ? numericValue : default);
                return;
            }

            if (IsIntType(prop.PropertyType))
            {
                int numericValue;
                var res = int.TryParse(value, out numericValue);
                prop.SetValue(objectT, res ? numericValue : default);
                return;
            }

            prop.SetValue(objectT, value);
        }

        // Check if the type is integer type (int, long, short, ushort, uint, ulong)
        static bool IsIntType(Type type)
        {

            return type == typeof(int) ||
                   type == typeof(long) ||
                   type == typeof(short) ||
                   type == typeof(ushort) ||
                   type == typeof(uint) ||
                   type == typeof(ulong);
        }

        // Check if the type is decimal numeric (float, double, decimal)
        static bool IsDecimalType(Type type)
        {

            return type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(decimal);
        } 
        #endregion
    }
}
