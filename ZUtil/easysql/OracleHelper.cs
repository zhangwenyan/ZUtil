using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace easysql
{
    public class OracleHelper:BaseDBHelper
    {
        private String _connString;
        public OracleHelper(String connString)
        {
            _connString = connString;
        }

        public override BaseDatabase CreateDatabase()
        {
            return DatabaseFactory.CreateOracleDatabase(_connString);
        }
    }
}
