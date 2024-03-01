using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using DataModel.Attributes;
using DataModel;

namespace DBProcessing.Tests
{
    [TestClass]
    public class CsvStreamReaderTests
    {
        public class SampleClass
        {
            [PK]
            [Order(0)]
            public int Id { get; set; }
            [Order(1)]
            public string Name { get; set; }
            [Order(2)]
            public double Price { get; set; }
        }

        [TestMethod]
        [DataRow(10,"somename", 1.23)]
        [DataRow(20, "\"somename,and something\"", 0)]
        public void ReadCSVLineOfType_ValidInput_ReturnsObjectWithCorrectValues(int id, string name, double price)
        {
            string csvLine = $"{id},{name},{price}";
            var stream = new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvLine)));
            
            var result = stream.ReadCSVLineOfType<SampleClass>();

            // Assert            
            Assert.AreEqual(id, result.Id);
            Assert.AreEqual(name.Trim('"'), result.Name);
            Assert.AreEqual(price, result.Price);
        }

        [TestMethod]
        public void ReadCSVLineOfType_InvalidInput_ReturnsDefaultObject()
        {            
            string csvLine = ",,"; // Missing values
            var stream = new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvLine)));
         
            var result = stream.ReadCSVLineOfType<SampleClass>();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Id);
            Assert.AreEqual(string.Empty, result.Name);
            Assert.AreEqual(0, result.Price); 
        }
    }
}
