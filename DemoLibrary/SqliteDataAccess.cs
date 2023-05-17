using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Xml.Linq;
using DBLink.Classes;
#pragma warning disable IDE0063 // Use simple 'using' statement

namespace DBLink
{
    public static class SqliteDataAccess
    {
        internal static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        /// <summary>
        /// Insert, update, delete operations only
        /// </summary>
        public static int ExecCmd(string cmd)
        {
            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                cnn.Open();
                SQLiteCommand command = new(cmd, cnn);
                cnn.Execute(cmd);
                int id = (int)cnn.LastInsertRowId;
                cnn.Close();

                return id;
            }
        }


        // Load player from DB given an Id
        public static T QuerySingleById<T>(int id, string table)
        {
            if (id == 0) throw new NullReferenceException($"No element in table {table} with id {id}");

            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                T output = cnn.QuerySingleOrDefault<T>($"SELECT * FROM {table} WHERE Id={id}", new DynamicParameters());

                if (output == null) throw new NullReferenceException($"No element in table {table} with id {id}");
                
                return output;
            }
        }

        public static List<T> QueryMultipleByOwnerId<T>(string table, int OwnerId = 0)
        {
            // Select all in table
            string cmd = $"SELECT * FROM {table}";

            // Filter by OwnerId
            if (OwnerId > 0) cmd = $"SELECT * FROM {table} where OwnerId={OwnerId}";

            using (SQLiteConnection cnn = new(SqliteDataAccess.LoadConnectionString()))
            {
                IEnumerable<T> output = cnn.Query<T>(cmd, new DynamicParameters());

                return output.ToList();
            }
        }
    }
}
