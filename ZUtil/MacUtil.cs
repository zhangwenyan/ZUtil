using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZUtil
{
    public class MacUtil
    {
        /// <summary>
        /// 将mac地址统一成aa:bb:cc:dd:ee:ff的形式
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        public static String formatMac(String mac)
        {
            if (mac == null || mac.Length < 12)
            {
                return "";
            }
            mac = mac.Replace("-", ":");
            mac = String.Join(":", mac.Split(' ')).ToLower();
            return mac;
        }
    }
}
