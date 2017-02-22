using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ZUtil;
namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // var str = Path.GetFileName("aaa.txt");

            //  var t = DllUtil.execute("zsms.dll", "zsms.SmsMethod", "sendSms", new object[] { "17681109309","test 123333" });

            SmsUtil.sendSms("17681109309", "1234567890");



        }
    }
}
