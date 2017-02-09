using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace easysql
{
    public class SqliteHelper:BaseDBHelper
    {
        private String _connString;
        public SqliteHelper(String connString)
        {
            _connString = connString;
        }



        public override BaseDatabase CreateDatabase()
        {
            return DatabaseFactory.CreateSqliteDatabase(_connString);
        }
    }
}
