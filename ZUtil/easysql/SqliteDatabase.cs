using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace easysql
{
    public class SqliteDatabase:BaseDatabase
    {

        private static Type _ConnectionType;
        private static Type _DataAdapterType;
        private static String filename = "System.Data.SQLite.dll";
        private static String clientName = "System.Data.SQLite";


        private String _connString;
        public SqliteDatabase(String connString)
            : base("@Param", "@Param")
        {
            this._connString = connString;
            if (_ConnectionType == null)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + filename); // 加载程序集（EXE 或 DLL） 
                }
                catch (Exception)
                {
                    throw new Exception("缺少"+filename+"文件");
                }

                _ConnectionType = assembly.GetType(clientName+".SQLiteConnection");

                _DataAdapterType = assembly.GetType(clientName+".SQLiteDataAdapter");

                if (_ConnectionType == null || _DataAdapterType == null)
                {
                    throw new Exception(filename+"不符合要求");
                }

            }



        }

        #region 必须重载的父类方法

        protected override System.Data.Common.DbConnection CreateConnection()
        {
            dynamic obj = Activator.CreateInstance(_ConnectionType, new Object[] { _connString });
            return obj;
        }

        protected override System.Data.Common.DbDataAdapter CreateAdapter(System.Data.Common.DbCommand cmd)
        {
            dynamic dy = Activator.CreateInstance(_DataAdapterType, new object[] { cmd });
            return dy;
        }
        protected override string AutoIncreSql()
        {
            return "select last_insert_rowid() ";
        }

        #endregion 必须重载的父类方法
    }
}
