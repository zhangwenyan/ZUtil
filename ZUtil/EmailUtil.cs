using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ZUtil
{
    /// <summary>
    /// 邮件操作工具类
    /// </summary>
    public static class EmailUtil
    {

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="receivers">接收邮件的人</param>
        /// <param name="title">邮件标题</param>
        /// <param name="msg">邮件内容</param>
        /// <param name="emailAccount">发件人账号</param>
        /// <param name="emailPassword">发件人密码</param>
        /// <param name="smtpServer">邮件服务器地址</param>
        /// <param name="smtpPort">邮件服务器端口</param>
        /// <param name="sslEnable">是否启用ssl安全(qq邮件必须为true,电力内网可能需要false)</param>
        public static void sendEmail(String receivers, String title, String msg,String emailAccount, String emailPassword,String smtpServer,int smtpPort,bool sslEnable)
        {
            MailMessage message = new MailMessage();
            //设置发件人,发件人需要与设置的邮件发送服务器的邮箱一致
            MailAddress fromAddr = new MailAddress(emailAccount);
            message.From = fromAddr;
            //设置收件人,可添加多个,添加方法与下面的一样
            String[] receiverArr = receivers.Trim().Split(',');
            foreach (var receiver in receiverArr)
            {
                message.To.Add(receiver);
            }
            //设置抄送人
            //message.CC.Add("630288535@qq.com");
            //设置邮件标题
            message.Subject = title;
            //设置邮件内容
            message.Body = msg;
            //设置邮件发送服务器,服务器根据你使用的邮箱而不同,可以到相应的 邮箱管理后台查看,下面是QQ的
            SmtpClient client = new SmtpClient(smtpServer, smtpPort);
            //设置发送人的邮箱账号和密码
            client.Credentials = new NetworkCredential(emailAccount, emailPassword);
            //启用ssl,也就是安全发送
            client.EnableSsl = sslEnable;
            //发送邮件
            client.Send(message);

        }

        /// <summary>
        ///使用配置的账号发送邮件
        ///账号:emailAccount,
        ///密码:emailPassword
        ///邮件服务器:smtpServer (例如:smtp.qq.com)
        ///邮件服务器端口(默认25):smtpPort
        ///是否启用用ssl(默认false):sslEnable
        ///
        /// 配置文件示例:
        //<add key = "emailAccount" value="sxyth@ah.sgcc.com.cn"/>
        //<add key = "emailPassword" value="admin123456"/>
        //<add key = "smtpServer" value="10.1.182.251"/>
        //<add key = "smtpPort" value="25"/>
        //<add key = "sslEnable" value="false"/>
        /// </summary>
        /// <param name="receivers">接收邮件的人</param>
        /// <param name="title">邮件标题</param>
        /// <param name="msg">邮件内容</param>
        public static void sendEmail(String receivers, String title, String msg)
        {
            var emailAccount = ConfigUtil.readSetting("emailAccount");
            var emailPassword = ConfigUtil.readSetting("emailPassword");
            var smtpServer = ConfigUtil.readSetting("smtpServer");
            var smtpPort = int.Parse(ConfigUtil.readSetting("smtpPort","25"));
            var sslEnable = bool.Parse(ConfigUtil.readSetting("sslEnable", "false"));

            if (String.IsNullOrWhiteSpace(emailAccount) || String.IsNullOrWhiteSpace(emailPassword) || String.IsNullOrWhiteSpace(smtpServer))
            {
                    throw new Exception("请配置emailAccount,emailPassword,smtpServer");
            }

            sendEmail(receivers, title, msg, emailAccount, emailPassword, smtpServer, smtpPort, sslEnable);
           
        }

    }
}
