using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OracleClient;

namespace easysql
{
    public class OracleDatabase:BaseDatabase
    {
        private String _connString;
        public OracleDatabase(String connString)
            : base("Param", ":Param")
        {
            _connString = connString;
        }

        #region 必须重载的父类方法
        protected override System.Data.Common.DbConnection CreateConnection()
        {
            return new OracleConnection(_connString);
        }

        protected override System.Data.Common.DbDataAdapter CreateAdapter(System.Data.Common.DbCommand cmd)
        {
            return new OracleDataAdapter(cmd as OracleCommand);
        }
        protected override string AutoIncreSql()
        {
            throw new NotImplementedException("oracle数据库暂时不支持该方法");
        }
        #endregion 必须重载的父类方法

    }
}
