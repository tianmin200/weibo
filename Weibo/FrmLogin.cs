using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Weibo.Common;

namespace Weibo
{
    public partial class FrmLogin : Form
    {
        public string UserName = "";
        public string Pwd = "";
        public string url = "";
        public FrmLogin(string username,string pwd,string url)
        {
            InitializeComponent();
            UserName = username;
            Pwd = pwd;
            this.webBrowser1.Url = new Uri(url);
        }
        public CookieContainer cc;
        private void btnOK_Click(object sender, EventArgs e)
        {
            //获取Cookie
            CookieContainer myCookieContainer = new CookieContainer();
            string cookieStr = CookieReader.GetGlobalCookies(webBrowser1.Document.Url.AbsoluteUri);
            string domain = "alimama.com";

            //string domian = "alimama.com";
            //File.AppendAllText("cookie.txt", UserName + "\t" + cookieStr + "\r\n");//记录异常账号，下一次不再重复访问
            if (File.Exists("alimamacookie.txt"))
                File.Delete("alimamacookie.txt");
            File.AppendAllText("alimamacookie.txt", cookieStr);//1.8 修改只记录阿里妈妈登陆cookie
            CookieCollection ccl = CookieHelper.GetCookieCollectionByString(cookieStr, domain);
            myCookieContainer.Add(ccl);
            cc = myCookieContainer;
            this.DialogResult = DialogResult.OK;
        }


        
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlElement txtqq = this.webBrowser1.Document.GetElementById("TPL_username_1");
            //txtqq.InnerText = UserName;
            HtmlElement txtpwd = this.webBrowser1.Document.GetElementById("TPL_password_1");
            //txtpwd.InnerText = Pwd;
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}