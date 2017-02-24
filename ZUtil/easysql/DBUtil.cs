using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;

namespace easysql
{
    /// <summary>
    /// 生成sql语句的工具类
    /// </summary>
    public static class DBUtil
    {
        /// <summary>
        /// 查询语句的转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bean"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="sqlOrder"></param>
        /// <param name="paramValues"></param>
        /// <param name="i"></param>
        /// <param name="restrains"></param>
        public static void QueryBean<T>(T bean, StringBuilder sqlWhere, StringBuilder sqlOrder, List<Object> paramValues, ref int i, params Restrain[] restrains)
        {
            if (bean != null && sqlWhere != null)
            {
                //根据bean找出约束
                Type type = bean.GetType();

                PropertyInfo[] pis = type.GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    String name = pi.Name;
                    Object value = pi.GetValue(bean, null);
                    var pType = pi.PropertyType;
                    if (value == null || value.Equals(DefaultForType(pType)))
                    {
                        //没有赋值或值为原始值，跳过
                        continue;
                    }
                    if (pType.Equals(typeof(String)))
                    {
                        if (value.ToString().Length != 0)
                        {
                            //如果是字符串类型，则加入like约束
                            sqlWhere.AppendFormat(" and {0} like {{{1}}}", name, i++);
                            paramValues.Add("%" + value + "%");
                        }
                    }
                    else if (pType.IsValueType)
                    {
                        //如果是值类型,则加入等于约束
                        sqlWhere.AppendFormat(" and {0}={{{1}}}", name, i++);
                        paramValues.Add(value);
                    }

                }
            }

            foreach (var restrain in restrains)
            {
                if (sqlWhere != null)
                {
                    switch (restrain.RestrainType)
                    {
                        case RestrainType.between:
                            var start = restrain.Values[0];
                            var end = restrain.Values[1];
                            if (start != null && !start.Equals(DefaultForType(start.GetType())))
                            {
                                sqlWhere.AppendFormat(" and {0}>={{{1}}}", restrain.Key, i++);
                                paramValues.Add(start);
                            }
                            if (end != null && !end.Equals(DefaultForType(end.GetType())))
                            {
                                sqlWhere.AppendFormat(" and {0}<={{{1}}}", restrain.Key, i++);
                                paramValues.Add(end);
                            }
                            break;
                        case RestrainType.inc:
                        case RestrainType.notin:
                            var arr = new List<String>();
                            foreach (var obj in restrain.Values)
                            {
                                arr.Add("'" + obj.ToString().Replace("'", "''") + "'");
                            }
                            String ci = " in ";
                            if (restrain.RestrainType == RestrainType.notin)
                            {
                                ci = " not in ";
                            }
                            sqlWhere.Append(" and " + restrain.Key + " " + ci + " (" + string.Join(",", arr) + ")");
                            break;
                        case RestrainType.eq:
                            sqlWhere.AppendFormat(" and {0}={{{1}}}", restrain.Key, i++);
                            paramValues.Add(restrain.Values[0]);
                            break;
                        case RestrainType.lt:
                            sqlWhere.AppendFormat(" and {0}<{{{1}}}", restrain.Key, i++);
                            paramValues.Add(restrain.Values[0]);
                            break;
                        case RestrainType.gt:
                            sqlWhere.AppendFormat(" and {0}>{{{1}}}", restrain.Key, i++);
                            paramValues.Add(restrain.Values[0]);
                            break;
                        case RestrainType.not:
                            sqlWhere.AppendFormat(" and {0} != {{{1}}}", restrain.Key, i++);
                            paramValues.Add(restrain.Values[0]);
                            break;
                        case RestrainType.like:
                            sqlWhere.AppendFormat(" and {0} like {{{1}}}", restrain.Key, i++);
                            paramValues.Add(restrain.Values[0]);
                            break;

                        case RestrainType.add:
                            sqlWhere.Append(" and " + restrain.Values[0] + " ");
                            break;

                    }


                }

                if (sqlOrder != null)
                {
                    switch (restrain.RestrainType)
                    {
                        case RestrainType.order:
                        case RestrainType.orderdesc:
                            if (sqlOrder.Length == 0)
                            {
                                sqlOrder.Append("order by " + restrain.Key);
                            }
                            else
                            {
                                sqlOrder.Append("," + restrain.Key);
                            }
                            if (restrain.RestrainType == RestrainType.orderdesc)
                            {
                                sqlOrder.Append(" desc");
                            }
                            break;
                    }

                }

            }

        }

