using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ZUtil
{
    /// <summary>
    /// 密码工具类
    /// </summary>
    public class PasswordUtil
    {

        /// <summary>
        /// base64加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns></returns>
        public static String base64Encoded(String str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// base64解密
        /// </summary>
        /// <param name="str">需要解密的字符串</param>
        /// <returns></returns>
        public static String base64Decode(String str)
        {
            byte[] bytes = Convert.FromBase64String(str);
            return Encoding.Default.GetString(bytes);
        }


        /// <summary>
        /// md5 32加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns></returns>
        public static String md532(String str)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(str));

            StringBuilder sBuilder = new StringBuilder();
            //将每个字节转为16进制
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private static String zEncryptKey = "zhangwen";
        private static Regex reg_ZPwd = new Regex(@"^ZPwd\[(.*)\]$");
        public static String ZEncrypt(String str)
        {
            if (reg_ZPwd.IsMatch(str))
            {
                return str;
            }
            return "ZPwd[" + EncryptDES(str, zEncryptKey) + "]";
        }
        public static String ZDecrypt(String str)
        {
            Match m = reg_ZPwd.Match(str);
            if (m.Success)
            {
                return DecryptDES(m.Groups[1].Value, zEncryptKey);
            }
            else
            {
                return str;
            }
        }


        private static String youoEncryptKey = "youotech";
        private static Regex reg_YouoPwd = new Regex(@"^YouoPwd\[(.*)\]$");

        /// <summary>
        /// Youo加密字符串,使用YouoDecrypt可以解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string YouoEncrypt(String str)
        {
            if (reg_YouoPwd.IsMatch(str))
            {
                return str;
            }
            return "YouoPwd["+EncryptDES(str, youoEncryptKey)+"]";
        }
        /// <summary>
        /// Youo解密字符串
        /// 解密由YouoEncrypt(String)方法加密的字符串,如果不能由YouoEncrypt方法加密的字符串将原字符串返回
        /// </summary>
        /// <param name="str">需要解密的字符串,需要是YouoEncrypt(String)加密的字符串</param>
        /// <returns></returns>
        public static string YouoDecrypt(string str)
        {
            Match m = reg_YouoPwd.Match(str);
            if (m.Success)
            {
                return DecryptDES(m.Groups[1].Value, youoEncryptKey);
            }
            else
            {
                return str;
            }
        }


         //默认密钥向量
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
      /**//// <summary>
      /// DES加密字符串
      /// </summary>
      /// <param name="encryptString">待加密的字符串</param>
      /// <param name="encryptKey">加密密钥,要求为8位</param>
      /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
      public static string EncryptDES(string encryptString, string encryptKey)
      {
          try
          {           
              byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
              byte[] rgbIV = Keys;
              byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
               DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
               MemoryStream mStream = new MemoryStream();
               CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
               cStream.Write(inputByteArray, 0, inputByteArray.Length);
               cStream.FlushFinalBlock();
              return Convert.ToBase64String(mStream.ToArray());
           }
          catch
          {
              return encryptString;
          }
      }
  
      /**//// <summary>
      /// DES解密字符串
      /// </summary>
      /// <param name="decryptString">待解密的字符串</param>
      /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
      /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
      public static string DecryptDES(string decryptString, string decryptKey)
      {
          try
          {
              byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
              byte[] rgbIV = Keys;
              byte[] inputByteArray = Convert.FromBase64String(decryptString);
               DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
               MemoryStream mStream = new MemoryStream();
               CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
               cStream.Write(inputByteArray, 0, inputByteArray.Length);
               cStream.FlushFinalBlock();
              return Encoding.UTF8.GetString(mStream.ToArray());
           }
          catch
           {
              return decryptString;
          }
     }




    }
}
