using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZUtil
{
    public class ByteUtil
    {

        /// <summary>
        /// 将byte数组转换为十六进制字符串形式
        /// </summary>
        /// <param name="InBytes"></param>
        /// <returns></returns>
        public static String byteToString(byte[] InBytes)
        {
            return byteToString(InBytes, InBytes.Length);
        }
        /// <summary>
        /// 将byte数组转换为十六进制字符串形式
        /// </summary>
        /// <param name="InBytes"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static String byteToString(byte[] InBytes, int len)
        {
            string StringOut = "";
            for (int i = 0; i < len; i++)
            {
                StringOut = StringOut + String.Format("{0:X2} ", InBytes[i]);
            }
            return StringOut;
        }
        /// <summary>
        /// crc16_modbus加密(x16+x15+x2+1)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] crc16_modbus(byte[] data)
        {
            //常数
            int Const = 0xa001;
            //设置CRC寄存器初始值
            int crc = 0xffff;
            for (int i = 0; i < data.Length; i++)
            {
                //CRC与命令字节异或
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((int)(crc & 0x1) == 1) //二进制末尾是1,进行右移和常数异或运算
                    {
                        crc >>= 1;
                        crc ^= Const;
                    }
                    else //二进制末尾为0,仅仅进行右移运算
                    {
                        crc >>= 1;
                    }
                }
            }
            byte[] result = new byte[2];
            result[0] = (byte)(crc >> 8);
            result[1] = (byte)(crc & 0xFF);
            return result;
        }

    }
}
