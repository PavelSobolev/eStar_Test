using DataModel.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DataModel
{
    [TableName("Stock")]
    public class ProductStock 
    {
        [PK]
        [Order(0)]
        public string SKU { get; set; } = string.Empty;
        [PK]
        [Order(1)]        
        public string Location { get; set; } = string.Empty;
        [Order(2)]
        public int Quantity { get; set; }
    }
}
