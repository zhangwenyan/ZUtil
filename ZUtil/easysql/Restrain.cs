using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace easysql
{
    public enum RestrainType
    {
        /// <summary>
        /// 介于两者之间
        /// </summary>
        between,
        /// <summary>
        /// 等于
        /// </summary>
        eq,
        /// <summary>
        /// 大于
        /// </summary>
        lt,
        //小于
        gt,
        /// <summary>
        /// 不是
        /// </summary>
        not,
        /// <summary>
        /// in为保留词，所以只能用inc代替，包含
        /// </summary>
        inc,
        /// <summary>
        /// 不包括
        /// </summary>
        notin,
        /// <summary>
        /// 正序排列
        /// </summary>
        order,
        /// <summary>
        /// 倒序排列
        /// </summary>
        orderdesc,
        /// <summary>
        /// 模糊查询
        /// </summary>
        like,
        
        /// <summary>
        /// 增加一个约束语句比如值可以是name like '%111%'
        /// </summary>
        add,
    }

    public class Restrain
    {
        /// <summary>
        /// 数据库字段名,没有防止sql注入
        /// </summary>
        public string Key { get; set; }
        public object[] Values { get; set; }
        public RestrainType RestrainType { get; set; }

        public Restrain()
        {
        }
        public Restrain(string key, object[] values, RestrainType restrainType)
            : this()
        {
            this.Key = key;
            this.Values = values;
            this.RestrainType = restrainType;
        }

        public static Restrain Between(string key, object lo, object hi)
        {
            return new Restrain(key, new object[] { lo, hi }, RestrainType.between);
        }
        public static Restrain Eq(string key, object value)
        {
            return new Restrain(key, new object[] { value }, RestrainType.eq);
        }
        public static Restrain Lt(string key, object value)
        {
            return new Restrain(key, new object[] { value }, RestrainType.lt);
        }
        public static Restrain Gt(string key, object value)
        {
            return new Restrain(key, new object[] { value }, RestrainType.gt);
        }
        public static Restrain Not(string key, object value)
        {
            return new Restrain(key, new object[] { value }, RestrainType.not);
        }
        public static Restrain In(string key, object[] values)
        {
            return new Restrain(key, values, RestrainType.inc);
        }
        public static Restrain NotIn(string key, object[] values)
        {
            return new Restrain(key, values, RestrainType.notin);
        }

        public static Restrain Order(string key)
        {
            return new Restrain(key, null, RestrainType.order);
        }
        public static Restrain OrderDesc(string key)
        {
            return new Restrain(key, null, RestrainType.orderdesc);
        }
        public static Restrain Like(string key, string value)
        {
            return new Restrain(key, new object[] { value }, RestrainType.like);
        }
        public static Restrain Add(string value)
        {
            return new Restrain(null, new object[] { value }, RestrainType.add);
        }

    }
}
