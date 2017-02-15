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
        public Thread nhThread;
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
                    string nickname = qq[2];
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
                        AppenSpaceCmd(nickname+"["+qqhaoma + "]未登录，正在第" + logintime.ToString() + "次尝试登录!");
                        spacecc = QQSpaceHelper.Login(qqhaoma, password);
                        CookieHelper.SaveCookie(cookiepath, spacecc);
                        token = QQSpaceHelper.GetToken(spacecc, qqhaoma);
                    }
                    if (token == "") continue;
                    AppenSpaceCmd(nickname + "[" + qqhaoma + "]已登录");
                    string g_tk = QQSpaceHelper.Getgtk(spacecc.GetCookieHeader(new Uri("http://qzone.qq.com")).ToString());

                    QQHao qqhao = new QQHao();
                    qqhao.Qqhaoma = qqhaoma;
                    qqhao.Password = password;
                    qqhao.G_tk = g_tk;
                    qqhao.Token = token;

                    string result = QQSpaceHelper.GetUserInfo(qqhao, spacecc);
                    #endregion
                    DirectoryInfo TheFolder = new DirectoryInfo("待发布");
                    DirectoryInfo nextFolder = TheFolder.GetDirectories()[0];//发布选品库第一条
                    AppenSpaceCmd(nickname + "[" + qqhaoma + "]发布正文：" + nextFolder.Name);
                    ArrayList imgs = new ArrayList();
                    foreach (FileInfo NextFile in nextFolder.GetFiles())
                    {
                        if (NextFile.Extension == ".jpg" || NextFile.Extension == ".gif")
                        {
                            AppenSpaceCmd("上传图片");
                            UploadImageData imagedata = QQSpaceHelper.UploadPic(NextFile.FullName, spacecc, qqhao);
                            imgs.Add(imagedata);
                        }
                    }
                    string content = " ";
                    if (!IsGuidByReg(nextFolder.Name)) content = nextFolder.Name;
                    QQSpaceHelper.SendShuoshuoWithPic(content, imgs, spacecc, qqhao);
                    Directory.Move(nextFolder.FullName, "已发布/" + nextFolder.Name);
                    AppenSpaceCmd(nickname + "[" + qqhaoma + "]发布完成");
                }
                int jiange = Convert.ToInt32(this.nud_spacejiange.Value);
                AppenSpaceCmd("此次发布完毕，等待" + jiange.ToString() + "分钟");
                Thread.Sleep(jiange * 60 * 1000);
            }
        }
        static bool IsGuidByReg(string strSrc)
        {
            try
            {
                Guid guid = new Guid(strSrc);
                return true;
            }
            catch
            {
                return false;
            }
            //Regex reg = new Regex("^[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}$", RegexOptions.Compiled);
            //bool c = reg.IsMatch(strSrc);
            //log.Error("------------------------");
            //log.Error(c);
            //log.Error("------------------------");
            //return  c;
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
                                else
                                    continue;
                                foreach (Pic pic in mblog.Pics)
                                {
                                    AppenColCmd("下载图" + picnum.ToString());
                                    string[] strs = pic.Url.Split('.');
                                    string houzhui = strs[strs.Length - 1];
                                    string savepicurl = "temp/" + dirname + "/" + picnum.ToString() + "." + houzhui;
                                    string picurl = "http://wx2.sinaimg.cn/large/" + pic.Pid + "." + houzhui;
                                    HttpHelper1.HttpDownloadFile(picurl, "temp/" + dirname + "/" + picnum.ToString() + "." + houzhui, weibocc);
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
            this.btn_colweibo.Enabled = !isexe;
            this.btn_colStop.Enabled = isexe;
        }
        private void SetSpaceeButtonStatus(bool isexe)
        {
            this.btn_test.Enabled = !isexe;
            this.btn_spaceStop.Enabled = isexe;
        }
        private void SetNHButtonStatus(bool isexe)
        {
            this.btn_colnh.Enabled = !isexe;
            this.btn_nhStop.Enabled = isexe;
        }

        private void btn_colStop_Click(object sender, EventArgs e)
        {
            if (colThread != null && colThread.IsAlive)
            {
                colThread.Abort();
                SetColeButtonStatus(false);
                AppenColCmd("用户停止抓取任务!!!");
            }
        }

        private void btn_spaceStop_Click(object sender, EventArgs e)
        {
            if (spaceThread != null && spaceThread.IsAlive)
            {
                spaceThread.Abort();
                SetSpaceeButtonStatus(false);
                AppenSpaceCmd("用户停止发布任务!!!");
            }
        }

        private void btn_colnh_Click(object sender, EventArgs e)
        {
            //ThreadPool.QueueUserWorkItem(new WaitCallback(startColNH));
            ArrayList parms = new ArrayList();
            parms.Add("");
            parms.Add(1);
            nhThread = new Thread(new ParameterizedThreadStart(startColNH));
            nhThread.Start(parms);
        }

        /// <summary>
        /// 抓取内涵段子gif图片
        /// </summary>
        /// <param name="obj"></param>
        public void startColNH(object obj)
        {
            while (true)
            {
                SetNHButtonStatus(true);
                AppenColCmd("开始抓取内涵段子GIF图片");
                string url = "http://neihanshequ.com/pic/?is_json=1&app_name=neihanshequ_web&max_time=" + HttpHelper1.GetTicks() + ".0";
                CookieContainer cc = new CookieContainer();
                string resulthtml = HttpHelper1.SendDataByGET(url, ref cc);
                string jsonstr = "";
                HttpHelper1.GetStringInTwoKeyword(resulthtml, ref jsonstr, "\"data\": [", "], \"max_time\"", 0);
                jsonstr = jsonstr.Replace("360p", "s360p");
                jsonstr = jsonstr.Replace("480p", "s480p");
                jsonstr = jsonstr.Replace("720p", "s720p");
                jsonstr = "{\"data\": [" + jsonstr + "],\"max_time\": 1485232534}";
                NHPicData nhpicdata = Newtonsoft.Json.JsonConvert.DeserializeObject<NHPicData>(jsonstr);
                int picnum = 1;
                foreach (NHPicData.Datum datum in nhpicdata.data)
                {
                    int isHave = Convert.ToInt32(SQLiteHelper.ExecuteScalar("select count(*) from mblog where id=" + datum.group.group_id));
                    if (isHave > 0)
                        continue;//如果数据已存在，则跳过
                    if (datum.group.is_gif != null && datum.group.is_gif == 1)
                    {
                       
                        AppenColCmd("抓取正文："+ datum.group.text);
                        string dirname = "temp/" + datum.group.text;
                        if (datum.group.text == "") dirname = "temp/" + Guid.NewGuid().ToString();
                        if (!Directory.Exists(dirname))
                            Directory.CreateDirectory(dirname);
                        else
                            continue;
                        if(datum.group.gifvideo != null)
                        {
                            string mp4file = datum.group.gifvideo.mp4_url;
                        }
                        string giffile = "http://p3.pstatp.com/" + datum.group.large_image.uri;
                        //HttpHelper1.HttpDownloadFile(mp4file, dirname+"/"+picnum+".mp4", cc);
                        AppenColCmd("下载GIF");
                        HttpHelper1.HttpDownloadFile(giffile, dirname + "/" + picnum + ".gif", cc);
                        picnum++;
                        Directory.Move(dirname, dirname.Replace("temp", "待发布"));
                    }
                    int staresult = SQLiteHelper.ExecuteNonQuery("insert into mblog(id,Source,Text,CreateAt,TbkLinks)values(@id,@Source,@Text,@CreateAt,@TbkLinks)", new[] {
                                                    datum.group.group_id,
                                                    datum.group.category_name,
                                                    datum.group.text,
                                                    datum.group.create_time,
                                                    ""
                                                 });
                }
                int alimamajiange = Convert.ToInt32(this.nud_coljiange.Value);
                AppenColCmd("此次抓取完毕，等待" + alimamajiange.ToString() + "分钟");
                Thread.Sleep(alimamajiange * 60 * 1000);
            }
            
        }

        private void btn_nhStop_Click(object sender, EventArgs e)
        {
            if (nhThread != null && nhThread.IsAlive)
            {
                nhThread.Abort();
                SetNHButtonStatus(false);
                AppenColCmd("用户停止内涵段子抓取任务!!!");
            }
        }
    }
}
