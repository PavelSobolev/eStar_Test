using System;

namespace DataModel
{
    /// <summary>
    /// Custom attribute to map data class to the table name of a database
    /// </summary>
    public class TableNameAttribute : Attribute
    {
        public string TableName { get; }

        public TableNameAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
