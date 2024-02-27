using System.Configuration;
using System.Data.SqlClient;

namespace InitialImport
{
    /// <summary>
    /// Creates Db and its tables (throws exception in case of errors)
    /// Uses app.confing connection string
    /// and queries stored as the project's string recourses
    /// </summary>   
    internal class DBCreator
    {       
        public void CreateDataBase()
        {
            ExecuteCreationCommand(
                    ConfigurationManager.ConnectionStrings["eStarMaster"]?.ToString(),
                    Properties.Resources.Create_DB);
        }

        public void CreateTables()
        {
            ExecuteCreationCommand(
                    ConfigurationManager.ConnectionStrings["eStarTest"]?.ToString(),
                    Properties.Resources.Create_Tables);
        }

        private void ExecuteCreationCommand(string connectionString,  string sqlQuery)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(sqlQuery, cn);
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
    }
}
