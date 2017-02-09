using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PasswordUtil
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txt_youPwd_msg.Text = ZUtil.PasswordUtil.YouoEncrypt(txt_youoPwd_password.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txt_youPwd_msg.Text = ZUtil.PasswordUtil.YouoDecrypt(txt_youoPwd_password.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(txt_youPwd_msg.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Text = ZUtil.PasswordUtil.md532(textBox1.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox5.Text = ZUtil.PasswordUtil.EncryptDES(textBox3.Text, textBox4.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox5.Text = ZUtil.PasswordUtil.DecryptDES(textBox3.Text, textBox4.Text);
        }
    }
}
