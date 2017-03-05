using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Reflection;

namespace easysql
{
    /// <summary>
    /// 数据库连接的基类
    /// </summary>
    public abstract class BaseDatabase : IDisposable
    {

        protected string _paramNamePrefix;
        protected string _paramPrefix;
        protected DbConnection _dbConnection;
        protected int _executeTimeout = 60;//执行超时时间
        private DbTransaction _dbTranscation;//事务

        protected BaseDatabase(String paramNamePrefix,String paramPrefix){
            this._paramNamePrefix = paramNamePrefix;
            this._paramPrefix = paramPrefix;
        }
        #region 必须重载的方法
        /// <summary>
        /// 建议，如果没有则select 0;
        /// </summary>
        /// <returns></returns>
        protected abstract string AutoIncreSql();
        protected abstract DbConnection CreateConnection();
        protected abstract DbDataAdapter CreateAdapter(DbCommand cmd);
        #endregion 必须重载的方法




        #region 数据库开关释放等
        public Boolean IsOpen()
        {
            return _dbConnection != null && _dbConnection.State == ConnectionState.Open;
        }
        public void Open()
        {
            if(_dbConnection!=null){
                if (IsOpen())
                {
                    _dbConnection.Close();
                }
                _dbConnection.Dispose();
            }
            _dbConnection = CreateConnection();
            _dbConnection.Open();
        }
        public void Close()
        {
            if (IsOpen())
            {
                _dbConnection.Close();
            }

        }
        public void Dispose()
        {
            Close();
            if (_dbConnection != null)
            {
                _dbConnection.Dispose();
            }
        }



        #endregion 数据库开关释放等


        #region 数据库核心方法
        /// <summary>
        /// 执行select查询并返回结果
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public virtual DataTable ExecuteDataTable(String sql, params Object[] paramValues)
        {

            using (DbCommand cmd = CreateDbCommand())
            {
                cmd.CommandTimeout = _executeTimeout;
                RetreatCmd(sql, cmd, paramValues);
                using (DbDataAdapter adp = CreateAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    return dt;
                }
            }
        }

        protected virtual String getLimitString(String sql, ref int start, int maxResult)
        {
            return sql;
        }

