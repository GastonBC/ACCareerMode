using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Xml.Linq;
#pragma warning disable IDE0063 // Use simple 'using' statement

namespace DBLink
{
    public static class SqliteDataAccess
    {
        internal static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }


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

        public static List<T> QueryByOwnerId<T>(string table, int OwnerId = 0)
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
