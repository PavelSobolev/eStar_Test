using System;

namespace DataModel
{
    /// <summary>
    /// Defines a property as a primary key
    /// </summary>
    public class PKAttribute : Attribute
    {
        public bool IsPrimaryKey { get; } = false;
        public PKAttribute()
        {
            IsPrimaryKey = true;
        }
    }
}