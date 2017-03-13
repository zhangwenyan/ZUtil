using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace easysql
{
    public class OleDatabase:BaseDatabase
    {
        protected String _autoIncreSqlStr;
        protected String _connString;
        public OleDatabase(String connString, String paramNamePrefix, String paramPrefix, String autoIncreSqlStr)
            : base(paramNamePrefix, paramPrefix,DBType.ole)
        {
            this._connString = connString;
            this._autoIncreSqlStr = autoIncreSqlStr;
        }

        #region 必须重载的父类方法
        protected override System.Data.Common.DbConnection CreateConnection()
        {
            return new OleDbConnection(_connString);
        }
        protected override System.Data.Common.DbDataAdapter CreateAdapter(System.Data.Common.DbCommand cmd)
        {
            return new OleDbDataAdapter(cmd as OleDbCommand);
        }
        protected override string AutoIncreSql()
        {
            return _autoIncreSqlStr;
        }
        #endregion 必须重载的父类方法


    }
}
