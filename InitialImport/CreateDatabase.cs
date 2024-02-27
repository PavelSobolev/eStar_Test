using System;

namespace InitialImport
{
    internal partial class Program
    {
        #region creation of DB

        private static void ExecuteCreationScripts()
        {
            var cr = new DBCreator();
            bool creationSucceded = CreateDB(cr) && CreateTables(cr);

            if (!creationSucceded)
            {
                Console.WriteLine("\nProgram terminated due to errors.");
                Console.ReadKey();
            }
        }

        private static bool CreateDB(DBCreator cr)
        {
            try
            {
                Console.WriteLine("Creating database ...");
                Console.WriteLine(Properties.Resources.Create_DB);

                cr.CreateDataBase();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Creation of database failed.\n");
                Console.WriteLine(ex);
                err.WriteException(ex);
                return false;
            }

            Console.WriteLine("Success.\n");
            return true;
        }

        private static bool CreateTables(DBCreator cr)
        {
            try
            {
                Console.WriteLine("Creating tables ...");
                Console.WriteLine(Properties.Resources.Create_Tables);

                cr.CreateTables();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Creation of tables failed.");
                Console.WriteLine(ex);
                err.WriteException(ex);
                return false;
            }

            Console.WriteLine("Success.\n");
            return true;
        }
        #endregion
    }
}
