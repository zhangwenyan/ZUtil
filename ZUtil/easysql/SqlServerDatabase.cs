using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace easysql
{
    /// <summary>
    /// 微软sqlserver数据库
    /// </summary>
    public class SqlServerDatabase:BaseDatabase
    {
        private String _connString;
        /// <summary>
        /// 通过连接字符串获得连接
        /// </summary>
        /// <param name="connString">user id=用户名;password=密码;initial catalog=数据库名;Server=服务地址</param>
        public SqlServerDatabase(String connString)
            : base("@Param","@Param")
        {
            this._connString = connString;
        }

        #region 必须重载的父类方法
        protected override System.Data.Common.DbConnection CreateConnection()
        {
            return new SqlConnection(_connString);
        }
        protected override System.Data.Common.DbDataAdapter CreateAdapter(System.Data.Common.DbCommand cmd)
        {
            return new SqlDataAdapter(cmd as SqlCommand);
        }

        protected override string AutoIncreSql()
        {
            return " select @@identity ";
        }
        #endregion 必须重载的父类方法
        protected override string getLimitString(string sql, ref int start, int maxResult)
        {
            int selectIndex = sql.ToLower().IndexOf("select");
            var m = new Regex("select\\sdistinct").Match(sql.ToLower());
            int selectDistinctIndex = m.Success ? m.Index : -1;
            sql = sql.Insert(selectIndex + (selectDistinctIndex == selectIndex ? 15 : 6), " top 100 percent ");




            String str = @"select * from (
                select *,row_number() over (order  by current_timestamp) as _easysql_rn from ("
                + sql +
                @") easysql_query 
                  ) easysql_result where 1=1 ";

            Regex reg = new Regex(@"^([\s\S]*)(order\s+by\s+(\w+(\s+desc)?)(,(\w+(\s+desc)?))*)\s*$");
            var match = reg.Match(sql);
            if (match.Success)
            {
                str = @"select * from (
                select *,row_number() over (" + match.Groups[2].Value + ") as _easysql_rn from ("
                    + match.Groups[1].Value +
                    @") easysql_query 
                  ) easysql_result where 1=1 ";
            }



            if (start != 0)
            {
                str += " and _easysql_rn>" + start;
            }

            if (maxResult != 0)
            {
                if (start == 0 || start < 1)
                {
                    str += " and _easysql_rn<=" + maxResult;
                }
                else
                {
                    str += " and _easysql_rn<=" + (start + maxResult);
                }
            }
            start = 0;
            return str;
        }

    }
}
