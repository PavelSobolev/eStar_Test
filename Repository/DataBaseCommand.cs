using DataModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DBProcessing
{
    public sealed class DataBaseCommand<T> where T : class
    {
        private readonly PropertyInfo[] _classProperties;
        private readonly SqlConnection _connection;
        private readonly string _tableName;
        private readonly ErrorInfo _errorInfo;

        public DataBaseCommand(SqlConnection sqlConnection, ErrorInfo errorInfo)
        {
            _connection = sqlConnection;
            _errorInfo = errorInfo;

            _classProperties = typeof(T).GetProperties();
            TableNameAttribute tableNameAttrInfo = Attribute.GetCustomAttribute(typeof(T), typeof(TableNameAttribute))
                as TableNameAttribute;

            _tableName = tableNameAttrInfo != null ? tableNameAttrInfo.TableName : typeof(T).Name;
        }

        public void Insert(T newObject)
        {
            try
            {
                BuildInsertSqlCommand(newObject).ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //log error
                _errorInfo.WriteException(ex);
            }
        }

        public void Update(T newObject)
        {
            try
            {
                BuildUpdateSqlCommand(newObject).ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //log error
                _errorInfo.WriteException(ex);
            }
        }

        /// <summary>
        /// Update existing record or adds new one
        /// </summary>
        /// <param name="newObject">object representing data for update/insert command</param>
        public void Upsert(T newObject)
        {
            try
            {
                if ((int)BuildCountSqlCommand(newObject).ExecuteScalar() > 0)
                    BuildUpdateSqlCommand(newObject).ExecuteNonQuery();
                else
                    BuildInsertSqlCommand(newObject).ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                //log error
                _errorInfo.WriteException(ex);
            }
        }

        private string GetInsertCommandFields()
        {
            var result = new StringBuilder($"INSERT INTO {_tableName} (");

            foreach (var item in _classProperties)
            {
                result.Append(item.Name);
                result.Append(",");
            }

            return result.Replace(",", ")", result.Length - 1, 1).ToString();
        }

        private string GetInsertCommandParameters()
        {
            var result = new StringBuilder($" VALUES (");


            for (int i = 0; i < _classProperties.Length; i++)
            {
                result.Append($"@P{i}");
                result.Append(",");
            }

            return result.Replace(",", ")", result.Length - 1, 1).ToString();
        }


        public SqlCommand BuildInsertSqlCommand(T inObject)
        {

            SqlCommand sqlCommand = new SqlCommand(GetInsertCommandFields() + GetInsertCommandParameters(), _connection);

            for (int i = 0; i < _classProperties.Length; i++)
            {
                var sqlParameter = new SqlParameter { ParameterName = $"@P{i}", Value = _classProperties[i].GetValue(inObject) };
                sqlCommand.Parameters.Add(sqlParameter);
            }


            return sqlCommand;
        }

        private List<(string, int)> GetPK()
        {
            var result = new List<(string, int)>();

            PKAttribute pkAttrInfo;

            for (int i = 0; i < _classProperties.Length; i++)
            {
                pkAttrInfo = _classProperties[i].GetCustomAttribute<PKAttribute>();

                if (pkAttrInfo != null && pkAttrInfo.IsPrimaryKey)
                {
                    result.Add((_classProperties[i].Name, i));
                }
            }

            if (result.Count == 0)
                throw new Exception($"Update failed. Primary key of {_tableName} is not set in the model class {typeof(T).Name}.");
            else
                return result;
        }


        private SqlCommand BuildUpdateSqlCommand(T newObject)
        {
            var PK = GetPK();
            var updateText = GetUpdateCommandText(PK);

            SqlCommand sqlCommand = new SqlCommand(updateText, _connection);

            for (var i = 0; i < _classProperties.Length; i++)
            {
                var sqlParameter = new SqlParameter { ParameterName = $"@P{i}", Value = _classProperties[i].GetValue(newObject) };
                sqlCommand.Parameters.Add(sqlParameter);
            }


            return sqlCommand;
        }

        private SqlCommand BuildCountSqlCommand(T newObject)
        {
            var PK = GetPK();
            var selectText = GetSelectCountCommandText(PK);
            var sqlCommand = new SqlCommand(selectText, _connection);

            for (var i = 0; i < PK.Count; i++)
            {
                var sqlParameter = new SqlParameter
                {
                    ParameterName = $"@P{i}",
                    Value = _classProperties[PK[i].Item2].GetValue(newObject)
                };
                sqlCommand.Parameters.Add(sqlParameter);
            }

            return sqlCommand;
        }

        private string GetSelectCountCommandText(List<(string pkName, int pkIndex)> primaryKeys)
        {
            var result = new StringBuilder($"SELECT COUNT(*) FROM {_tableName} WHERE ");
            var pkIndex = 0;
            var where = primaryKeys.Aggregate("", (a, b) => $"{a}{b.pkName} = @P{pkIndex} AND ");
            where = where.Substring(0, where.Length - 4);


            result.Append(where);
            return result.ToString();
        }

        private string GetUpdateCommandText(List<(string pkName, int pkIndex)> primaryKeys)
        {
            var result = new StringBuilder($"UPDATE {_tableName} SET ");

            for (int i = 0; i < _classProperties.Length; i++)
            {
                var propName = _classProperties[i].Name;

                if (!primaryKeys.Select(x => x.Item1).Contains(propName))
                {
                    result.Append($"{propName} = @P{i}");
                    result.Append(",");
                }
            }

            result.Replace(",", "", result.Length - 1, 1).ToString();

            var where = primaryKeys.Aggregate(" WHERE ", (a, b) => $"{a}{b.pkName} = @P{b.pkIndex} AND ");
            where = where.Substring(0, where.Length - 4);


            result.Append(where);
            return result.ToString();
        }
    }
}
