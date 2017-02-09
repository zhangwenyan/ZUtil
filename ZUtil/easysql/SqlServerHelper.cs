using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace easysql
{
    public class SqlServerHelper:BaseDBHelper
    {
        private String _connString;
        public SqlServerHelper(String connString)
        {
            _connString = connString;
        }
        public override BaseDatabase CreateDatabase()
        {
            return DatabaseFactory.CreateSqlServerDatabase(_connString);
        }


    }
}
