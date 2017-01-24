using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Weibo.Common;

namespace Weibo
{
    public partial class FrmVcode : Form
    {
        public string QQ = "";
        public string vc_type = "";
        public string vcode = "";
        public string retcode = "";
        public CookieContainer cc = new CookieContainer();
        public string refer = "";
        public FrmVcode(string qq,string vctype,CookieContainer cookiec)
        {
            InitializeComponent();
            QQ = qq;
            vc_type = vctype;
            cc = cookiec;
        }
        public FrmVcode(CookieContainer cookiec,string refer1)
        {
            InitializeComponent();
            cc = cookiec;
            refer = refer1;
           
        }


        private void FrmVcode_Load(object sender, EventArgs e)
        {
            this.Focus();
            
            GetVcodePic();

        }
        public void GetVcodePic()
        {
            ////生成图片验证码   
            ////生成随机数列
            CookieWebClient client = new CookieWebClient();

            client.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
            client.Headers.Add("Accept-Language", "zh-CN");
            client.Headers.Add("Accept", "*/*");
            
            client.Headers.Add("Accept-Encoding", "gzip, deflate");
            
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1);
            client.Cookies = cc;//带Cookie访问
            string ticks = ts.Ticks.ToString().Substring(0, 13);

            byte[] bytes = client.DownloadData("http://weibo.com/aj/pincode/pin?_wv=5&type=rule&lang=zh-cn&ts=" + ticks);


            MemoryStream ms = new MemoryStream(bytes); // MemoryStream创建其支持存储区为内存的流。           
            //MemoryStream属于System.IO类
            ms.Position = 0;
            Image img = Image.FromStream(ms);
            this.pictureBox1.Image = img;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            vcode = this.txtVcode.Text;
            string yanzhengurl = "http://weibo.com/aj/pincode/verified?ajwvr=6";
            string yanzhengpost = "secode=" + vcode + "&type=rule";
            string yanzhengresult = HttpHelper1.SendDataByPost(yanzhengurl, yanzhengpost, refer, ref cc);
            if (yanzhengresult.Contains("{\"code\":\"100000\""))
            {
                //验证通过
                
                HttpHelper1.GetStringInTwoKeyword(yanzhengresult,ref retcode, "retcode\":\"", "\"}}",0);
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                GetVcodePic();
            }
            
        }

        private void btn_Refresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GetVcodePic();
        }
    }
}