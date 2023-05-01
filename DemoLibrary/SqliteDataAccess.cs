using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Data.SqlTypes;
#pragma warning disable IDE0063 // Use simple 'using' statement

namespace DBLink
{
    public static class SqliteDataAccess
    {
        internal static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
