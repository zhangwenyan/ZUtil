using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace easysql
{
    public static class DatabaseFactory
    {
        public static SqlServerDatabase CreateSqlServerDatabase(string connString)
        {
            return new SqlServerDatabase(connString);
        }
        public static MySqlDatabase CreateMySqlDatabase(string connString)
        {
            return new MySqlDatabase(connString);
        }
        public static OracleDatabase CreateOracleDatabase(string connString)
        {
            return new OracleDatabase(connString);
        }
        public static SqliteDatabase CreateSqliteDatabase(string connString)
        {
            return new SqliteDatabase(connString);
        }
        public static OleDatabase CreateOleDatabase(string connString, string paramNamePrefix, string paramPrefix, string autoIncreSqlStr)
        {
            return new OleDatabase(connString, paramNamePrefix, paramPrefix, autoIncreSqlStr);
        }

        public static ExcelDatabase CreateExcelDatabase(string connString)
        {
            return new ExcelDatabase(connString);
        }

    }
}
