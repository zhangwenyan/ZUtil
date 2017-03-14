using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using easysql;

namespace eweb
{
    public class ResultDic:Dictionary<String,Object>
    {
        public static ResultDic data(String key,Object value)
        {
            var rd = new ResultDic();
            rd.Add(key, value);
            return rd;
        }
        /// <summary>
        /// 将该obj的参数转换为ResultDic对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ResultDic p(Object obj)
        {
            var pis = obj.GetType().GetProperties();
            foreach(var pi in pis)
            {
                var value = pi.GetValue(obj, null);
                this.p(pi.Name, value);
            }
            return this;
        }

        public ResultDic p(String key,Object value)
        {
            this.Add(key, value);
            return this;
        }



    }
}
