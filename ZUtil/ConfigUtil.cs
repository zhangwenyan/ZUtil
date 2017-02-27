using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ZUtil
{
    public static class ConfigUtil
    {
        /// <summary>
        /// 读取配置文件的内容,如果内容是加密的,将会返回解密后的结果
        /// </summary>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static String readSetting(String key,String def)
        {
            return PasswordUtil.YouoDecrypt(ConfigurationManager.AppSettings[key] ?? def);
        }
        /// <summary>
        /// 读取配置文件的内容,如果内容是加密的,将会返回解密后的结果
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String readSetting(String key)
        {
            return readSetting(key, null);
        }

    }
}
