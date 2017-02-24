using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZUtil
{
    public static class DllUtil
    {
        public static object execute(String filename,String namespaceAndClassName,String methodName,object[] ps)
        {
            if (filename == null)
            {
                throw new Exception("filename cannot null");
            }
            Assembly assembly = null;
            try
            {
                String dPath = filename;
                

                if (!Path.IsPathRooted(filename))
                {
                    dPath = AppDomain.CurrentDomain.BaseDirectory + filename;
                }

                if (!File.Exists(dPath))
                {
                    dPath = AppDomain.CurrentDomain.BaseDirectory +"bin\\"+ filename;
                }

                if (!File.Exists(dPath))
                {
                    throw new Exception("缺少" + filename + "文件");
                }


                assembly = Assembly.LoadFile(dPath); 
            }
            catch (Exception ex)
            {
                throw ex;
            }


            var type = assembly.GetType(namespaceAndClassName);
            var method = type.GetMethod(methodName);
            if (method == null)
            {
                throw new Exception("method:" + methodName + " not exist");
            }
            return method.Invoke(null, ps);
        }

    }
}
