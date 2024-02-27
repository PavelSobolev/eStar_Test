using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace DBProcessing
{
    internal enum DBOperation
    {
        InsertOnly,
        InsertOrUpdate
    }

    /// <summary>
    /// Imports csv data conforming the definition of type T
    /// </summary>
    /// <typeparam name="T">type of an object being imported</typeparam>
    public class CsvImporter<T> where T : class, new()
    {

        private readonly List<Action<T>> _transformActions;
        private readonly List<Func<T, (bool, string)>> _validationActions;
        private readonly ErrorInfo _errorINfo;

        /// <summary>
        /// Collection of actions that can be performed on the object of T
        /// </summary>
        public List<Action<T>> TransformActions => _transformActions;

        /// <summary>
        /// Collection of functions that can be performed on the object of T
        /// to check correctness of the values of its properties 
        /// </summary>
        public List<Func<T, (bool,string)>> ValidationActions => _validationActions;

        public CsvImporter(ErrorInfo errorINfo)
        {
            _transformActions = new List<Action<T>>();
            _validationActions = new List<Func<T, (bool, string)>>();
            _errorINfo = errorINfo;
        }

        public void InsertFromCsv(string fileName, SqlConnection sqlConnection)
        {
            sqlConnection.Open();
            ReadAndExecuteDBOperation(fileName, sqlConnection, DBOperation.InsertOnly);
            sqlConnection.Close();
        }

        public void UpdateFromCsv(string fileName, SqlConnection sqlConnection)
        {
            sqlConnection.Open();
            ReadAndExecuteDBOperation(fileName, sqlConnection, DBOperation.InsertOrUpdate);
            sqlConnection.Close();
        }

        private void ReadAndExecuteDBOperation(string fileName, SqlConnection sqlConnection, DBOperation op)
        {
            var command = new DataBaseCommand<T>(sqlConnection, _errorINfo);

            using (var sr = new StreamReader(fileName))
            {
                sr.ReadLine(); // read file header

                ProcessStream(sr, command, op);                
            }
        }

        private void ProcessStream(StreamReader sr, DataBaseCommand<T> command, DBOperation op)
        {
            while (!sr.EndOfStream)
            {
                var objectT = sr.ReadCSVLineOfType<T>(); // get object of T from file

                ApplyTransofrmation(objectT);

                if (IsValid(objectT))
                {
                    if (op == DBOperation.InsertOnly)
                        command.Insert(objectT);
                    else
                        command.Upsert(objectT);
                }
            }
        }

        private void ApplyTransofrmation(T t)
        {
            // execute registered transformations on object's properties
            _transformActions.ForEach(transformMethod => transformMethod(t));
        }

        private bool IsValid(T t)
        {
            var outMessage = new StringBuilder();            

            foreach(var validationMethod in _validationActions)
            {
                var res = validationMethod(t); 
                if (!res.Item1)
                {
                    outMessage.AppendLine(res.Item2);
                }
            }

            if (outMessage.Length > 0)
            {
                _errorINfo.WriteMessage(outMessage.ToString());
            }

            return outMessage.Length == 0;

        }
    }
}
