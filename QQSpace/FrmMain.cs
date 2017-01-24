using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Weibo.Common;
using Weibo.Core;
using Weibo.Models;

namespace QQSpace
{

    public partial class FrmMain : Form
    {
        public CookieContainer spacecc;
        public Thread colThread;
        public Thread spaceThread;
        public ArrayList qqhaos;
        public FrmMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            ArrayList parms = new ArrayList();
            parms.Add("");
            parms.Add(1);
            spaceThread = new Thread(new ParameterizedThreadStart(StartSend));
            spaceThread.Start(parms);
            //QQSpaceHelper.SendShuoshuo("哈哈哈", spacecc, qqhao);//发布文字说说，测试成功

            //ArrayList imgs = new ArrayList();
            //UploadImageData imagedata = QQSpaceHelper.UploadPic("1.jpg",spacecc,qqhao);//上传图片，测试成功

            //UploadImageData imagedata2 = QQSpaceHelper.UploadPic("2.jpg", spacecc, qqhao);//上传图片，测试成功

            //imgs.Add(imagedata);
            //imgs.Add(imagedata2);

            //QQSpaceHelper.SendShuoshuoWithPic("哈哈哈",imgs,spacecc,qqhao);

        }
        public void StartSend(object parms)
        {
            while (true)
            {
                SetSpaceeButtonStatus(true);
                AppenSpaceCmd("开始发布说说");
                #region 登录账号并初始化
                string[] qqlist = File.ReadAllLines("qq.txt");
                foreach (string qqstr in qqlist)
                {
                    string[] qq = qqstr.Split(',');
                    string qqhaoma = qq[0];
                    string password = qq[1];
                    AppenSpaceCmd(qqhaoma + "发布");
                    string cookiepath = "cookie/" + qqhaoma + ".txt";
                    string token = "";
                    if (File.Exists(cookiepath))
                    {
                        spacecc = CookieHelper.ReadCookieFromFile(cookiepath);
                        token = QQSpaceHelper.GetToken(spacecc, qqhaoma);
                    }
                    int logintime = 0;
                    while (token == "")
                    {
                        logintime++;
                        if (logintime == 10)
                        {
                            AppenSpaceCmd("登录失败，放弃!");
                            break;
                        }
                        AppenSpaceCmd(qqhaoma + "未登录，正在第" + logintime.ToString() + "次尝试登录!");
                        spacecc = QQSpaceHelper.Login(qqhaoma, password);
                        CookieHelper.SaveCookie(cookiepath, spacecc);
                        token = QQSpaceHelper.GetToken(spacecc, qqhaoma);
                    }
                    if (token == "") continue;
                    AppenSpaceCmd(qqhaoma + "已登录");
                    string g_tk = QQSpaceHelper.Getgtk(spacecc.GetCookieHeader(new Uri("http://qzone.qq.com")).ToString());

                    QQHao qqhao = new QQHao();
                    qqhao.Qqhaoma = qqhaoma;
                    qqhao.Password = password;
                    qqhao.G_tk = g_tk;
                    qqhao.Token = token;
                    #endregion
                    DirectoryInfo TheFolder = new DirectoryInfo("待发布");
                    DirectoryInfo nextFolder = TheFolder.GetDirectories()[0];//发布选品库第一条
                    AppenSpaceCmd(qqhaoma+"发布正文：" + nextFolder.Name);
                    ArrayList imgs = new ArrayList();
                    foreach (FileInfo NextFile in nextFolder.GetFiles())
                    {
                        if (NextFile.Extension == ".jpg")
                        {
                            AppenSpaceCmd("上传图片");
                            UploadImageData imagedata = QQSpaceHelper.UploadPic(NextFile.FullName, spacecc, qqhao);
                            imgs.Add(imagedata);
                        }
                    }
                    QQSpaceHelper.SendShuoshuoWithPic(nextFolder.Name, imgs, spacecc, qqhao);
                    Directory.Move(nextFolder.FullName, "已发布/" + nextFolder.Name);
                    AppenSpaceCmd(qqhaoma + "发布完成");
                }
                int jiange = Convert.ToInt32(this.nud_spacejiange.Value);
                AppenSpaceCmd("此次发布完毕，等待" + jiange.ToString() + "分钟");
                Thread.Sleep(jiange * 60 * 1000);
            }
        }
        private void btn_col_Click(object sender, EventArgs e)
        {
            ArrayList parms = new ArrayList();
            parms.Add("");
            parms.Add(1);
            colThread = new Thread(new ParameterizedThreadStart(StartCol));
            colThread.Start(parms);
        }
        public void StartCol(object parms)
        {
            while (true)
            {

                SetColeButtonStatus(true);
                int pagenum = Convert.ToInt32(this.nud_Pages.Value);
                for (int i = 1; i < pagenum + 1; i++)
                {
                    AppenColCmd("开始抓取第" + i.ToString() + "页微博内容");

                    //string[] weiboAccounts = File.ReadAllLines("weiboAccounts.txt");

                    CookieContainer weibocc = new CookieContainer();
                    string[] urls = File.ReadAllLines(("colweibo.txt"));
                    foreach (string urltemp in urls)
                    {
                        string url = urltemp + i.ToString();
                        string ss = "";
                        string html = HttpHelper1.GetHttpsHtml(url, "", ref ss);
                        if (html.Contains(""))
                        {
                            html = html.Replace("page\":null", "page\":1");
                        }
                        MblogData mbloglist = Newtonsoft.Json.JsonConvert.DeserializeObject<MblogData>(html);
                        foreach (Card card in mbloglist.Cards)
                        {
                            if (card.Mblog == null) continue;
                            Mblog mblog = card.Mblog;
                            string mid = card.Mblog.Id;
                            try
                            {
                                int isHave = Convert.ToInt32(SQLiteHelper.ExecuteScalar("select count(*) from mblog where id=" + mid));
                                if (isHave > 0)
                                    continue;//如果数据已存在，则跳过

                                string dirname = HttpHelper1.NoHTML(mblog.Text).Trim();//截取选品文件夹名称
                                int picnum = 1;

                                dirname = dirname.Replace("\"", "");
                                if (!Directory.Exists("temp/" + dirname))
                                    Directory.CreateDirectory("temp/" + dirname);
                                foreach (Pic pic in mblog.Pics)
                                {
                                    AppenColCmd("下载图" + picnum.ToString());
                                    string savepicurl = "temp/" + dirname + "/" + picnum.ToString() + ".jpg";
                                    string picurl = "http://wx2.sinaimg.cn/large/" + pic.Pid + ".jpg";
                                    HttpHelper1.HttpDownloadFile(picurl, "temp/" + dirname + "/" + picnum.ToString() + ".jpg", weibocc);
                                    picnum++;
                                }
                                Directory.Move("temp/" + dirname, "待发布/" + dirname);
                                int staresult = SQLiteHelper.ExecuteNonQuery("insert into mblog(id,Source,Text,CreateAt,TbkLinks)values(@id,@Source,@Text,@CreateAt,@TbkLinks)", new[] {
                                                    mblog.Id,
                                                    mblog.Source,
                                                    mblog.Text,
                                                    mblog.CreatedAt,
                                                    ""
                                                 });
                                AppenColCmd("单条微博抓取完成！");
                            }
                            catch (Exception ex)
                            {
                                int staresult = SQLiteHelper.ExecuteNonQuery("insert into mblog(id,Source,Text,CreateAt,TbkLinks)values(@id,@Source,@Text,@CreateAt,@TbkLinks)", new[] {
                                                    mblog.Id,
                                                    mblog.Source,
                                                    mblog.Text,
                                                    mblog.CreatedAt,
                                                    ""
                                                 });
                                AppenColCmd(ex.Message);
                            }

                        }
                    }
                }
                int alimamajiange = Convert.ToInt32(this.nud_coljiange.Value);
                AppenColCmd("此次抓取完毕，等待" + alimamajiange.ToString() + "分钟");
                Thread.Sleep(alimamajiange * 60 * 1000);
            }
        }

