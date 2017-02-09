using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace easysql
{
    public class DBHelperFactory
    {

        /// <summary>
        /// 创建数据库操作类对象,
        /// </summary>
        /// <param name="dbType">数据库类型名称,支持:mysql,sqlserver,oracle,sqlite,excel不区分大小写</param>
        /// <param name="connString">数据库连接字符串</param>
        /// <returns></returns>
        public static BaseDBHelper Create(String dbType, String connString)
        {
            switch (dbType.ToLower())
            {
                case "mysql":
                    return CreateMysqlHelper(connString);
                case "sqlserver":
                case "mssqlserver":
                    return CreateSqlServerHelper(connString);
                case "oracle":
                    return CreateOracleHelper(connString);
                case "sqlite":
                    return CreateSqliteHelper(connString);
                case "excel":
                    return CreateExcelHelper(connString);
            }
            throw new Exception("该数据库类型不支持");
        }
        public static MySqlHelper CreateMysqlHelper(string connString)
        {
            return new MySqlHelper(connString);
        }
        public static MySqlHelper CreateMysqlHelper(string server, string dbname, string username, string pwd)
        {
            return new MySqlHelper(string.Format("server={0};database={1};Persist Security Info=False;uid={2};pwd={3}", server, dbname, username, pwd));
        }

        public static SqlServerHelper CreateSqlServerHelper(string connString)
        {
            return new SqlServerHelper(connString);
        }
        public static SqlServerHelper CreateSqlServerHelper(string server, string dbname, string username, string pwd)
        {
            return new SqlServerHelper(string.Format("user id={0};password={1};initial catalog={2};Server={3}", username, pwd, dbname, server));
        }

        public static OracleHelper CreateOracleHelper(string connString)
        {
            return new OracleHelper(connString);
        }
        public static SqliteHelper CreateSqliteHelper(string connString)
        {
            return new SqliteHelper(connString);
        }
        public static SqliteHelper CreateSqliteHelperByFilename(string filename)
        {

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(System.IO.Path.GetFullPath(filename));
            }
            return CreateSqliteHelper("Data Source=" + filename);
        }

        public static ExcelHelper CreateExcelHelper(string connString)
        {
            return new ExcelHelper(connString);
        }

        /// <summary>
        /// 此连接可以操作.xls与.xlsx文件 (支持Excel2003 和 Excel2007 的连接字符串)
        /// 即:CreateExcelHelper("Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + filename + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'");
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static ExcelHelper CreateExcelHelperByFilename(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(System.IO.Path.GetFullPath(filename));
            }
            return CreateExcelHelper("Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + filename + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'");
        }

    }
}
