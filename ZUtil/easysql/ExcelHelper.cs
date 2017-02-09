using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace easysql
{
    public class ExcelHelper:BaseDBHelper
    {
        private string _connString;
        public ExcelHelper(string connString)
        {
            _connString = connString;
        }

        public override BaseDatabase CreateDatabase()
        {
            return new ExcelDatabase(_connString);
        }
    }
}