        /// <summary>
        /// 执行select查询并返回部分结果
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="start"></param>
        /// <param name="maxResult"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public virtual DataTable ExecuteDataTable(String sql, int start, int maxResult, params Object[] paramValues)
        {
            using (DbCommand cmd = CreateDbCommand())
            {
                cmd.CommandTimeout = _executeTimeout;
                sql = getLimitString(sql, ref start, maxResult);
                RetreatCmd(sql, cmd, paramValues);
                using (DbDataAdapter adp = CreateAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(start, maxResult, dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// 执行更新操作
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public virtual int Execute(String sql, params Object[] paramValues)
        {
            using (DbCommand cmd = CreateDbCommand())
            {
                RetreatCmd(sql, cmd, paramValues);
                int rowCount = cmd.ExecuteNonQuery();
                return rowCount;
            }
        }
        /// <summary>
        /// 执行返回单个值得查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public virtual Object ExecuteScalar(String sql, params Object[] paramValues)
        {
            using (DbCommand cmd = CreateDbCommand())
            {
                RetreatCmd(sql, cmd, paramValues);
                Object result = cmd.ExecuteScalar();
                return result;
            }
        }


        #endregion 数据库核心方法

        #region 数据库扩展方法

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public List<T> QueryBySql<T>(String sql, params Object[] paramValues) where T : new()
        {
            DataTable dt = ExecuteDataTable(sql, paramValues);
            return DBUtil.ToList<T>(dt);
        }

        public List<dynamic> QueryBySql_Dy(String sql,params Object[] paramValues)
        {
            DataTable dt = ExecuteDataTable(sql, paramValues);
            return DBUtil.ToDynamic(dt);
        }


        public List<T> QueryPageBySql<T>(String sql, int page, int rows,out int total, params Object[] paramValues) where T : new()
        {
            DataTable dt = ExecuteDataTable(sql, (page - 1) * rows, rows, paramValues);
            String sqlCount = DBUtil.getSqlCount(sql);
            total = int.Parse(ExecuteScalar(sqlCount, paramValues).ToString());
            return DBUtil.ToList<T>(dt);
        }
        public List<dynamic> QueryPageBySql_Dy(String sql, int page, int rows,out int total,  params Object[] paramValues)
        {
            DataTable dt = ExecuteDataTable(sql, (page - 1) * rows, rows, paramValues);
            String sqlCount = DBUtil.getSqlCount(sql);
            total = (int)ExecuteScalar(sqlCount, paramValues);
            return DBUtil.ToDynamic(dt);
        }





        /// <summary>
        /// 根据bean以及restrains里面的约束查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbname"></param>
        /// <param name="bean"></param>
        /// <param name="restrains"></param>
        /// <returns></returns>
        public List<T> Query<T>(String tbname, T bean, params Restrain[] restrains) where T : new()
        {
            var sqlWhere = new StringBuilder();
            var sqlOrder = new StringBuilder();
            var paramValues = new List<Object>();
            DBUtil.QueryBean(bean, sqlWhere, sqlOrder, paramValues, restrains);
            var sql = "select * from " + tbname + " where 1=1 " + sqlWhere +" "+ sqlOrder;
            return QueryBySql<T>(sql, paramValues.ToArray());
            //DataTable dt = ExecuteDataTable(sql, paramValues.ToArray());
            //return DBUtil.ToList<T>(dt);
        }

        public List<dynamic> Query_Dy(String tbname,Object bean,params Restrain[] restrains)
        {
            var sqlWhere = new StringBuilder();
            var sqlOrder = new StringBuilder();
            var paramValues = new List<Object>();
            DBUtil.QueryBean(bean, sqlWhere, sqlOrder, paramValues, restrains);
            var sql = "select * from " + tbname + " where 1=1 " + sqlWhere + " " + sqlOrder;
            var dt = ExecuteDataTable(sql, paramValues.ToArray());
            return DBUtil.ToDynamic(dt);
        }

 


        /// <summary>
        /// 查询数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbname"></param>
        /// <param name="bean"></param>
        /// <param name="restrains"></param>
        /// <returns></returns>
        public int Total<T>(String tbname, T bean, params Restrain[] restrains)
        {
            var sqlWhere = new StringBuilder();
            var paramValues = new List<Object>();
            DBUtil.QueryBean(bean, sqlWhere, null, paramValues, restrains);
            var sql = "select count(*) from " + tbname + " where 1=1 " + sqlWhere;
            return Total(sql, paramValues.ToArray());
        }



        public int Total(String sql,params Object[] paramValues)
        {
            return int.Parse(this.ExecuteScalar(sql, paramValues.ToArray()).ToString());
        }


        private DataTable QueryByPage_Dt(string tbname, Object bean, int page, int rows, out int total, params Restrain[] restrains)
        {
            var sqlWhere = new StringBuilder();
            var sqlOrder = new StringBuilder();
            var paramValues = new List<Object>();
            DBUtil.QueryBean(bean, sqlWhere, sqlOrder, paramValues, restrains);
            var sql = "select * from " + tbname + " where 1=1 " + sqlWhere + " " + sqlOrder;
            var sqlCount = "select count(*) from " + tbname + " where 1=1 " + sqlWhere;
            total = int.Parse(this.ExecuteScalar(sqlCount, paramValues.ToArray()).ToString());
            DataTable dt = ExecuteDataTable(sql, (page - 1) * rows, rows, paramValues.ToArray());
            return dt;
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbname"></param>
        /// <param name="bean"></param>
        /// <param name="page">页码(与start不同,从1开始)</param>
        /// <param name="rows">页大小</param>
        /// <param name="total"></param>
        /// <param name="restrains"></param>
        /// <returns></returns>
        public List<T> QueryByPage<T>(string tbname, T bean, int page, int rows, out int total, params Restrain[] restrains) where T : new()
        {
            var dt = QueryByPage_Dt(tbname, (object)bean, page, rows, out total, restrains);
            return DBUtil.ToList<T>(dt);
        }
        public List<dynamic> QueryByPage_Dy(string tbname, object bean, int page, int rows, out int total, params Restrain[] restrains)
        {
            var dt = QueryByPage_Dt(tbname, (object)bean, page, rows, out total, restrains);
            return DBUtil.ToDynamic(dt);

        }


        /// <summary>
        /// 能够快速的增加多条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbname"></param>
        /// <param name="modelList"></param>
        public void Add<T>(string tbname,List<T> modelList)
        {
            foreach(T model in modelList)
            {
                Add<T>(tbname, model);
            }
        }

        private void Add<T>(string tbname, T model, Boolean autoSetId)
        {
            var sql1 = new StringBuilder();
            var sql2 = new StringBuilder();
            var i = 0;
            var paramValues = new List<Object>();
            Type type = model.GetType();
            PropertyInfo[] pis = type.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                var name = pi.Name;
                var value = pi.GetValue(model, null);

                if (value == null)
                {
                    continue;
                }

                if (value.Equals(DBUtil.DefaultForType(pi.PropertyType)))
                {
                    //如果值是默认状态,则跳过????
                    continue;
                }

                sql1.Append(name + ",");
                sql2.AppendFormat("{{{0}}},", i++);
                paramValues.Add(value);
            }
            if (sql1.Length>0)
            {
                sql1.Remove(sql1.Length - 1, 1);//一处最后的","
                sql2.Remove(sql2.Length - 1, 1);
            }
            var sql = "insert into " + tbname + "(" + sql1 + ") values(" + sql2 + ")";
            if (autoSetId)
            {
                var cmdText = sql +" " + AutoIncreSql();
                object id = int.Parse(ExecuteScalar(cmdText, paramValues.ToArray()).ToString());
                SetIdValue(model, id);//设置model的Id属性的值
            }
            else
            {
                Execute(sql, paramValues.ToArray());
            }
        }
        public void Add<T>(string tbname, T model)
        {
            Add(tbname, model, false);

        }
        public void Add_AutoSetId<T>(string tbname, T model)
        {
            Add(tbname, model, true);
        }


        /// <summary>
        /// 批量删除多条记录,太危险了,一不小心就删掉所有记录了
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbname"></param>
        /// <param name="bean"></param>
        /// <param name="restrains"></param>
        /// <returns></returns>
        public int Del<T>(string tbname, T bean, params Restrain[] restrains)
        {
            var sqlWhere = new StringBuilder();
            var paramValues = new List<object>();

            DBUtil.QueryBean(bean, sqlWhere, null, paramValues, restrains);

            var sql = "delete from " + tbname + " where 1=1 " + sqlWhere;
            return Execute(sql, paramValues.ToArray());
        }
        /// <summary>
        /// 根据id删除对象
        /// </summary>
        /// <param name="tbname"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Boolean DelById(string tbname, int id)
        {
            var sql = "delete from " + tbname + " where id={0}";

            return Execute(sql, id) == 1;

        }
        /// <summary>
        ///  修改Id为model.Id的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbname"></param>
        /// <param name="model"></param>
        /// <param name="pros">可以指定一定要修改的参数</param>
        public void Modify<T>(String tbname,T model,String[] pros)
        {
            HashSet<string> hs = new HashSet<string>();
            foreach(var key in pros)
            {
                hs.Add(key.ToLower());
            }

            var sql1 = new StringBuilder();
            var i = 0;
            var paramValues = new List<Object>();
            Type type = model.GetType();
            PropertyInfo[] pis = type.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                String name = pi.Name;
                Object value = pi.GetValue(model, null);

                if (!hs.Contains(name))
                {
                    if (value == null)
                    {
                        continue;
                    }
                    if (value.Equals(DBUtil.DefaultForType(pi.PropertyType)))
                    {
                        continue;
                    }
                    else if (name.ToUpper().Equals("ID"))
                    {
                        continue;
                    }
                }
                sql1.AppendFormat("{0}={{{1}}},", name, i++);

                if (value is DateTime && (DateTime)value == DateTime.MinValue)
                {
                    paramValues.Add(null);
                }
                else
                {
                    paramValues.Add(value);
                }
            }

            if (sql1.Length == 0)
            {
                //没有需要修改的
                return;
            }
            sql1 = sql1.Remove(sql1.Length - 1, 1);
            var sql = String.Format("update {0} set {1} where Id={{{2}}}", tbname, sql1, i++);

            object id = GetIdValue(model);
            paramValues.Add(id);
            Execute(sql, paramValues.ToArray());
        }

        /// <summary>
        /// 修改Id为model.Id的数据
        /// bug?无法将int类型修改为0,无法将DateTime类型修改为DateTime.minValue....
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbname"></param>
        /// <param name="model"></param>
        public int Modify<T>(String tbname, T model)
        {
            var sql1 = new StringBuilder();
            var i = 0;
            var paramValues = new List<Object>();
            Type type = model.GetType();
            PropertyInfo[] pis = type.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                String name = pi.Name;
                Object value = pi.GetValue(model, null);
                if (value == null)
                {
                    continue;
                }
                if (value.Equals(DBUtil.DefaultForType(pi.PropertyType)))
                {
                    continue;
                }
                else if (name.ToUpper().Equals("ID"))
                {
                    continue;
                }

                sql1.AppendFormat("{0}={{{1}}},", name, i++);
                paramValues.Add(value);

            }

            if (sql1.Length == 0)
            {
                //没有需要修改的
                return 0;
            }
            sql1 = sql1.Remove(sql1.Length - 1, 1);
            var sql = String.Format("update {0} set {1} where Id={{{2}}}", tbname, sql1, i++);

            object id = GetIdValue(model);
            paramValues.Add(id);
            return Execute(sql, paramValues.ToArray());
        }

            /// <summary>
        /// 通过id查询对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbname"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public T QueryById<T>(string tbname, int id) where T : new()
        {
            var sql = "select * from " + tbname + " where id={0}";
            DataTable dt = ExecuteDataTable(sql, id);
            if (dt.Rows.Count < 1)
            {
                return default(T);
            }
            DataRow dr = dt.Rows[0];
            return DBUtil.ToModel<T>(dr);
        }


        #endregion 数据库扩展方法



        #region 事务核心辅助方法
        private DbCommand CreateDbCommand()
        {
            var cmd = _dbConnection.CreateCommand();
            if (_dbTranscation != null)
            {
                cmd.Transaction = _dbTranscation;
            }

            return cmd;
        }


        #endregion 事务核心辅助方法
        #region 辅助方法
        /// <summary>
        /// 重新格式化处理查询语句并生成查询参数
        /// 即select * from tb where id={0} and name={1}转化为类似select * from tb where id=@params1,@parms2的形式,并将参数值添加到cmd中
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmd"></param>
        /// <param name="paramValues"></param>
        protected void RetreatCmd(String sql, DbCommand cmd, object[] paramValues)
        {
            //如果无参数则直接返回
            if (paramValues == null || paramValues.Length == 0)
            {
                cmd.CommandText = sql;
                return;
            }
            String[] paramsName = new string[paramValues.Length];
            for (int i = 0; i < paramValues.Length; i++)
            {
                //创建查询新参数
                DbParameter oneParam = cmd.CreateParameter();
                paramsName[i] = _paramPrefix + i.ToString();
                oneParam.ParameterName = _paramNamePrefix + i.ToString();
                oneParam.Value = paramValues[i] ?? DBNull.Value;
                //添加至command对象参数集中
                cmd.Parameters.Add(oneParam);
            }
            sql = String.Format(sql, paramsName.ToArray());

            cmd.CommandText = sql;

        }

       
    
        //设置对象的属性名称
        private static void SetProValue(Object model, String name, Object value)
        {
            model.GetType().GetProperty(name).SetValue(model, value, null);

        }
        private object GetProValue(Object model, String name)
        {
            return model.GetType().GetProperty(name).GetValue(model, null);

        }
        private void SetIdValue(Object model, Object value)
        {
            if (model == null)
            {
                return;
            }

            var ps = model.GetType().GetProperties();
            foreach (var p in ps)
            {
                if (p.Name.ToUpper().Equals("ID"))
                {
                    p.SetValue(model, value, null);
                    continue;
                }
            }

        }
        private Object GetIdValue(Object model)
        {
            var ps = model.GetType().GetProperties();
            foreach (var p in ps)
            {
                if (p.Name.ToUpper().Equals("ID"))
                {
                    return p.GetValue(model, null);
                }
            }
            return null;
        }


        #endregion 辅助方法


        #region 事务方法
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public void BeginTransaction()
        {
            if (_dbTranscation != null)
            {
                throw new Exception("开启事务失败,有线程正在开启事务并没有释放");
            }

            _dbTranscation = _dbConnection.BeginTransaction();
        }
        /// <summary>
        /// 提交事务,并释放(dispose后并设置为null)
        /// </summary>
        /// <param name="dbTranscation"></param>
        public void CommitTranscation()
        {
            if (_dbTranscation == null)
            {
                return;
            }
            try
            {
                _dbTranscation.Commit();
            }
            finally
            {
                _dbTranscation.Dispose();
                _dbTranscation = null;
            }

        }

        /// <summary>
        /// 回滚事务,并释放
        /// </summary>
        /// <param name="dbTranscation"></param>
        public void RollbackTranscation()
        {
            if (_dbTranscation == null)
            {
                return;
            }
            try
            {
                _dbTranscation.Rollback();
            }
            finally
            {
                _dbTranscation.Dispose();
                _dbTranscation = null;
            }
        }


        #endregion 事务方法





    }
}
