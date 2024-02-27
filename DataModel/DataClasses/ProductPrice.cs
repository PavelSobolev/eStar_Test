using DataModel.Attributes;

namespace DataModel
{
    [TableName("Pricing")]
    public class ProductPrice 
    {

        [PK]
        [Order(0)]
        public string SKU  { get; set; } = string.Empty;
        [Order(1)]
        public double Price { get; set; }
    }
}
