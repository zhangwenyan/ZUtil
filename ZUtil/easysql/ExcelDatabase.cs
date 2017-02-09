using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace easysql
{
    public class ExcelDatabase:OleDatabase
    {
        /// <summary>
        /// Provider=Microsoft.Ace.OleDb.12.0;" + "data source=4.xls;Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'
        /// </summary>
        /// <param name="connString"></param>
        public ExcelDatabase(String connString)
            : base(connString, "@param", "@param", ";select @@identity")
        {
        }

    }
}
