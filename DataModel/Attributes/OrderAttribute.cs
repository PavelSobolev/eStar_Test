using System;

namespace DataModel.Attributes
{
    /// <summary>
    /// OrderAttribute sets the ordinal number of the CSV file column 
    /// that is used for the property of the corresponding class.
    /// </summary>
    public class OrderAttribute : Attribute
    {
        public int Order { get; set; }
        public OrderAttribute(int value)
        {
            Order = value;
        }
    }
}
