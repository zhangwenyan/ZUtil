using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
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
            String val = ConfigurationManager.AppSettings[key] ?? def;
            if (val == null)
            {
                return null;
            }
            else
            {
                return PasswordUtil.YouoDecrypt(val);
            }
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

        public  static void changeSetting(String key, String value)
        {
            //调用  
            string assemblyConfigFile = Assembly.GetEntryAssembly().Location;
            Configuration config = ConfigurationManager.OpenExeConfiguration(assemblyConfigFile);    //获取appSettings节点 
            AppSettingsSection appSettings = (AppSettingsSection)config.GetSection("appSettings");
            //删除name，然后添加新值  
            appSettings.Settings.Remove(key);
            appSettings.Settings.Add(key, value);
            //保存配置文件  
            config.Save();
        }



        public static void changeSettingByPwd(String key,String value)
        {
            changeSetting(key, PasswordUtil.YouoEncrypt(value));
        }

    }
}
