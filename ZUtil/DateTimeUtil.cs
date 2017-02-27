using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZUtil
{
    public class DateTimeUtil
    {
        /// <summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>long</returns>
        public static long ConvertDateTimeUnixLong(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000 / 1000;
            return t;
        }
        /// <summary>
        /// 将Unix时间戳格式转换为c#DateTime时间格式
        /// </summary>
        /// <param name="clock"></param>
        /// <returns></returns>
        public static DateTime ConvertUnixLongDateTime(long clock)
        {

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            return new DateTime(clock * 10000000 + startTime.Ticks);
        }
    }
}
