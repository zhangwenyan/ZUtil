using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace easysql
{
    public abstract class BaseDBHelper
    {
        public abstract BaseDatabase CreateDatabase();
        public BaseDatabase CreateDatabaseAndOpen()
        {
            BaseDatabase db = CreateDatabase();
            db.Open();
            return db;
        }
        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql, params object[] paramValues)
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.ExecuteDataTable(sql, paramValues);
            }
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="paramsValues"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTableByPage(string sql, int page, int rows, params object[] paramsValues)
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.ExecuteDataTable(sql, (page - 1) * rows, rows, paramsValues);
            }
        }
        /// <summary>
        /// 执行sql语句并返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public int Execute(string sql, params object[] paramValues)
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.Execute(sql, paramValues);
            }
        }
        /// <summary>
        /// 执行返回单值的查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, params object[] paramValues)
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.ExecuteScalar(sql, paramValues);
            }
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public List<T> QueryBySql<T>(String sql, params Object[] paramValues)where T:new()
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.QueryBySql<T>(sql, paramValues);
            }
        }
        /// <summary>
        ///  根据sql语句查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public List<dynamic> QueryBySql_Dy(String sql,params Object[] paramValues)
        {
            using(BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.QueryBySql_Dy(sql, paramValues);
            }
        }

        /// <summary>
        /// 根据sql语句进行分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public List<T> QueryPageBySql<T>(String sql, int page, int rows, params Object[] paramValues) where T : new()
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.QueryPageBySql<T>(sql, page, rows, paramValues);
            }
        }
        /// <summary>
        /// 根据sql语句进行分页查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public List<dynamic> QueryPageBySql_Dy(String sql, int page, int rows, params Object[] paramValues)
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.QueryPageBySql_Dy(sql, page, rows, paramValues);
            }
        }



        /// <summary>
        /// 根据bean以及restrains里面的约束查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbname"></param>
        /// <param name="bean"></param>
        /// <param name="restrains"></param>
        /// <returns></returns>
        public List<T> Query<T>(string tbname, T bean, params Restrain[] restrains) where T : new()
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.Query(tbname, bean, restrains);
            }

        }
        /// <summary>
        /// 根据bean以及restrains里面的约束查询
        /// </summary>
        /// <param name="tbname"></param>
        /// <param name="bean"></param>
        /// <param name="restrains"></param>
        /// <returns></returns>
        public List<dynamic> Query_Dy(String tbname,object bean,params Restrain[] restrains)
        {
            using(BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.Query_Dy(tbname, bean, restrains);
            }
        }

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbname"></param>
        /// <param name="bean"></param>
        /// <param name="restrains"></param>
        /// <returns></returns>
        public int Total<T>(string tbname, T bean, params Restrain[] restrains)
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.Total(tbname, bean, restrains);
            }
        }
        /// <summary>
        /// 查询数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public int Total<T>(String sql,params Object[] paramValues)
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.Total(sql,paramValues.ToArray());
            }
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
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.ExecuteDataTable(sql, start, maxResult, paramValues);
            }
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
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.QueryByPage(tbname, bean, page, rows, out total, restrains);
            }
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
        public List<dynamic> QueryByPage_Dy(string tbname, object bean, int page, int rows, out int total, params Restrain[] restrains)
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.QueryByPage_Dy(tbname, bean, page, rows, out total, restrains);
            }
        }


        /// <summary>
        /// 添加model对象到数据库中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbname"></param>
        /// <param name="model"></param>
        public void Add<T>(string tbname, T model)
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                db.Add(tbname, model);
            }
        }
        /// <summary>
        /// 添加数据库记录并设置自动生成的Id的值到model中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbname"></param>
        /// <param name="model"></param>
        public void Add_AutoSetId<T>(string tbname, T model)
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                db.Add_AutoSetId(tbname, model);
            }
        }
        /// <summary>
        /// 能够快速的增加多条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbname"></param>
        /// <param name="modelList"></param>
        public void Add<T>(string tbname, List<T> modelList)
        {
            using(BaseDatabase db = CreateDatabaseAndOpen())
            {
                db.Add<T>(tbname, modelList);
            }
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
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.Del(tbname, bean, restrains);
            }
        }
        /// <summary>
        /// 根据id删除对象
        /// </summary>
        /// <param name="tbname"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DelById(string tbname, int id)
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.DelById(tbname, id);
            }

        }
        /// <summary>
        /// 修改Id为model.Id的数据
        /// bug?无法将int类型修改为0,无法将DateTime类型修改为DateTime.minValue....
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbname"></param>
        /// <param name="model"></param>
        public void Modify<T>(string tbname, T model)
        {
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                db.Modify(tbname, model);

            }
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
            using (BaseDatabase db = CreateDatabaseAndOpen())
            {
                return db.QueryById<T>(tbname, id);
            }
        }

    }
}
