﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using ZUtil.ex;

namespace ZUtil
{
    /// <summary>
    /// 短信工具类
    /// </summary>
    public static class SmsUtil
    {
        public static readonly String smsSendWay = ConfigUtil.readSetting("smsSendWay_zutil", "db");

        /// <summary>
        /// 发送短信
        /// 需要配置短信数据库连接字符串connStr_smsdb
        /// <add key="connStr_smsdb" value="server=192.168.1.223;database=smsdb;Persist Security Info=False;uid=root;pwd=123456"/>
        /// </summary>
        /// <param name="mbmo">接收短信的手机号</param>
        /// <param name="msg">短信内容</param>
        public static void sendSms(String mbno,String msg)
        {
            sendSms(mbno, msg, DateTime.MinValue);
        }
        /// <summary>
        /// 指定时间发送短信
        /// 需要配置短信数据库连接字符串connStr_smsdb dbType_smsdb
        /// <add key="connStr_smsdb" value="server=192.168.1.223;database=smsdb;Persist Security Info=False;uid=root;pwd=123456"/>
        /// </summary>
        /// <param name="mbno">接收短信的手机号</param>
        /// <param name="msg">短信内容</param>
        /// <param name="dt">发送短信的时间</param>
        public static void sendSms(String mbno,String msg,DateTime dt)
        {

       
            if (smsSendWay == "db")
            {
                String connStr_smsdb = ConfigUtil.readSetting("connStr_smsdb");
                String dbType = ConfigUtil.readSetting("dbType_smsdb");
                sendSmsByConnStr_smsdb(mbno, msg, dt, connStr_smsdb, dbType);
            }
            else if(smsSendWay == "zsms")
            {
                try
                {
                    DllUtil.execute("zsms.dll", "zsms.SmsMethod", "sendSms", new object[] { mbno, msg });
                }catch(Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        throw ex.InnerException;
                    }
                    throw ex;
                }
            }
            else
            {
                throw new SmsUtilException("unknow smsSendWay");
            }

        }
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mbno"></param>
        /// <param name="msg"></param>
        /// <param name="conStr_smsdb">短信数据库smsdb连接字符串</param>
        public static void sendSmsByConnStr_smsdb(String mbno,String msg,String conStr_smsdb)
        {
            sendSmsByConnStr_smsdb(mbno, msg, DateTime.MinValue, conStr_smsdb);
        }
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mbno"></param>
        /// <param name="msg"></param>
        /// <param name="dt"></param>
        /// <param name="connStr_smsdb">短信数据库smsdb连接字符串,mysql数据库</param>
        public static void sendSmsByConnStr_smsdb(String mbno,String msg,DateTime dt,String connStr_smsdb)
        {
            sendSmsByConnStr_smsdb(mbno, msg, dt, connStr_smsdb, "mysql");
        }

        public static void sendSmsByConnStr_smsdb(String mbno, String msg, DateTime dt, String connStr_smsdb,String dbType)
        {
            if (String.IsNullOrEmpty(dbType))
            {
                dbType = "mysql";
            }

            if (String.IsNullOrEmpty(connStr_smsdb))
            {
                throw new SmsUtilException("没有配置短信数据库连接字符串connStr_smsdb");
            }


            var dh = easysql.DBHelperFactory.Create(dbType, connStr_smsdb);
            if (dt != DateTime.MinValue)
            {
                dh.Execute("insert into OutBox(mbno, msg,sendTime) values({0},{1},{2})", mbno, msg, dt);
            }
            else
            {
                dh.Execute("insert into OutBox(mbno, msg) values({0},{1})", mbno, msg);
            }

        }




        }
}
