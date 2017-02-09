using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZUtil
{
    /// <summary>
    /// ip工具类
    /// </summary>
    public class IpUtil
    {
        /// <summary>
        /// 将ip转换为长整形数据
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long ip2long(String ip)
        {
            String[] ips = ip.Split('.');
            long num = 16777216L * long.Parse(ips[0]) + 65536L * long.Parse(ips[1]) + 256 * long.Parse(ips[2]) + long.Parse(ips[3]);
            return num;
        }

        /// <summary>
        /// 将长整形ip转换成普通ip形式
        /// </summary>
        /// <param name="ipLong"></param>
        /// <returns></returns>
        public static String long2ip(long ipLong)
        {
            //long ipLong = 1037591503;   
            long[] mask = new long[] { 0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000 };
            long num = 0;
            StringBuilder ipInfo = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                num = (ipLong & mask[i]) >> (i * 8);
                if (i > 0) ipInfo.Insert(0, ".");
                ipInfo.Insert(0, long.Parse(Convert.ToString(num, 10)));
            }
            return ipInfo.ToString();
        }
    }
}
