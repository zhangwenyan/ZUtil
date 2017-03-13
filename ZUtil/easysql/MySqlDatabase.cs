using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZUtil;
namespace easysql
{

    public class MySqlDatabase:BaseDatabase
    {
        private static Type _ConnectionType;
        private static Type _DataAdapterType;
        private static String filename = "mysql.data.dll";
        private static String clientName = "MySql.Data.MySqlClient";


        public String _connString;
        /// <summary>
        /// 通过连接字符串获得连接
        /// </summary>
        /// <param name="connString">server=数据库地址;database=数据库名;Persist Security Info=False;uid=用户名;pwd=密码</param>
        public MySqlDatabase(String connString)
            : base("@Param", "?@Param",DBType.mysql)
        {
            this._connString = connString;
            if(_ConnectionType == null)
            {
        
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.LoadFile(FileUtil.findFile(filename)); // 加载程序集（EXE 或 DLL） 
                }
                catch (Exception e)
                {
                    throw new Exception("缺少"+filename+"文件");
                }

                _ConnectionType = assembly.GetType(clientName+".MySqlConnection");
                _DataAdapterType = assembly.GetType(clientName+".MySqlDataAdapter");

                if (_ConnectionType == null || _DataAdapterType==null)
                {
                    throw new Exception(filename+"不符合要求");
                }
            }

        }

        #region 必须重载的方法
        protected override System.Data.Common.DbConnection CreateConnection()
        {
            dynamic obj = Activator.CreateInstance(_ConnectionType, new object[] { _connString });
            return obj;
        }
        protected override System.Data.Common.DbDataAdapter CreateAdapter(System.Data.Common.DbCommand cmd)
        {
            dynamic dy = Activator.CreateInstance(_DataAdapterType, new object[] { cmd });
            return dy;
        }
        protected override string AutoIncreSql()
        {
            return " select last_insert_id()";
        }
        #endregion 必须重载的方法
        protected override string getLimitString(string sql, ref int start, int maxResult)
        {
            sql = sql + " limit " + start + "," + maxResult + " ";
            start = 0;
            return sql;
        }
    }
}
