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
    }
}
