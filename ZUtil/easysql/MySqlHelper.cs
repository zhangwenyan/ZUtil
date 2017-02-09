using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace easysql
{
    public class MySqlHelper:BaseDBHelper
    {
        private String _connString;
        public MySqlHelper(String connString)
        {
            _connString = connString;
        }
        public override BaseDatabase CreateDatabase()
        {
            return DatabaseFactory.CreateMySqlDatabase(_connString);
        }

    }
}
