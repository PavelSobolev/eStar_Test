using DataModel.Attributes;

namespace DataModel
{
    [TableName("Products")]
    public class Product
    {
        [Order(0)]
        public string Name { get; set; } = string.Empty;
        [PK]
        [Order(1)]
        public string SKU { get; set; } = string.Empty;
        [Order(2)]
        public string Description { get; set; } = string.Empty;
        [Order(3)]
        public string Brand { get; set; } = string.Empty;
        [Order(4)]
        public string StyleCode { get; set; } = string.Empty;

    }
}