        /// <summary>
        /// 查询语句的转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bean"></param>
        /// <param name="sqlWhere"></param>
        /// <param name="sqlOrder"></param>
        /// <param name="list"></param>
        /// <param name="restrains"></param>
        public static void QueryBean<T>(T bean, StringBuilder sqlWhere, StringBuilder sqlOrder, List<object> list, params Restrain[] restrains)
        {
            var i = 0;
            QueryBean(bean, sqlWhere, sqlOrder, list, ref i, restrains);
        }

        public static List<T> ToList<T>(DataTable dt) where T : new()
        {
            var result = new List<T>();
            //创建一个属性的列表
            var prlist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口
            var t = typeof(T);

            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表 
            Array.ForEach(t.GetProperties(), p =>
            {
                if (dt.Columns.Contains(p.Name))
                {
                    prlist.Add(p);
                }
            });

            foreach (DataRow dr in dt.Rows)
            {

                var model = ToModel<T>(dr);
                result.Add(model);
            }

            return result;
        }

        public static T ToModel<T>(DataRow dr) where T : new()
        {
            var model = new T();
            Type type = model.GetType();
            PropertyInfo[] prlist = type.GetProperties();
            foreach (var p in prlist)
            {
                object val = dr[p.Name];
                if (val != DBNull.Value)
                {
                    var valType = val.GetType();
                    var pType = p.PropertyType;
                    if (valType != pType)
                    {//类型转换

                        if (valType.Equals(typeof(Int64)) && pType.Equals(typeof(Int32)))
                        {
                            val = (int)(long)val;
                        }
                        else if (valType.Equals(typeof(String)) && pType.Equals(typeof(DateTime)))
                        {
                            val = DateTime.Parse(val.ToString());
                        }
                    }

                    p.SetValue(model, val, null);
                }
            }
            return model;

        }

        /// <summary>
        /// DateTable转换成List<dynamic> 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<dynamic> ToDynamic(DataTable dt)
        {
            var dynamicDt = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new System.Dynamic.ExpandoObject();
                var dic = (IDictionary<string, object>)dyn;
                foreach (DataColumn column in dt.Columns)
                {
                    dic[column.ColumnName] = row[column];
                }
                dynamicDt.Add(dyn);
            }
            return dynamicDt;
        }


        /// <summary>
        /// 该方法可以将数据库中string类型转换为DateTime类型或int类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ToModel2<T>(DataRow dr) where T : new()
        {
            var result = new T();
            Type type = result.GetType();
            PropertyInfo[] pis = type.GetProperties();
            foreach (var p in pis)
            {
                object val = null;
                try
                {
                    val = dr[p.Name];
                }
                catch
                {
                    //如果dr里面没有该列，则跳过
                    continue;
                }
                if (val != DBNull.Value)
                {

                    if (p.PropertyType.Equals(typeof(DateTime)))
                    {
                        if (val.ToString().Length == 0)
                        {
                            continue;
                        }

                        val = DateTime.Parse(val.ToString());
                    }
                    else if (p.PropertyType.Equals(typeof(int)))
                    {
                        if (val.ToString().Length == 0)
                        {
                            continue;
                        }
                        val = int.Parse(val.ToString());
                    }

                    p.SetValue(result, val, null);
                }

            }
            return result;
        }

        public static object ReadProValue(object obj, string proName)
        {
            Type type = obj.GetType();
            object value = type.GetProperty(proName).GetValue(obj, null);
            return value;
        }

        public static object DefaultForType(Type targetType)
        {
            return !targetType.IsValueType || IsNullableType(targetType) ? null : Activator.CreateInstance(targetType);
        }
        /// <summary>
        /// 判断一个类型是否可以为null
        /// </summary>
        /// <param name="theType"></param>
        /// <returns></returns>
        public static bool IsNullableType(Type theType)
        {
            return (theType.IsGenericType && theType.
              GetGenericTypeDefinition().Equals
              (typeof(Nullable<>)));
        }


    }
}