        private void AppenColCmd(string info)
        {
            string loginfo = "";

            loginfo = DateTime.Now.ToString() + ":" + info + "\r\n";

            this.txtColCmd.AppendText(loginfo);
            if (!Directory.Exists("log/"))
                Directory.CreateDirectory("log");
            File.AppendAllText("log/col" + DateTime.Now.ToString("yyyy-MM-dd") + "-log.log", loginfo);
        }
        private void AppenSpaceCmd(string info)
        {
            string loginfo = "";

            loginfo = DateTime.Now.ToString() + ":" + info + "\r\n";

            this.txtSpaceCmd.AppendText(loginfo);
            if (!Directory.Exists("log/"))
                Directory.CreateDirectory("log");
            File.AppendAllText("log/space" + DateTime.Now.ToString("yyyy-MM-dd") + "-log.log", loginfo);
        }

        private void txtSpaceCmd_TextChanged(object sender, EventArgs e)
        {
            this.txtSpaceCmd.SelectionStart = txtSpaceCmd.Text.Length;
            txtSpaceCmd.ScrollToCaret();
        }

        private void txtColCmd_TextChanged(object sender, EventArgs e)
        {
            this.txtColCmd.SelectionStart = txtColCmd.Text.Length;
            txtColCmd.ScrollToCaret();
        }

        private void SetColeButtonStatus(bool isexe)
        {
            this.btn_col.Enabled = !isexe;
            this.btn_colStop.Enabled = isexe;
        }
        private void SetSpaceeButtonStatus(bool isexe)
        {
            this.btn_test.Enabled = !isexe;
            this.btn_spaceStop.Enabled = isexe;
        }

        private void btn_colStop_Click(object sender, EventArgs e)
        {
            if (colThread != null && colThread.IsAlive)
            {
                colThread.Abort();
                SetColeButtonStatus(false);
                AppenColCmd("用户停止抓取任务...");
            }
        }

        private void btn_spaceStop_Click(object sender, EventArgs e)
        {
            if (spaceThread != null && spaceThread.IsAlive)
            {
                colThread.Abort();
                SetSpaceeButtonStatus(false);
                AppenSpaceCmd("用户停止发布任务...");
            }
        }
    }
}
