using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;
using System.Net;
using Weibo.Common;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;
using Top.Api.Domain;
using System.Web;
using Newtonsoft.Json.Linq;
using Weibo.Models;
using Weibo.Core;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;
using System.Runtime.InteropServices;

namespace Weibo
{
    public partial class FrmMain : Form
    {

        private Thread alimamaThread;
        private Thread weiboThread;
        private CookieContainer alimamacc;
        private CookieContainer weibocc;
        private ArrayList DEWeiboAccounts;
        public FrmMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        /// <summary>
        /// 添加阿里妈妈日志信息
        /// </summary>
        /// <param name="info">日志信息</param>
        public void AppenAlimamaCmd(string info)
        {
            string loginfo = "";

            loginfo = DateTime.Now.ToString() + ":" + info + "\r\n";

            this.txtAlimamaCmd.AppendText(loginfo);
            if (!Directory.Exists("log/"))
                Directory.CreateDirectory("log");
            File.AppendAllText("log/alimama" + DateTime.Now.ToString("yyyy-MM-dd") + "-log.log", loginfo);
        }

        /// <summary>
        /// 添加阿里妈妈日志信息
        /// </summary>
        /// <param name="info">日志信息</param>
        public void AppenWeiboCmd(string info)
        {
            string loginfo = "";

            loginfo = DateTime.Now.ToString() + ":" + info + "\r\n";

            this.txtWeiboCmd.AppendText(loginfo);
            if (!Directory.Exists("log/"))
                Directory.CreateDirectory("log");
            File.AppendAllText("log/weibo-" + DateTime.Now.ToString("yyyy-MM-dd") + "-log.log", loginfo);
        }

        /// <summary>
        /// 阿里妈妈测试登录按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestLogin_Alimama_Click(object sender, EventArgs e)
        {
            if (alimamacc == null)
                alimamacc = Alimama.GetCookieContainer();
            if (!Alimama.TestLogin(alimamacc))
            {
                //未登录
                //FrmLogin frmlogin = new FrmLogin("tianmin200", "tianmin3216779236", "http://www.alimama.com/member/login.htm");
                //if (frmlogin.ShowDialog() == DialogResult.OK)
                //    alimamacc = frmlogin.cc;
                //this.lblIsLogin_Alimama.ForeColor = Color.Red;
                //this.lblIsLogin_Alimama.Text = "已登录";
                alimamacc = Alimama.Login();
                this.lblIsLogin_Alimama.ForeColor = Color.Red;
                this.lblIsLogin_Alimama.Text = "已登录";

            }
            else
            {
                this.lblIsLogin_Alimama.ForeColor = Color.Red;
                this.lblIsLogin_Alimama.Text = "已登录";
            }

        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            //cc = Alimama.GetCookieContainer();
            //if (Alimama.TestLogin(cc))
            //{
            //    this.lblIsLogin_Alimama.ForeColor = Color.Red;
            //    this.lblIsLogin_Alimama.Text = "已登录";
            //}

        }


        /// <summary>
        /// 开始抓取淘宝客选品库按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_StartAlimama_Click(object sender, EventArgs e)
        {
            ArrayList parms = new ArrayList();
            parms.Add("");
            parms.Add(1);
            alimamaThread = new Thread(new ParameterizedThreadStart(StartGetXuanpinku));
            alimamaThread.Start(parms);
        }

        /// <summary>
        /// 设置淘宝客选品库抓取按钮状态
        /// </summary>
        /// <param name="isexe"></param>
        public void SetStartAlimamaBtnStatus(bool isexe)
        {
            if (isexe)
            {
                this.btn_StartAlimama.Enabled = false;
                this.btn_col.Enabled = false;
                this.btn_SearchShop.Enabled = false;
                this.btn_NanzhuangCol.Enabled = false;
                this.button5.Enabled = false;
                this.btn_StopAlimama.Enabled = true;
            }
            else
            {
                this.btn_StartAlimama.Enabled = true;
                this.btn_col.Enabled = true;
                this.btn_SearchShop.Enabled = true;
                this.btn_NanzhuangCol.Enabled = true;
                this.button5.Enabled = true;
                this.btn_StopAlimama.Enabled = false;
            }
        }

        /// <summary>
        /// 设置开始发布微博按钮状态
        /// </summary>
        /// <param name="isexe"></param>
        public void SetStartWeiboBtnStatus(bool isexe)
        {
            if (isexe)
            {
                this.btn_StartWeibo.Enabled = false;
                this.btn_sendTuwen.Enabled = false;
                this.btn_StopWeibo.Enabled = true;
            }
            else
            {
                this.btn_StartWeibo.Enabled = true;
                this.btn_sendTuwen.Enabled = true;
                this.btn_StopWeibo.Enabled = false;
            }
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);
        /// <summary>
        /// 阿里妈妈首页访问，保持登录状态，每半小时自动执行一次
        /// </summary>
        /// <param name="parmsobj"></param>
        public void AlimamaKeepCookie(object parmsobj)
        {
            //this.webBrowser_alimama.Refresh();
        }
        /// <summary>
        /// 异步启动获取选品库
        /// </summary>
        /// <param name="parmsobj"></param>
        public void StartGetXuanpinku(object parmsobj)
        {
            while (true)
            {
                //自动获取选品库步骤：
                //1、登陆阿里妈妈获取Cookie（如已有Cookie则跳过）
                //2、抓取阿里妈妈选品库数据并下载选品库图片，生成评论所需链接
                SetStartAlimamaBtnStatus(true);
                AppenAlimamaCmd("开始提取选品库……");


                if (this.lblIsLogin_Alimama.Text == "未登录")
                {
                    MessageBox.Show("未登录");
                    SetStartAlimamaBtnStatus(false);
                    return;
                }
                string json_xuanpinku = HttpHelper1.SendDataByGET(Alimama.url_xuanpinku, ref alimamacc);
                while (json_xuanpinku.Contains("totalCount\":0"))
                {
                    AppenAlimamaCmd("共找到0个待发布选品库，该去阿里妈妈选品了!!!20分钟后再来！！！");
                    Thread.Sleep(20 * 60 * 1000);
                    json_xuanpinku = HttpHelper1.SendDataByGET(Alimama.url_xuanpinku, ref alimamacc);
                    //SetStartAlimamaBtnStatus(false);
                    //return;
                }
                JSONObject jsonobj = JSONConvert.DeserializeObject(json_xuanpinku);
                JSONObject datajson = (JSONObject)jsonobj["data"];
                JSONArray array_xiaopinku = (JSONArray)datajson["result"];

                AppenAlimamaCmd("共找到" + array_xiaopinku.Count.ToString() + "个待发布选品库……");
                for (int i = 0; i < array_xiaopinku.Count; i++)
                {
                    AppenAlimamaCmd("处理第" + (i + 1).ToString() + "个选品库……");
                    JSONObject obj_xuanpinku = (JSONObject)array_xiaopinku[i];
                    string groupid = obj_xuanpinku["id"].ToString();//选品库ID
                    string groupName = obj_xuanpinku["title"].ToString();//选品库title
                    DEWeiboAccount deweiboaccount = WeiboHandler.GetOneAccount();
                    string fileDownLoadUrl = "http://pub.alimama.com/favorites/item/export.json?adzoneId=" + deweiboaccount.Adzoneid + "&siteId=" + deweiboaccount.Siteid + "&groupId=" + groupid;//选品库Excel文件下载
                    if (Directory.Exists("待发布/"))
                        Directory.CreateDirectory("待发布");
                    string fileSaveName = "待发布/" + groupName + "/" + groupName + ".xls";//保存文件名
                    if (!Directory.Exists("待发布/" + groupName))
                        Directory.CreateDirectory("待发布/" + groupName);
                    AppenAlimamaCmd("下载选品库Excel……");
                    string filepath = HttpHelper1.HttpDownloadFile(fileDownLoadUrl, fileSaveName, alimamacc);//下载Excel文件

                    AppenAlimamaCmd("完成");

                    #region 解析选品库Excel文件
                    DataSet ds = HttpHelper1.ExcelToDS(fileSaveName);
                    string picurl = "";
                    int j = 1;
                    string comment = "";//微博评论链接内容
                    AppenAlimamaCmd("下载选品库九图……");
                    foreach (DataTable dt in ds.Tables)   //遍历所有的datatable
                    {
                        foreach (DataRow dr in dt.Rows)   ///遍历所有的行
                        {

                            picurl = dr[2].ToString();
                            string[] str = picurl.Split('/');
                            string picfile = str[str.Length - 1];

                            string link = dr[10].ToString();
                            string weiboShortlink = WeiboHandler.GetWeiboShorturl(link);//微博短地址
                            comment += "图" + j.ToString() + ":" + weiboShortlink + " ";
                            string picresultpath = HttpHelper1.HttpDownloadFile(picurl, "待发布/" + groupName + "/" + j.ToString() + ".jpg", alimamacc);
                            if (picresultpath == null) continue;
                            j++;
                        }
                    }
                    AppenAlimamaCmd("完成");
                    AppenAlimamaCmd("生成评论链接……");


                    File.WriteAllText("待发布/" + groupName + "/comment.txt", comment);
                    //删除选品库，每次将选品库下载到本地之后，从阿里妈妈后台删除选品库
                    string deletepoststr = "groupId=" + groupid;
                    string deleteurl = "http://pub.alimama.com/favorites/group/delete.json";
                    string refer = "http://pub.alimama.com/manage/selection/list.htm?spm=a219t.7664554.a214tr8.3.gWP3G2";
                    string deleteresult = HttpHelper1.SendDataByPost(deleteurl, deletepoststr, refer, ref alimamacc);

                    AppenAlimamaCmd("第" + (i + 1).ToString() + "个选品库处理完成！！！");
                    #endregion
                }

                AppenAlimamaCmd("此次选品库处理完成！！！共" + array_xiaopinku.Count.ToString() + "个，成功：" + array_xiaopinku.Count.ToString() + "个！！！");
                AppenAlimamaCmd("选品休息三十分钟！！！");
                Thread.Sleep(30 * 60 * 1000);
            }
            //SetStartAlimamaBtnStatus(false);
        }

        /// <summary>
        /// 开始发布微博按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_StartWeibo_Click(object sender, EventArgs e)
        {
            ArrayList parms = new ArrayList();
            parms.Add("");
            parms.Add(1);
            weiboThread = new Thread(new ParameterizedThreadStart(StartSendWeibo));
            weiboThread.Start(parms);
        }

        /// <summary>
        /// 异步自动监控选品库并发送微博
        /// </summary>
        /// <param name="parmsobj"></param>
        public void StartSendWeibo(object parmsobj)
        {
            SetStartWeiboBtnStatus(true);
            string[] weiboAccounts = File.ReadAllLines("config/weiboAccounts.txt");

            while (true)
            {
                int accountnum = 0;
                if (DEWeiboAccounts == null) InitWeiboAccounts();
                foreach (DEWeiboAccount deweiboaccount in DEWeiboAccounts)
                {
                    #region 初始定义
                    bool isSendSuc = false;//是否发布成功
                    string mid = "";
                    string ouid = "";
                    #endregion
                    DirectoryInfo TheFolder = new DirectoryInfo("待发布");
                    while (TheFolder.GetDirectories().Length == 0)
                    {
                        AppenWeiboCmd("没有找到发布素材，等待10分钟！");
                        Thread.Sleep(10 * 60 * 1000);
                    }
                    DirectoryInfo nextFolder = TheFolder.GetDirectories()[0];//发布选品库第一条
                    try
                    {
                        #region 初始化微博账号
                        string username = deweiboaccount.Username;
                        string password = deweiboaccount.Password;
                        string nickname = deweiboaccount.Nickname;
                        AppenWeiboCmd("[" + nickname + "]开始发布微博");
                        #endregion
                        #region 验证登陆并获取refer
                        string strresult = "";
                        CookieContainer weibocc = WeiboHandler.InitWeiboCookie(deweiboaccount.Username);
                        if (!WeiboHandler.TestLogin(weibocc, ref strresult))
                        {
                            //如果账号未登录，先自动登录
                            AppenWeiboCmd("账号:[" + nickname + "]未登录，开始登录！！！");
                            weibocc = WeiboHandler.Login(username, password);
                            if (weibocc == null)
                            {
                                AppenWeiboCmd("账号:[" + nickname + "]登录失败,跳过");
                                continue;
                            }
                            InitWeiboAccounts();//登录之后初始化账号
                        }

                        string userid = "";
                        HttpHelper1.GetStringInTwoKeyword(strresult, ref userid, "$CONFIG['watermark']='", "$CONFIG['domain']", 0);
                        userid = userid.Replace("';", "").Trim();
                        string refer = "http://www.weibo.com/" + userid + "/home?wvr=5";
                        deweiboaccount.Userid = userid.Replace("u/", "");

                        #endregion
                        #region 上传图片文件并获取图片picid
                        //遍历选品库文件夹

                        AppenWeiboCmd("共找到待发布选品库 " + TheFolder.GetDirectories().Length.ToString() + "个");
                        //遍历文件夹，上传九图，组成图片ID参数
                        while (TheFolder.GetDirectories().Length == 0)
                        {
                            AppenWeiboCmd("暂时没有待发布选品库，等待10分钟");
                            Thread.Sleep(10 * 60 * 1000);

                            TheFolder = new DirectoryInfo("待发布");
                        }


                        string picids = "";
                        string comment = "";//链接评论内容
                        string couponcomment = "";//优惠券评论内容
                        ArrayList commentlist = new ArrayList();
                        string weibotext = nextFolder.Name;//微博正文
                        int picnum = 0;
                        bool iscomment = true;
                        //以下是处理单个选品库
                        if (!File.Exists(nextFolder.FullName + "/comment.txt"))
                        {
                            string sheqidir = nextFolder.FullName.Replace("待发布", "已舍弃");
                            //nextFolder.MoveTo(sheqidir);//移动文件夹至“已发布”

                            nextFolder.Delete(true);
                            iscomment = false;
                        }
                        foreach (FileInfo NextFile in nextFolder.GetFiles())
                        {

                            if (NextFile.Extension == ".jpg")
                            {
                                picnum++;
                                AppenWeiboCmd("上传图片" + picnum.ToString());
                                string picpath = NextFile.FullName;
                                string uploadresult = WeiboHandler.uploadWeiboImage(picpath, deweiboaccount.Nickname, weibocc);
                                string picid = "";
                                HttpHelper1.GetStringInTwoKeyword(uploadresult, ref picid, "pid\":\"", "\"}}}}", 0);
                                picids = picids + picid + " ";
                            }
                            else if (NextFile.Name == "comment.txt")
                            {
                                comment = File.ReadAllText(NextFile.FullName);
                                commentlist.Add(comment);
                            }
                            else if (NextFile.Name == "coupon.txt")
                            {
                                couponcomment = File.ReadAllText(NextFile.FullName);
                                commentlist.Add(couponcomment);
                            }

                        }
                        picids = picids.Trim().Replace(" ", "%20");
                        #endregion
                        #region 发布微博
                        AppenWeiboCmd("发布微博，正文：" + weibotext);

                        string result = WeiboHandler.SendWeibo(weibotext, picids, refer, weibocc);

                        if (result.Contains("{\"code\":\"100000\""))
                        {
                            AppenWeiboCmd("微博发布成功");
                            HttpHelper1.GetStringInTwoKeyword(result, ref ouid, "ouid=", "\\\"", 0);
                            HttpHelper1.GetStringInTwoKeyword(result, ref mid, "action-data=\\\"mid=", "&from", 0);
                        }
                        else if (result.Contains("{\"code\":\"100004\""))
                        {
                            //提示100004，重试发布
                            result = WeiboHandler.SendWeiboFromM(weibotext, picids, refer, deweiboaccount, weibocc);
                            if (result.Contains("ok\":1"))
                            {
                                AppenWeiboCmd("微博发布成功");
                            }
                            else
                            {
                                AppenWeiboCmd("微博发布失败，返回：result=" + result);
                                continue;
                            }
                        }
                        #endregion
                        #region 添加评论
                        if (!iscomment)
                        {
                            AppenWeiboCmd("无需发布评论");
                        }
                        else
                        {
                            AppenWeiboCmd("发布评论");
                            while (mid == "")
                            {
                                //http://m.weibo.cn/container/getIndex?type=uid&value=5249378691&containerid=1005055249378691
                                string quanbuweibourl = "http://m.weibo.cn/container/getIndex?uid=" + deweiboaccount.Userid + "&luicode=10000011&lfid=100103type%3D3%26q%3D" + HttpUtility.UrlEncode(deweiboaccount.Nickname) + "&type=uid&value=" + deweiboaccount.Userid;
                                string containerid = "";
                                string result1 = HttpHelper1.SendDataByGET(quanbuweibourl, ref weibocc);
                                HttpHelper1.GetStringInTwoKeyword(result1, ref containerid, "weibo\",\"containerid\":\"", "\",", 0);
                                containerid = containerid.Replace(",\"containerid\":\"", "").Replace("\",", "");
                                quanbuweibourl = quanbuweibourl + "&containerid=" + containerid;
                                string result2 = HttpHelper1.SendDataByGET(quanbuweibourl, ref weibocc);
                                MblogData mbloglist1 = Newtonsoft.Json.JsonConvert.DeserializeObject<MblogData>(result2);
                                foreach (Card card in mbloglist1.Cards)
                                {
                                    Mblog mblog1 = card.Mblog;
                                    if (mblog1.Mblogtype != 0)
                                        continue;
                                    if (picids.StartsWith(mblog1.Pics[0].Pid))
                                        mid = mblog1.Id;
                                    break;
                                }
                                //如果Mid为空，直接读取微博第一条Mid
                            }

                            foreach (string forcommentstr in commentlist)
                            {
                                string commentstr = forcommentstr;
                                //if (commentstr.StartsWith("图1") || commentstr.Contains("粉丝优惠购"))
                                //{
                                //    //牺牲发布效率，把每个微博的渠道数据进行监控，找出不那么赚钱的渠道
                                //    Regex reg = new Regex(@"[a-zA-z]+://[^\s]*");
                                //    MatchCollection mc = reg.Matches(commentstr);
                                //    foreach (Match m in mc)
                                //    {
                                //        string weiboshortlink = m.Value;
                                //        string tbrealitem = "";
                                //        Alimama.GetItemResultWithWeiboShortUrl(weiboshortlink, alimamacc, ref tbrealitem);
                                //        string targetweibourl = GetWeiboShortUrlByTbItem(tbrealitem, deweiboaccount.Siteid, deweiboaccount.Adzoneid, alimamacc);
                                //        if (targetweibourl != "")
                                //        {
                                //            commentstr = commentstr.Replace(m.Value, targetweibourl);
                                //        }
                                //    }
                                //}
                                string commentresult = WeiboHandler.Comment(mid, ouid, commentstr, refer, weibocc);
                                string decode_commentresult = Regex.Unescape(commentresult);
                                if (!decode_commentresult.Contains("抱歉"))
                                {
                                    if (!commentresult.Contains("{\"code\":\"100000\""))
                                    {
                                        //评论失败之后，再次评论
                                        commentresult = WeiboHandler.Comment(mid, ouid, "地址 " + commentstr, refer, weibocc);
                                    }
                                    if (!commentresult.Contains("{\"code\":\"100000\""))
                                    {
                                        //第二次评论尝试
                                        commentresult = WeiboHandler.Comment(mid, ouid, "链接 " + commentstr, refer, weibocc);
                                    }
                                    if (!commentresult.Contains("{\"code\":\"100000\""))
                                    {
                                        //第三次评论尝试
                                        commentresult = WeiboHandler.Comment(mid, ouid, "购 " + commentstr, refer, weibocc);
                                    }
                                    //else
                                    //{
                                    //    //评论发布失败
                                    //    string code = "";
                                    //    HttpHelper1.GetStringInTwoKeyword(commentresult, ref code, "{\"code\":\"", "\"", 0);
                                    //    AppenWeiboCmd("评论发布失败，code:" + code);
                                    //}
                                }
                                if (commentresult.Contains("{\"code\":\"100000\""))
                                    AppenWeiboCmd("评论发布成功");
                                else
                                {
                                    AppenWeiboCmd("评论发布失败，返回值：" + commentresult);
                                    int insertcomment_result = SQLiteHelper.ExecuteNonQuery("insert into MissComment(mid,ouid,uid,text)values(@mid,@ouid,@uid,@text)", new[] {
                            mid,
                            ouid,
                            username,
                            comment
                            });

                                }
                            }
                        }
                        #endregion
                        isSendSuc = true;
                    }
                    catch (Exception ex)
                    {
                        int jiangeshij = Convert.ToInt32(this.nud_weiboJiange.Value);
                        AppenWeiboCmd("发布微博出错，错误信息：" + ex.Message);
                        AppenWeiboCmd("间隔" + jiangeshij.ToString() + "分钟继续");
                        isSendSuc = false;
                        Thread.Sleep(jiangeshij * 60 * 1000);
                    }

                    #region 微博发布后处理
                    //string newdir = nextFolder.FullName.Replace("待发布", "已发布");
                    //nextFolder.MoveTo(newdir);//移动文件夹至“已发布”
                    nextFolder.Delete(true);
                    if (!isSendSuc && mid != "") WeiboHandler.DeleteWeibo(mid, weibocc);
                    accountnum++;
                    if (accountnum == DEWeiboAccounts.Count) continue;//如果是最后一个账号，则不间隔

                    AppenWeiboCmd("[" + deweiboaccount.Nickname + "]发布完成!账号间 间隔2分钟");
                    Thread.Sleep(2 * 60 * 1000);//账号间间隔3分钟
                    #endregion

                }//foreach账号
                int jiange = Convert.ToInt32(this.nud_weiboJiange.Value);
                AppenWeiboCmd("全部微博账号发布完成，间隔" + jiange.ToString() + "分钟！！！");
                Thread.Sleep(jiange * 60 * 1000);



            }//while
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            ArrayList parms = new ArrayList();
            parms.Add("");
            parms.Add(1);
            Thread exeThread = new Thread(new ParameterizedThreadStart(AlimamaKeepCookie));
            exeThread.Start(parms);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //TbkUatmFavoritesGetResponse rsp = Alimama.GetXuanpinku();
            //if (rsp.TotalResults == 0)
            //{
            //    //没有选品库
            //}
            //foreach (TbkFavorites tbkFav in rsp.Results)
            //{
            //    //单个选品库处理
            //    string groupName = tbkFav.FavoritesTitle;
            //    long groupid = tbkFav.FavoritesId;
            //    TbkUatmFavoritesItemGetResponse UatmRsp = Alimama.GetXuanpinkuItem(70220294, groupid);
            //    string picurl = "";
            //    int j = 1;
            //    string comment = "";//微博评论链接内容
            //    AppenAlimamaCmd("下载选品库九图……");
            //    foreach (UatmTbkItem item in UatmRsp.Results)
            //    {
            //        //单个商品处理
            //        picurl = item.PictUrl;
            //        HttpHelper1.HttpDownloadFile(picurl, "待发布/" + groupName + "/" + j.ToString() + ".jpg", alimamacc);
            //        //string[] str = picurl.Split('/');
            //        //string picfile = str[str.Length - 1];

            //        //string link = dr[10].ToString();
            //        string weiboShortlink = WeiboHandler.GetWeiboShorturl(item.ClickUrl);//微博短地址
            //        comment += "图" + j.ToString() + ":" + weiboShortlink + " ";

            //        j++;

            //    }
            //}
            string query = "马丁靴";
            TbkItemGetResponse rsp = Alimama.GetItemsWithQuery(query);


        }

        private void button1_Click(object sender, EventArgs e)
        {
            //获取余额
            //uint ulBalance = 0;
            //int ret = Dama2.D2Balance(
            //    m_softKey, //softawre key (software id)
            //    m_userName,    //user name
            //    m_password,     //password
            //    ref ulBalance);
            //MessageBox.Show(this, "ret=" + ret + "; balance=" + ulBalance);
            //初始化Cookie
            //string cookiestr = File.ReadAllText("weibocookie.txt");
            //CookieCollection ccl = CookieHelper.GetCookieCollectionByString(cookiestr, "weibo.com");
            //CookieContainer weibocc = new CookieContainer();
            //weibocc.MaxCookieSize = 100000;
            //weibocc.Add(ccl);
            //string imgPath = WeiboHandler.GetVcodePic(weibocc);
            //FileInfo fi = new FileInfo(imgPath);
            //FileStream fs = new FileStream(imgPath, FileMode.Open);
            //byte[] ba = new byte[fi.Length];
            //int nRet = fs.Read(ba, 0, (int)fi.Length);
            //if (nRet == 0)
            //{
            //    MessageBox.Show(this, "文件长度为0");
            //    return;
            //}

            ////请求答题
            //StringBuilder VCodeText = new StringBuilder(100);
            //int ret = Dama2.D2Buf(
            //    m_softKey, //softawre key (software id)
            //    m_userName,    //user name
            //    m_password,     //password
            //    ba,         //图片数据，图片数据不可大于4M
            //    (uint)nRet, //图片数据长度
            //    60,         //超时时间，单位为秒，更换为实际需要的超时时间
            //    200,        //验证码类型ID，参见 http://wiki.dama2.com/index.php?n=ApiDoc.GetSoftIDandKEY
            //    VCodeText); //成功时返回验证码文本（答案）
            //MessageBox.Show(this, "ret=" + ret + "; VCodeText=" + VCodeText);
            //if (ret > 0)
            //{
            //    uint ulVCodeID = (uint)ret;
            //}

        }

        private void btnLogin_weibo_Click(object sender, EventArgs e)
        {
            InitWeiboAccounts();
        }
        public void InitWeiboAccounts()
        {
            this.lvwWeiboAccountList.Items.Clear();
            string[] weiboAccounts = File.ReadAllLines("config/weiboAccounts.txt");
            DEWeiboAccounts = new ArrayList();
            int rownum = 1;
            foreach (string strs in weiboAccounts)
            {
                string[] weiboAccount = strs.Split(',');
                if (weiboAccount.Length < 2) continue;
                string username = weiboAccount[0];
                string password = weiboAccount[1];
                string nickname = weiboAccount[2];
                string siteid = weiboAccount[3];
                string adzoneid = weiboAccount[4];
                ListViewItem lvi = new ListViewItem(rownum.ToString());
                lvi.SubItems.Add(nickname);

                string uid = "";
                if (WeiboHandler.TestLogin(username, ref weibocc, ref uid))
                {
                    AppenWeiboCmd("账号 " + username + " Cookie可用，无需登录！！！");
                    DEWeiboAccount deweiboaccount = new DEWeiboAccount();
                    deweiboaccount.Username = username;
                    deweiboaccount.Password = password;
                    deweiboaccount.Nickname = nickname;
                    deweiboaccount.Islogin = true;
                    deweiboaccount.Cookie = weibocc;
                    deweiboaccount.Siteid = siteid;
                    deweiboaccount.Adzoneid = adzoneid;
                    string st = WeiboHandler.GetSTFromM(weibocc);
                    deweiboaccount.St = st;
                    deweiboaccount.Userid = uid;
                    if (DEWeiboAccounts == null) DEWeiboAccounts = new ArrayList();
                    DEWeiboAccounts.Add(deweiboaccount);
                    lvi.SubItems.Add("已登录");
                    this.lvwWeiboAccountList.Items.Add(lvi);
                    rownum++;
                    continue;
                }
                var preData = WeiboHandler.PreLogin(username);
                if (preData != null)
                {
                    string code = "";
                    var img = WeiboHandler.GetLoginCodePic(preData.pcid);
                    while (code == "" || code == "IERROR" || code == "ERROR")
                    {
                        code = Dama2.GetVcode(img);
                    }

                    var loginData = WeiboHandler.Login(preData, username, password, code);
                    weibocc = WeiboHandler.InitWeiboCookie(username, loginData.cookies);
                    string strresult = "";
                    bool isLogin = WeiboHandler.TestLogin(weibocc, ref strresult);
                    if (isLogin)
                    {
                        //登录成功保存Cookie
                        string userid = "";
                        HttpHelper1.GetStringInTwoKeyword(strresult, ref userid, "$CONFIG['uid']='", "';", 0);
                        File.AppendAllText("weibocookie/" + username + ".txt", loginData.cookies);
                        AppenWeiboCmd("账号 " + username + "登录成功！记录Cookie！！！");
                        DEWeiboAccount deweiboaccount = new DEWeiboAccount();
                        deweiboaccount.Username = username;
                        deweiboaccount.Password = password;
                        deweiboaccount.Nickname = nickname;
                        deweiboaccount.Islogin = true;
                        deweiboaccount.Cookie = weibocc;
                        deweiboaccount.Siteid = siteid;
                        deweiboaccount.Adzoneid = adzoneid;
                        string st = WeiboHandler.GetSTFromM(weibocc);
                        deweiboaccount.St = st;
                        deweiboaccount.Userid = uid;
                        if (DEWeiboAccounts == null) DEWeiboAccounts = new ArrayList();
                        DEWeiboAccounts.Add(deweiboaccount);
                        lvi.SubItems.Add("已登录");
                    }
                    else
                    {
                        lvi.SubItems.Add("未登录");
                        AppenWeiboCmd("账号 " + username + "登录失败！");
                    }
                    this.lvwWeiboAccountList.Items.Add(lvi);
                    rownum++;
                }
            }
        }

        private void btn_StopAlimama_Click(object sender, EventArgs e)
        {
            SetStartAlimamaBtnStatus(false);
            if (alimamaThread != null && alimamaThread.IsAlive)
            {
                alimamaThread.Abort();
                //SetStartAlimamaBtnStatus(false);
                AppenAlimamaCmd("用户停止读取选品库任务...");
            }
        }

        private void btn_StopWeibo_Click(object sender, EventArgs e)
        {
            if (weiboThread != null && weiboThread.IsAlive)
            {
                weiboThread.Abort();
                SetStartWeiboBtnStatus(false);
                AppenWeiboCmd("用户停止微博发布任务...");
            }

        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (alimamaThread != null)
                this.alimamaThread.Abort();
            if (weiboThread != null)
                this.weiboThread.Abort();
            this.Dispose();

        }

        private void btn_col_Click(object sender, EventArgs e)
        {
            ArrayList parms = new ArrayList();
            parms.Add("");
            parms.Add(1);
            alimamaThread = new Thread(new ParameterizedThreadStart(StartColXuanpinku));
            alimamaThread.Start(parms);
        }

        public void StartColXuanpinku(object parms)
        {
            while (true)
            {
                SetStartAlimamaBtnStatus(true);
                Mblog mblog = null;
                try
                {
                    int pagenum = Convert.ToInt32(this.nud_Pages.Value);
                    for (int i = 1; i < pagenum + 1; i++)
                    {
                        AppenAlimamaCmd("开始抓取第" + i.ToString() + "页微博内容");

                        string[] weiboAccounts = File.ReadAllLines("config/weiboAccounts.txt");

                        weibocc = WeiboHandler.InitWeiboCookie(weiboAccounts[0].Split(',')[0]);

                        CookieCollection ccl = weibocc.GetCookies(new Uri("http://weibo.com"));


                        string[] urls = File.ReadAllLines(("config/colweibo.txt"));
                        foreach (string urltemp in urls)
                        {
                            string url = urltemp + i.ToString();
                            MblogData mbloglist = WeiboHandler.GetMblogsWithUrl(url,weibocc);

                            foreach (Card card in mbloglist.Cards)
                            {
                                if (card.Mblog == null) continue;
                                mblog = card.Mblog;
                                string mid = card.Mblog.Id;
                                string links = "";//多链接以,分割
                                int j = 1;
                                string own_tbklinks = "";
                                string couponlinks = "粉丝优惠购：";
                                bool isCoupon = false;

                                int isHave = Convert.ToInt32(SQLiteHelper.ExecuteScalar("select count(*) from mblog where id=" + mid));
                                if (isHave > 0)
                                    continue;//如果数据已存在，则跳过

                                //先查看评论是否包含链接
                                WeiboComment comment = WeiboHandler.GetComment(mid, weibocc);
                                if (comment == null) continue;
                                
                                string dirname = WeiboHandler.GetDirnameByMblogText(mblog.Text);
                                
                                #region 九宫格淘宝客处理
                                if (comment.Data.Html.Contains("图1："))
                                {
                                    //评论包含淘宝客链接
                                    string tbklinks_html = "";
                                    HttpHelper1.GetStringInTwoKeyword(comment.Data.Html, ref tbklinks_html, "图1：", "<!-- 评论图片的处理 -->", 0);
                                    Hashtable tbklinks = new Hashtable();
                                    Regex reg = new Regex(@"[a-zA-z]+://[^\s]*");
                                    MatchCollection mc = reg.Matches(tbklinks_html);
                                    //创建选品文件夹
                                    try
                                    {
                                        if (!Directory.Exists("temp/"))
                                            Directory.CreateDirectory("temp");
                                        if (!Directory.Exists("temp/" + dirname + "/"))
                                            Directory.CreateDirectory("temp/" + dirname + "/");
                                    }
                                    catch (Exception ex)
                                    {
                                        dirname = mblog.Id;
                                        if (!Directory.Exists("temp/" + dirname + "/"))
                                            Directory.CreateDirectory("temp/" + dirname + "/");

                                        //throw;
                                    }
                                    int linknum = 1;
                                    foreach (Match m in mc)
                                    {
                                        AppenAlimamaCmd("转换链接" + linknum.ToString());
                                        linknum++;
                                        string weiboshortlink = m.Value.Replace("\"", "");
                                        if (!weiboshortlink.StartsWith("http://t.cn")) continue;
                                        links = links + weiboshortlink + ",";
                                        
                                        string tbrealitem = "";
                                        string result = Alimama.GetItemResultWithWeiboShortUrl(weiboshortlink, alimamacc, ref tbrealitem);

                                        if (tbrealitem.Contains("http://ai.taobao.com"))
                                        {
                                            int staresult = SQLiteHelper.ExecuteNonQuery("insert into mblog(id,Source,Text,CreateAt,TbkLinks)values(@id,@Source,@Text,@CreateAt,@TbkLinks)", new[] {
                                                    mblog.Id,
                                                    mblog.Source,
                                                    mblog.Text,
                                                    mblog.CreatedAt,
                                                    own_tbklinks
                                                 });
                                            break;
                                        }
                                        if (alimamacc == null)
                                            alimamacc = Alimama.GetCookieContainer();
                                        
                                        DEWeiboAccount deweiboaccount = WeiboHandler.GetOneAccount();
                                        string own_tbklink = Alimama.GetTbkLink(tbrealitem, deweiboaccount.Siteid, deweiboaccount.Adzoneid, alimamacc);//把淘宝客链接更换为自己链接       
                                        if (own_tbklink == "" || own_tbklink == "链接不支持转化")
                                        {
                                            int staresult = SQLiteHelper.ExecuteNonQuery("insert into mblog(id,Source,Text,CreateAt,TbkLinks)values(@id,@Source,@Text,@CreateAt,@TbkLinks)", new[] {
                                                    mblog.Id,
                                                    mblog.Source,
                                                    mblog.Text,
                                                    mblog.CreatedAt,
                                                    own_tbklinks
                                                 });
                                            break;//没有淘宝客链接，当前内容不可用，直接退出循环
                                        }
                                        else if (own_tbklink == "阿里妈妈登录失效")
                                        {
                                            AppenAlimamaCmd("阿里妈妈登录失效，停止抓取！！！正在重新登录");
                                            alimamacc = Alimama.Login();
                                            if (Alimama.TestLogin(alimamacc))
                                                AppenAlimamaCmd("阿里妈妈重新登录成功！");
                                            else
                                            {
                                                AppenAlimamaCmd("阿里妈妈重新登录失败，停止抓取！");
                                                return;
                                            }

                                        }
                                        //string bdshorturl = HttpHelper1.GetBdShortUrl(own_tbklink);
                                        //bdshorturl = bdshorturl.Replace("\\", "");//包装一层百度短网址，防屏蔽  1.16 加一层跳转之后，会被微博反垃圾提示危险网址
                                        
                                        string weiboShortlink = WeiboHandler.GetWeiboShorturl(own_tbklink);//微博短地址
                                        own_tbklinks += "图" + j.ToString() + ":" + weiboShortlink + " ";

                                        string searchresult = Alimama.SearchItem(tbrealitem, alimamacc);
                                        bool isSuc = Alimama.ApplyCampaign(searchresult, tbrealitem, alimamacc);//申请定向计划

                                        //获取优惠券信息
                                        if (!searchresult.Contains("couponInfo\":\"无"))
                                        {
                                            string couponshorturl = Alimama.GetCouponInfo(searchresult, tbrealitem, alimamacc);
                                            string couponweiboShortlink = WeiboHandler.GetWeiboShorturl(couponshorturl);//微博短地址
                                            couponlinks += "图" + j.ToString() + ":" + couponweiboShortlink + " ";
                                            isCoupon = true;
                                        }

                                        j++;

                                    }//单个链接处理完成
                                    if (j == 10)//只有九图才添加comment.txt
                                    {
                                        //直接下载微博配图，截掉水印
                                        int picnum = 1;
                                        foreach (Pic pic in mblog.Pics)
                                        {
                                            AppenAlimamaCmd("下载图" + picnum.ToString());
                                            string savepicurl = "temp/" + dirname + "/" + picnum.ToString() + ".jpg";
                                            string picurl = "http://wx2.sinaimg.cn/large/" + pic.Pid + ".jpg";
                                            HttpHelper1.HttpDownloadFile(picurl, "temp/" + dirname + "/" + picnum.ToString() + ".jpgtemp", alimamacc);
                                            Image img = Image.FromFile(savepicurl + "temp");

                                            //截取水印
                                            Bitmap bitmap = new Bitmap(img);
                                            bitmap = ImageHelper.Cut(bitmap, 0, 0, img.Width, img.Height - 20);
                                            bitmap.Save(savepicurl);
                                            bitmap.Dispose();
                                            img.Dispose();
                                            File.Delete(savepicurl + "temp");
                                            picnum++;
                                        }
                                        File.WriteAllText("temp/" + dirname + "/comment.txt", own_tbklinks);
                                        if (isCoupon)
                                            File.WriteAllText("temp/" + dirname + "/coupon.txt", couponlinks);
                                        File.WriteAllText("temp/" + dirname + "/id.txt", mblog.Id);
                                        AppenAlimamaCmd("抓取成功，微博正文：" + mblog.Text + ";ID：" + mblog.Id);
                                        try
                                        {
                                            //如果文件夹重名，先删除老的文件夹
                                            if (Directory.Exists("待发布/" + dirname))
                                                Directory.Delete("待发布/" + dirname, true);
                                            Directory.Move("temp/" + dirname, "待发布/" + dirname);//下載完成放入待发布库
                                        }
                                        catch (Exception)
                                        {

                                        }

                                    }


                                }
                                #endregion
                                #region 优惠券淘宝客处理 舍棄
                                if (comment.Data.Html.Contains("领："))
                                {
                                    //评论包含淘宝客链接
                                    string tbklinks_html = "";
                                    HttpHelper1.GetStringInTwoKeyword(comment.Data.Html, ref tbklinks_html, "领：", "<!-- 评论图片的处理 -->", 0);
                                    Hashtable tbklinks = new Hashtable();
                                    Regex reg = new Regex(@"[a-zA-z]+://[^\s]*");
                                    MatchCollection mc = reg.Matches(tbklinks_html);
                                    int time = 1;
                                    foreach (Match m in mc)
                                    {
                                        string weiboshortlink = m.Value.Replace("\"", "");
                                        if (!weiboshortlink.StartsWith("http://t.cn")) continue;
                                        if (time == 1)
                                        {
                                            //第一个链接为优惠券链接
                                            own_tbklinks = "优惠：" + weiboshortlink + "  ";
                                            time++;
                                            continue;
                                        }

                                        string tbrealitem = "";
                                        string result = Alimama.GetItemResultWithWeiboShortUrl(weiboshortlink, alimamacc, ref tbrealitem);

                                        //创建选品文件夹
                                        try
                                        {
                                            if (!Directory.Exists("temp/" + dirname + "/"))
                                                Directory.CreateDirectory("temp/" + dirname + "/");
                                        }
                                        catch (Exception ex)
                                        {
                                            dirname = mblog.Id;
                                            if (!Directory.Exists("temp/" + dirname + "/"))
                                                Directory.CreateDirectory("temp/" + dirname + "/");
                                            //throw;
                                        }

                                        if (alimamacc == null)
                                            alimamacc = Alimama.GetCookieContainer();
                                        string siteid = "20116380";
                                        string adzoneId = "70602305";
                                        DEWeiboAccount deweiboaccount = WeiboHandler.GetOneAccount();
                                        string weiboShortlink = GetWeiboShortUrlByTbItem(tbrealitem, deweiboaccount.Siteid, deweiboaccount.Adzoneid, alimamacc);
                                        if (weiboshortlink == "")
                                        {
                                            continue;
                                        }
                                        own_tbklinks += "购买：" + weiboShortlink + "";

                                        //下载微博图片
                                        int picnum = 1;
                                        foreach (Pic pic in mblog.Pics)
                                        {
                                            string picurl = "http://wx2.sinaimg.cn/large/" + pic.Pid + ".jpg";
                                            HttpHelper1.HttpDownloadFile(picurl, "temp/" + dirname + "/" + picnum.ToString() + ".jpg", alimamacc);
                                            picnum++;
                                        }
                                    }//单个链接处理完成
                                    File.WriteAllText("temp/" + dirname + "/comment.txt", own_tbklinks);
                                    File.WriteAllText("temp/" + dirname + "/id.txt", mblog.Id);
                                    Directory.Move("temp/" + dirname, "待发布/" + dirname);
                                    AppenAlimamaCmd("抓取成功，微博正文：" + mblog.Text + ";ID：" + mblog.Id);
                                }
                                #endregion
                                int statu = SQLiteHelper.ExecuteNonQuery("insert into mblog(id,Source,Text,CreateAt,TbkLinks)values(@id,@Source,@Text,@CreateAt,@TbkLinks)", new[] {
                            mblog.Id,
                            mblog.Source,
                            mblog.Text,
                            mblog.CreatedAt,
                            own_tbklinks
                            });//选品库下载完成后，保存微博数据到数据库
                            }
                        }

                    }
                    int alimamajiange = Convert.ToInt32(this.nud_alimamajiange.Value);
                    AppenAlimamaCmd("此次抓取完毕，等待" + alimamajiange.ToString() + "分钟");
                    Thread.Sleep(alimamajiange * 60 * 1000);
                }
                catch (Exception ex)
                {
                    AppenAlimamaCmd(ex.Message);

                }
                finally
                {
                    if (mblog != null)
                    {
                        try
                        {
                            int statu = SQLiteHelper.ExecuteNonQuery("insert into mblog(id,Source,Text,CreateAt,TbkLinks)values(@id,@Source,@Text,@CreateAt,@TbkLinks)", new[] {
                            mblog.Id,
                            mblog.Source,
                            mblog.Text,
                            mblog.CreatedAt,
                            ""
                            });//选品库下载完成后，保存微博数据到数据库
                        }
                        catch (Exception)
                        {


                        }

                    }
                }
            }

        }

        public string GetWeiboShortUrlByTbItem(string tbrealitem, string siteid, string adzoneid, CookieContainer alimamacc)
        {
            string own_tbklink = Alimama.GetTbkLink(tbrealitem, siteid, adzoneid, alimamacc);//把淘宝客链接更换为自己链接       
            if (own_tbklink == "" || own_tbklink == "链接不支持转化")
            {
                return "";//没有淘宝客链接，跳过
            }
            else if (own_tbklink == "阿里妈妈登录失效")
            {
                AppenAlimamaCmd("阿里妈妈登录失效，停止抓取！！！请重新登录");
                alimamacc = Alimama.Login();
                if (Alimama.TestLogin(alimamacc))
                    AppenAlimamaCmd("阿里妈妈重新登录成功！");
                else
                {
                    AppenAlimamaCmd("阿里妈妈重新登录失败，链接转换失败！");
                    return "";
                }
            }
            string weiboShortlink = WeiboHandler.GetWeiboShorturl(own_tbklink);//微博短地址
            return weiboShortlink;
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void btn_ConfigLoginAlimama_Click(object sender, EventArgs e)
        {
            //获取Cookie
            CookieContainer myCookieContainer = new CookieContainer();
            string cookieStr = CookieReader.GetGlobalCookies(webBrowser_alimama.Document.Url.AbsoluteUri);
            string domain = "alimama.com";

            //string domian = "alimama.com";
            //File.AppendAllText("cookie.txt", UserName + "\t" + cookieStr + "\r\n");//记录异常账号，下一次不再重复访问
            if (File.Exists("config/alimamacookie.txt"))
                File.Delete("config/alimamacookie.txt");
            File.AppendAllText("config/alimamacookie.txt", cookieStr);//1.8 修改只记录阿里妈妈登陆cookie
            CookieCollection ccl = CookieHelper.GetCookieCollectionByString(cookieStr, domain);
            myCookieContainer.Add(ccl);
            alimamacc = myCookieContainer;
            //string zhuanhuanstr = "";
            //int linknum = 1;
            //string[] links = this.richTextBox1.Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //foreach (string link in links)
            //{
            //    DEWeiboAccount deweiboaccount = WeiboHandler.GetOneAccount();
            //    string shortutl = GetWeiboShortUrlByTbItem(link, deweiboaccount.Siteid, deweiboaccount.Adzoneid, alimamacc);
            //    zhuanhuanstr += "图" + linknum.ToString() + ":" + shortutl + " ";
            //    linknum++;
            //}

            //this.richTextBox1.Text = zhuanhuanstr;
        }

        private void txtWeiboCmd_TextChanged(object sender, EventArgs e)
        {
            this.txtWeiboCmd.SelectionStart = txtWeiboCmd.Text.Length;
            txtWeiboCmd.ScrollToCaret();
        }

        private void btn_SendJingXuan_Click(object sender, EventArgs e)
        {
            DirectoryInfo TheFolder = new DirectoryInfo("精选");
            while (TheFolder.GetDirectories().Length == 0)
            {
                return;
            }
            DirectoryInfo nextFolder = TheFolder.GetDirectories()[0];//发布选品库第一条
            foreach (DEWeiboAccount deweiboaccount in DEWeiboAccounts)
            {
                #region 初始化微博账号
                string username = deweiboaccount.Username;
                string password = deweiboaccount.Password;
                string nickname = deweiboaccount.Nickname;
                AppenWeiboCmd("[" + nickname + "]开始发布微博");
                #endregion
                #region 验证登陆并获取refer
                string strresult = "";
                CookieContainer weibocc = WeiboHandler.InitWeiboCookie(deweiboaccount.Username);
                if (!WeiboHandler.TestLogin(weibocc, ref strresult))
                {
                    //如果账号未登录，先自动登录
                    AppenWeiboCmd("账号:[" + nickname + "]未登录，开始登录！！！");
                    weibocc = WeiboHandler.Login(username, password);
                    if (weibocc == null)
                    {
                        AppenWeiboCmd("账号:[" + nickname + "]登录失败,跳过");
                        continue;
                    }
                }

                string userid = "";
                HttpHelper1.GetStringInTwoKeyword(strresult, ref userid, "$CONFIG['watermark']='", "$CONFIG['domain']", 0);
                userid = userid.Replace("';", "").Trim();
                string refer = "http://www.weibo.com/" + userid + "/home?wvr=5";
                #endregion

                #region 上传图片
                string picids = "";
                string comment = "";//评论内容
                string weibotext = nextFolder.Name;//微博正文
                int picnum = 0;
                //以下是处理单个选品库
                foreach (FileInfo NextFile in nextFolder.GetFiles())
                {

                    if (NextFile.Extension == ".jpg")
                    {
                        picnum++;
                        AppenWeiboCmd("上传图片" + picnum.ToString());
                        string picpath = NextFile.FullName;
                        string uploadresult = WeiboHandler.uploadWeiboImage(picpath, deweiboaccount.Nickname, weibocc);
                        string picid = "";
                        HttpHelper1.GetStringInTwoKeyword(uploadresult, ref picid, "pid\":\"", "\"}}}}", 0);
                        picids = picids + picid + " ";
                    }
                    else if (NextFile.Name == "comment.txt")
                        comment = File.ReadAllText(NextFile.FullName);
                }
                picids = picids.Trim().Replace(" ", "%20");
                #endregion
                #region 发布微博
                AppenWeiboCmd("发布微博，正文：" + weibotext);

                string result = WeiboHandler.SendWeibo(weibotext, picids, refer, weibocc);
                string mid = "";
                string ouid = "";
                if (result.Contains("{\"code\":\"100000\""))
                {
                    AppenWeiboCmd("微博发布成功");
                    HttpHelper1.GetStringInTwoKeyword(result, ref ouid, "ouid=", "\\\"", 0);
                    HttpHelper1.GetStringInTwoKeyword(result, ref mid, "action-data=\\\"mid=", "&from", 0);
                }
                #endregion
                #region 添加评论
                AppenWeiboCmd("发布评论");
                while (mid == "")
                {
                    //http://m.weibo.cn/container/getIndex?type=uid&value=5249378691&containerid=1005055249378691
                    string quanbuweibourl = "http://m.weibo.cn/container/getIndex?uid=" + deweiboaccount.Userid + "&luicode=10000011&lfid=100103type%3D3%26q%3D" + HttpUtility.UrlEncode(deweiboaccount.Nickname) + "&type=uid&value=" + deweiboaccount.Userid;
                    string containerid = "";
                    string result1 = HttpHelper1.SendDataByGET(quanbuweibourl, ref weibocc);
                    HttpHelper1.GetStringInTwoKeyword(result1, ref containerid, "weibo\",\"containerid\":\"", "\",", 0);
                    containerid = containerid.Replace(",\"containerid\":\"", "").Replace("\",", "");


                    quanbuweibourl = quanbuweibourl + "&containerid=" + containerid;


                    string result2 = HttpHelper1.SendDataByGET(quanbuweibourl, ref weibocc);
                    MblogData mbloglist1 = Newtonsoft.Json.JsonConvert.DeserializeObject<MblogData>(result2);
                    foreach (Card card in mbloglist1.Cards)
                    {
                        Mblog mblog1 = card.Mblog;
                        if (mblog1.Mblogtype != 0)
                            continue;
                        if (picids.StartsWith(mblog1.Pics[0].Pid))
                            mid = mblog1.Id;
                        break;
                    }
                    //如果Mid为空，直接读取微博第一条Mid
                }

                string commentresult = WeiboHandler.Comment(mid, ouid, comment, refer, weibocc);
                if (!commentresult.Contains("{\"code\":\"100000\""))
                {
                    //评论失败之后，再次评论
                    commentresult = WeiboHandler.Comment(mid, ouid, "地址 " + comment, refer, weibocc);
                }
                if (!commentresult.Contains("{\"code\":\"100000\""))
                {
                    //第二次评论尝试
                    commentresult = WeiboHandler.Comment(mid, ouid, "链接 " + comment, refer, weibocc);
                }
                if (!commentresult.Contains("{\"code\":\"100000\""))
                {
                    //第三次评论尝试
                    commentresult = WeiboHandler.Comment(mid, ouid, "购 " + comment, refer, weibocc);
                }
                //else
                //{
                //    //评论发布失败
                //    string code = "";
                //    HttpHelper1.GetStringInTwoKeyword(commentresult, ref code, "{\"code\":\"", "\"", 0);
                //    AppenWeiboCmd("评论发布失败，code:" + code);
                //}
                if (commentresult.Contains("{\"code\":\"100000\""))
                    AppenWeiboCmd("评论发布成功");
                else
                {
                    AppenWeiboCmd("评论发布失败，返回值：" + commentresult);
                    int insertcomment_result = SQLiteHelper.ExecuteNonQuery("insert into MissComment(mid,ouid,uid,text)values(@mid,@ouid,@uid,@text)", new[] {
                            mid,
                            ouid,
                            username,
                            comment
                            });

                }
                #endregion
            }
            Directory.Delete(TheFolder.FullName, true);
        }

        private void txtAlimamaCmd_TextChanged(object sender, EventArgs e)
        {
            this.txtAlimamaCmd.SelectionStart = txtAlimamaCmd.Text.Length;
            txtAlimamaCmd.ScrollToCaret();
        }

        private void btn_KeywordSearchTest_Click(object sender, EventArgs e)
        {
            ArrayList parms = new ArrayList();
            parms.Add("");
            parms.Add(1);
            alimamaThread = new Thread(new ParameterizedThreadStart(StartSearchShopHot));
            alimamaThread.Start(parms);

        }

        public void StartSearchShopHot(object obj)
        {
            SetStartAlimamaBtnStatus(true);
            while (true)
            {
                if (!File.Exists("config/shop.txt"))
                {
                    AppenAlimamaCmd("没有找到【shop.txt】店铺列表文件");
                    return;
                }
                string[] shops = File.ReadAllLines("config/shop.txt");
                AppenAlimamaCmd("开始抓取店铺热销单品作为选品库，此次共需处理" + shops.Length.ToString() + "个店铺");
                int[] randomNums = GetRandomNum(shops.Length, 0, shops.Length);
                foreach (int randomNum in randomNums)
                {
                    try
                    {
                        string shop = shops[randomNum];
                        string[] strs = shop.Split(',');
                        string shopname = strs[0];
                        string nickname = strs[1];
                        AppenAlimamaCmd("开始抓取关键词店铺：" + shopname);
                        GetShopHot(shopname, nickname, alimamacc);
                        AppenAlimamaCmd("抓取【" + shopname + "】成功！间隔5分钟");
                        Thread.Sleep(5 * 60 * 1000);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }

                }
                AppenAlimamaCmd("全部店铺处理完成");
            }
        }
        /// <summary>
        /// 获取店铺定向计划商品列表，默认取最高分成定向计划
        /// </summary>
        /// <param name="query"></param>
        /// <param name="nickname"></param>
        /// <param name="alimamacc"></param>
        public void GetShopCampaignItems(string query, string nickname, CookieContainer alimamacc)
        {
            AlimamaSearchShopData alimamasearchshopdata = Alimama.GetSearchData(query, "", alimamacc);
            string oriMemberId = alimamasearchshopdata.data.pagelist[0].oriMemberId.ToString();//取第一家店
            string dirname = "temp/" + nickname;
            if (!Directory.Exists(dirname))
                Directory.CreateDirectory(dirname);

        }
        /// <summary>
        /// 获取店铺热销商品列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="nickname"></param>
        /// <param name="alimamacc"></param>
        public void GetShopHot(string query, string nickname, CookieContainer alimamacc)
        {
            AlimamaSearchShopData alimamasearchshopdata = Alimama.GetSearchData(query, "", alimamacc);
            string oriMemberId = alimamasearchshopdata.data.pagelist[0].oriMemberId.ToString();//取第一家店
            string dirname = "temp/" + nickname;
            if (!Directory.Exists(dirname))
                Directory.CreateDirectory(dirname);
            AlimamaShopHotData alimamashophotdata = Alimama.GetShopHotItem(oriMemberId, "", alimamacc);
            int picnum = 1;
            string commentstr = "";
            string couponlinks = "粉丝优惠购 ";
            bool isCoupon = false;
            int maxnum = alimamashophotdata.data.pagelist.Length;
            AppenAlimamaCmd("共找到" + maxnum.ToString() + "个商品");
            int[] RandomNums = GetRandomNum(9, 0, maxnum - 1);//获取随机数，随机取热销单品中的九个单品
            foreach (int index in RandomNums)
            {
                AppenAlimamaCmd("处理第" + picnum.ToString() + "个商品！");

                AlimamaShopHotData.Pagelist2 HotItem = alimamashophotdata.data.pagelist[index];

                //处理链接
                string itemlink = HotItem.auctionUrl;
                string siteid = "20116380";
                string adzoneId = "70602305";
                DEWeiboAccount deweiboaccount = WeiboHandler.GetOneAccount();
                string weiboshorturl = GetWeiboShortUrlByTbItem(itemlink, deweiboaccount.Siteid, deweiboaccount.Adzoneid, alimamacc);
                commentstr += "图" + picnum.ToString() + ":" + weiboshorturl + " ";

                //下载图片
                HttpHelper1.HttpDownloadFile(HotItem.pictUrl, dirname + "/" + picnum.ToString() + ".jpg", alimamacc);

                string searchresult = Alimama.SearchItem(itemlink, alimamacc);
                //申请定向计划
                bool isSuc = Alimama.ApplyCampaign(searchresult, itemlink, alimamacc);

                //获取优惠券信息
                if (!searchresult.Contains("couponInfo\":\"无"))
                {
                    string couponshorturl = Alimama.GetCouponInfo(searchresult, itemlink, alimamacc);
                    string couponweiboShortlink = WeiboHandler.GetWeiboShorturl(couponshorturl);//微博短地址
                    couponlinks += "图" + picnum.ToString() + ":" + couponweiboShortlink + " ";
                    if (!isCoupon) isCoupon = true;
                }
                picnum++;
            }

            File.WriteAllText(dirname + "/comment.txt", commentstr);
            if (isCoupon)
                File.WriteAllText(dirname + "/coupon.txt", couponlinks);
            string daifabuDir = dirname.Replace("temp/", "待发布/");
            if (Directory.Exists(daifabuDir))
                Directory.Delete(daifabuDir, true);
            Directory.Move(dirname, daifabuDir);

        }


        public int[] GetRandomNum(int Number, int minNum, int maxNum)
        {
            int j;
            int[] b = new int[Number];
            Random r = new Random();
            for (j = 0; j < Number; j++)
            {
                int i = r.Next(minNum, maxNum + 1);
                int num = 0;
                for (int k = 0; k < j; k++)
                {
                    if (b[k] == i)
                    {
                        num = num + 1;
                    }
                }
                if (num == 0)
                {
                    b[j] = i;
                }
                else
                {
                    j = j - 1;
                }
            }
            return b;
        }




        private void btn_LoginTest_Click(object sender, EventArgs e)
        {
            string posturl = "https://login.taobao.com/member/login.jhtml?redirectURL=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1";
            string poststr = "TPL_username=tianmin200&TPL_password=tianmin3216779236&ncoSig=&ncoSessionid=&ncoToken=8bb84c14acdeac1ea8a25bd93ff9b3e8141bc70e&slideCodeShow=false&useMobile=false&lang=zh_CN&loginsite=0&newlogin=&TPL_redirect_url=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1&from=alimama&fc=default&style=mini&css_style=&keyLogin=false&qrLogin=true&newMini=false&newMini2=true&tid=&loginType=3&minititle=&minipara=&pstrong=&sign=&need_sign=&isIgnore=&full_redirect=true&sub_jump=&popid=&callback=&guf=&not_duplite_str=&need_user_id=&poy=&gvfdcname=&gvfdcre=&from_encoding=&sub=false&TPL_password_2=&loginASR=1&loginASRSuc=0&allp=&oslanguage=&sr=&osVer=&naviVer=&osACN=&osAV=&osPF=&miserHardInfo=&appkey=&bind_login=false&bind_token=&nickLoginLink=&mobileLoginLink=https%3A%2F%2Flogin.taobao.com%2Fmember%2Flogin.jhtml%3Fstyle%3Dmini%26newMini2%3Dtrue%26from%3Dalimama%26redirectURL%3Dhttp%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1%26full_redirect%3Dtrue%26disableQuickLogin%3Dtrue%26useMobile%3Dtrue";
            CookieContainer cc = new CookieContainer();
            string refer = "http://login.taobao.com/member/taobaoke/login.htm/is_login=1";
            string result = HttpHelper1.SendDataByPost(posturl, poststr, refer, ref cc);
            string strurl = "https://www.alimama.com/membersvc/my.htm?domain=taobao&service=user_on_taobao&sign_account=a49f9f62502fc96517a626ea7e685eb4";
            string resultstr = HttpHelper1.SendDataByGET(strurl, ref cc);
            bool isLogin = Alimama.TestLogin(cc);

        }

        private void btn_NanzhuangCol_Click(object sender, EventArgs e)
        {
            ArrayList parms = new ArrayList();
            parms.Add("");
            parms.Add(1);
            alimamaThread = new Thread(new ParameterizedThreadStart(StartColNanzhuang));
            alimamaThread.Start(parms);
        }
        public void StartColNanzhuang(object parms)
        {
            while (true)
            {
                SetStartAlimamaBtnStatus(true);
                try
                {
                    int pagenum = Convert.ToInt32(this.nud_Pages.Value);
                    for (int i = 1; i < pagenum + 1; i++)
                    {
                        AppenAlimamaCmd("开始抓取第" + i.ToString() + "页微博内容");

                        string[] weiboAccounts = File.ReadAllLines("config/weiboAccounts.txt");

                        weibocc = WeiboHandler.InitWeiboCookie(weiboAccounts[0].Split(',')[0]);

                        CookieCollection ccl = weibocc.GetCookies(new Uri("http://weibo.com"));


                        string[] urls = File.ReadAllLines(("config/colweibo.txt"));
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
                                if (mblog.Text.Contains("链接都在评论第一条")) continue;
                                int isHave = Convert.ToInt32(SQLiteHelper.ExecuteScalar("select count(*) from mblog where id=" + mid));
                                if (isHave > 0)
                                    continue;//如果数据已存在，则跳过
                                string[] strs = mblog.Text.Split('地');
                                string commentstr = strs[1];
                                string dirname = strs[0].Trim();//截取选品文件夹名称
                                string links = "";//多链接以,分割
                                int j = 1;
                                string own_tbklinks = "";
                                string couponlinks = "粉丝优惠购：";
                                bool isCoupon = false;
                                #region 九宫格淘宝客处理
                                if (commentstr.Contains("图1："))
                                {
                                    //评论包含淘宝客链接

                                    Hashtable tbklinks = new Hashtable();
                                    Regex reg = new Regex(@"[a-zA-z]+://[^\s]*");
                                    MatchCollection mc = reg.Matches(commentstr);
                                    //创建选品文件夹
                                    try
                                    {
                                        if (!Directory.Exists("temp/"))
                                            Directory.CreateDirectory("temp");
                                        if (!Directory.Exists("temp/" + dirname + "/"))
                                            Directory.CreateDirectory("temp/" + dirname + "/");
                                    }
                                    catch (Exception ex)
                                    {
                                        dirname = mblog.Id;
                                        if (!Directory.Exists("temp/" + dirname + "/"))
                                            Directory.CreateDirectory("temp/" + dirname + "/");
                                    }
                                    foreach (Match m in mc)
                                    {
                                        string weiboshortlink = m.Value.Replace("\"", "");
                                        if (!weiboshortlink.StartsWith("http://t.cn")) continue;
                                        links = links + weiboshortlink + ",";
                                        string tbrealitem = "";
                                        string result = Alimama.GetItemResultWithWeiboShortUrl(weiboshortlink, alimamacc, ref tbrealitem);

                                        if (tbrealitem.Contains("http://ai.taobao.com"))
                                        {
                                            int staresult = SQLiteHelper.ExecuteNonQuery("insert into mblog(id,Source,Text,CreateAt,TbkLinks)values(@id,@Source,@Text,@CreateAt,@TbkLinks)", new[] {
                                                    mblog.Id,
                                                    mblog.Source,
                                                    mblog.Text,
                                                    mblog.CreatedAt,
                                                    own_tbklinks
                                                 });
                                            break;
                                        }
                                        if (alimamacc == null)
                                            alimamacc = Alimama.GetCookieContainer();
                                        string siteid = "20116380";
                                        string adzoneId = "70602305";
                                        DEWeiboAccount deweiboaccount = WeiboHandler.GetOneAccount();
                                        string own_tbklink = Alimama.GetTbkLink(tbrealitem, deweiboaccount.Siteid, deweiboaccount.Adzoneid, alimamacc);//把淘宝客链接更换为自己链接       
                                        if (own_tbklink == "" || own_tbklink == "链接不支持转化")
                                        {
                                            int staresult = SQLiteHelper.ExecuteNonQuery("insert into mblog(id,Source,Text,CreateAt,TbkLinks)values(@id,@Source,@Text,@CreateAt,@TbkLinks)", new[] {
                                                    mblog.Id,
                                                    mblog.Source,
                                                    mblog.Text,
                                                    mblog.CreatedAt,
                                                    own_tbklinks
                                                 });
                                            break;//没有淘宝客链接，当前内容不可用，直接退出循环
                                        }
                                        else if (own_tbklink == "阿里妈妈登录失效")
                                        {
                                            AppenAlimamaCmd("阿里妈妈登录失效，停止抓取！！！正在重新登录");
                                            alimamacc = Alimama.Login();
                                            if (Alimama.TestLogin(alimamacc))
                                                AppenAlimamaCmd("阿里妈妈重新登录成功！");
                                            else
                                            {
                                                AppenAlimamaCmd("阿里妈妈重新登录失败，停止抓取！");
                                                return;
                                            }

                                        }
                                        //string bdshorturl = HttpHelper1.GetBdShortUrl(own_tbklink);
                                        //bdshorturl = bdshorturl.Replace("\\", "");//包装一层百度短网址，防屏蔽  1.16 加一层跳转之后，会被微博反垃圾提示危险网址
                                        string searchresult = Alimama.SearchItem(tbrealitem, alimamacc);
                                        bool isSuc = Alimama.ApplyCampaign(searchresult, tbrealitem, alimamacc);//申请定向计划

                                        //获取优惠券信息
                                        if (!searchresult.Contains("couponInfo\":\"无"))
                                        {
                                            string couponshorturl = Alimama.GetCouponInfo(searchresult, tbrealitem, alimamacc);
                                            string couponweiboShortlink = WeiboHandler.GetWeiboShorturl(couponshorturl);//微博短地址
                                            couponlinks += "图" + j.ToString() + ":" + couponweiboShortlink + " ";
                                            isCoupon = true;
                                        }

                                        string weiboShortlink = WeiboHandler.GetWeiboShorturl(own_tbklink);//微博短地址
                                        own_tbklinks += "图" + j.ToString() + ":" + weiboShortlink + " ";

                                        j++;
                                    }//单个链接处理完成
                                    if (j == 10)//只有九图才添加comment.txt
                                    {
                                        //直接下载微博配图，截掉水印
                                        int picnum = 1;
                                        foreach (Pic pic in mblog.Pics)
                                        {
                                            string savepicurl = "temp/" + dirname + "/" + picnum.ToString() + ".jpg";
                                            string picurl = "http://wx2.sinaimg.cn/large/" + pic.Pid + ".jpg";
                                            HttpHelper1.HttpDownloadFile(picurl, "temp/" + dirname + "/" + picnum.ToString() + ".jpgtemp", alimamacc);
                                            Image img = Image.FromFile(savepicurl + "temp");

                                            //截取水印
                                            Bitmap bitmap = new Bitmap(img);
                                            bitmap = ImageHelper.Cut(bitmap, 0, 0, img.Width, img.Height - 20);
                                            bitmap.Save(savepicurl);
                                            bitmap.Dispose();
                                            img.Dispose();
                                            File.Delete(savepicurl + "temp");
                                            picnum++;
                                        }
                                        File.WriteAllText("temp/" + dirname + "/comment.txt", own_tbklinks);
                                        if (isCoupon)
                                            File.WriteAllText("temp/" + dirname + "/coupon.txt", couponlinks);
                                        File.WriteAllText("temp/" + dirname + "/id.txt", mblog.Id);
                                        AppenAlimamaCmd("抓取成功，微博正文：" + mblog.Text + ";ID：" + mblog.Id);
                                        try
                                        {
                                            //如果文件夹重名，先删除老的文件夹
                                            if (Directory.Exists("待发布/" + dirname))
                                                Directory.Delete("待发布/" + dirname, true);
                                            Directory.Move("temp/" + dirname, "待发布/" + dirname);//下載完成放入待发布库
                                        }
                                        catch (Exception)
                                        {

                                        }
                                    }
                                }
                                #endregion
                                int statu = SQLiteHelper.ExecuteNonQuery("insert into mblog(id,Source,Text,CreateAt,TbkLinks)values(@id,@Source,@Text,@CreateAt,@TbkLinks)", new[] {
                            mblog.Id,
                            mblog.Source,
                            mblog.Text,
                            mblog.CreatedAt,
                            own_tbklinks
                            });//选品库下载完成后，保存微博数据到数据库
                            }
                        }

                    }
                    int alimamajiange = Convert.ToInt32(this.nud_alimamajiange.Value);
                    AppenAlimamaCmd("此次抓取完毕，等待" + alimamajiange.ToString() + "分钟");
                    Thread.Sleep(alimamajiange * 60 * 1000);
                }
                catch (Exception ex)
                {
                    AppenAlimamaCmd(ex.Message);

                }
            }

        }

        private void timer_KeepAlimamaAlive_Tick(object sender, EventArgs e)
        {
            alimamacc = Alimama.Login();
        }

        private void bnt_ColSingelWeibo_Click(object sender, EventArgs e)
        {
            if (this.txt_SingelWeibo.Text == "") return;
            string url = this.txt_SingelWeibo.Text;
            SignalWeiboData weibodata = WeiboHandler.GetSignalWeibo(url);

            string dirname = "temp/" + NoHTML(weibodata.status.text);

            if (!Directory.Exists(dirname))
                Directory.CreateDirectory(dirname);
            int picnum = 1;
            foreach (string picid in weibodata.status.pic_ids)
            {
                string picurl = "http://wx2.sinaimg.cn/large/" + picid + ".jpg";
                HttpHelper1.HttpDownloadFile(picurl, dirname + "/" + picnum.ToString() + ".jpg", weibocc);
                picnum++;
            }
            WeiboComment comment = WeiboHandler.GetComment(weibodata.status.mid, weibocc);

            Directory.Move(dirname, dirname.Replace("temp/", "待发布/"));
            //Directory.Delete(dirname, true);

        }
        public string NoHTML(string Htmlstring)  //替换HTML标记
        {
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<img[^>]*>;", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            //Htmlstring = HttpUtility.UrlEncode(Htmlstring).Trim();
            return Htmlstring;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ArrayList parms = new ArrayList();
            parms.Add("");
            parms.Add(1);
            alimamaThread = new Thread(new ParameterizedThreadStart(StartCol));
            alimamaThread.Start(parms);
        }
        public void StartCol(object parms)
        {
            while (true)
            {

                SetStartAlimamaBtnStatus(true);
                int pagenum = Convert.ToInt32(this.nud_Pages.Value);
                for (int i = 1; i < pagenum + 1; i++)
                {
                    AppenAlimamaCmd("开始抓取第" + i.ToString() + "页微博内容");

                    //string[] weiboAccounts = File.ReadAllLines("config/weiboAccounts.txt");

                    CookieContainer weibocc = new CookieContainer();
                    string[] urls = File.ReadAllLines(("config/colweibo.txt"));
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
                            if (mblog.Text.Contains("想瘦")) continue;
                            if (mblog.Text.Contains("瘦")) continue;
                            if (mblog.Text.Contains("祛痘")) continue;
                            if (mblog.Text.Contains("减肥")) continue;
                            if (mblog.Text.Contains("关注")) continue;
                            bool isfilter = false;
                            if (File.Exists("config/filter.txt"))
                            {
                                string[] filterstrs = File.ReadAllLines("config/filter.txt");
                                foreach (string filterstr in filterstrs)
                                {
                                    if (mblog.Text.Contains(filterstr))
                                    {
                                        isfilter = true;
                                        break;
                                    }
                                }
                            }
                            if (isfilter) continue;
                            string mid = card.Mblog.Id;
                            string dirname = HttpHelper1.NoHTML(mblog.Text).Trim();//截取选品文件夹名称
                            try
                            {
                                int isHave = Convert.ToInt32(SQLiteHelper.ExecuteScalar("select count(*) from mblog where id=" + mid));
                                if (isHave > 0)
                                    continue;//如果数据已存在，则跳过


                                int picnum = 1;

                                dirname = dirname.Replace("\"", "");
                                if (!Directory.Exists("temp/" + dirname))
                                    Directory.CreateDirectory("temp/" + dirname);
                                foreach (Pic pic in mblog.Pics)
                                {
                                    AppenAlimamaCmd("下载图" + picnum.ToString());
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
                                AppenAlimamaCmd("单条微博抓取完成！");
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
                                AppenAlimamaCmd(ex.Message);
                                if (Directory.Exists("temp/" + dirname))
                                    Directory.Delete("temp/" + dirname, true);
                            }
                        }
                    }
                }
                int alimamajiange = Convert.ToInt32(this.nud_alimamajiange.Value);
                AppenAlimamaCmd("此次抓取完毕，等待" + alimamajiange.ToString() + "分钟");
                Thread.Sleep(alimamajiange * 60 * 1000);
            }
        }

        private void btn_sendTuwen_Click(object sender, EventArgs e)
        {
            ArrayList parms = new ArrayList();
            parms.Add("");
            parms.Add(1);
            weiboThread = new Thread(new ParameterizedThreadStart(StartSendTuwen));
            weiboThread.Start(parms);
        }
        /// <summary>
        /// 异步自动监控选品库并发送微博
        /// </summary>
        /// <param name="parmsobj"></param>
        public void StartSendTuwen(object parmsobj)
        {
            int yanchi = (Int32)this.nud_yanchi.Value;
            AppenWeiboCmd("延迟" + yanchi.ToString() + "分钟启动发布");
            Thread.Sleep(yanchi * 60 * 1000);

            SetStartWeiboBtnStatus(true);
            string[] weiboAccounts = File.ReadAllLines("config/weiboAccounts.txt");

            while (true)
            {
                int accountnum = 0;
                if (DEWeiboAccounts == null) InitWeiboAccounts();
                foreach (DEWeiboAccount deweiboaccount in DEWeiboAccounts)
                {
                    #region 初始定义
                    bool isSendSuc = false;//是否发布成功
                    string mid = "";
                    string ouid = "";
                    #endregion
                    DirectoryInfo TheFolder = new DirectoryInfo("待发布");
                    while (TheFolder.GetDirectories().Length == 0)
                    {
                        AppenWeiboCmd("没有找到发布素材，等待10分钟！");
                        Thread.Sleep(10 * 60 * 1000);
                    }
                    DirectoryInfo nextFolder = TheFolder.GetDirectories()[0];//发布选品库第一条
                    try
                    {
                        #region 初始化微博账号
                        string username = deweiboaccount.Username;
                        string password = deweiboaccount.Password;
                        string nickname = deweiboaccount.Nickname;
                        AppenWeiboCmd("[" + nickname + "]开始发布微博");
                        #endregion
                        #region 验证登陆并获取refer
                        string strresult = "";
                        CookieContainer weibocc = WeiboHandler.InitWeiboCookie(deweiboaccount.Username);
                        if (!WeiboHandler.TestLogin(weibocc, ref strresult))
                        {
                            //如果账号未登录，先自动登录
                            AppenWeiboCmd("账号:[" + nickname + "]未登录，开始登录！！！");
                            weibocc = WeiboHandler.Login(username, password);
                            if (weibocc == null)
                            {
                                AppenWeiboCmd("账号:[" + nickname + "]登录失败,跳过");
                                continue;
                            }
                            InitWeiboAccounts();//登录之后初始化账号
                        }

                        string userid = "";
                        HttpHelper1.GetStringInTwoKeyword(strresult, ref userid, "$CONFIG['watermark']='", "$CONFIG['domain']", 0);
                        userid = userid.Replace("';", "").Trim();
                        string refer = "http://www.weibo.com/" + userid + "/home?wvr=5";
                        deweiboaccount.Userid = userid.Replace("u/", "");

                        #endregion
                        #region 上传图片文件并获取图片picid
                        //遍历选品库文件夹

                        AppenWeiboCmd("共找到待发布素材 " + TheFolder.GetDirectories().Length.ToString() + "个");
                        //遍历文件夹，上传九图，组成图片ID参数
                        while (TheFolder.GetDirectories().Length == 0)
                        {
                            AppenWeiboCmd("暂时没有待发布素材，等待10分钟");
                            Thread.Sleep(10 * 60 * 1000);

                            TheFolder = new DirectoryInfo("待发布");
                        }


                        string picids = "";
                        ArrayList commentlist = new ArrayList();
                        string weibotext = nextFolder.Name;//微博正文
                        int picnum = 0;


                        foreach (FileInfo NextFile in nextFolder.GetFiles())
                        {

                            if (NextFile.Extension == ".jpg" || NextFile.Extension == ".gif")
                            {
                                picnum++;
                                AppenWeiboCmd("上传图片" + picnum.ToString());
                                string picpath = NextFile.FullName;
                                string uploadresult = WeiboHandler.uploadWeiboImage(picpath, deweiboaccount.Nickname, weibocc);
                                string picid = "";
                                HttpHelper1.GetStringInTwoKeyword(uploadresult, ref picid, "pid\":\"", "\"", 0);
                                picids = picids + picid + " ";
                            }
                        }
                        picids = picids.Trim().Replace(" ", "%20");
                        #endregion
                        #region 发布微博
                        AppenWeiboCmd("发布微博，正文：" + weibotext);


                        //string result = WeiboHandler.SendWeiboFromM(weibotext, picids, refer, deweiboaccount, weibocc);
                        string result = WeiboHandler.SendWeibo(weibotext, picids, refer, weibocc);

                        if (result.Contains("{\"code\":\"100000\""))
                        //if (result.Contains("ok\":1"))
                        {
                            AppenWeiboCmd("微博发布成功");
                            //HttpHelper1.GetStringInTwoKeyword(result, ref ouid, "ouid=", "\\\"", 0);
                            //HttpHelper1.GetStringInTwoKeyword(result, ref mid, "action-data=\\\"mid=", "&from", 0);
                        }
                        else if (result.Contains("{\"code\":\"100004\""))
                        {
                            //提示100004，重试发布
                            result = WeiboHandler.SendWeiboFromM(weibotext, picids, refer, deweiboaccount, weibocc);
                            if (result.Contains("ok\":1"))
                            {
                                AppenWeiboCmd("微博发布成功");
                            }
                            else
                            {
                                AppenWeiboCmd("微博发布失败，返回：result=" + result);
                                continue;
                            }
                        }
                        #endregion
                        isSendSuc = true;
                    }
                    catch (Exception ex)
                    {
                        int jiangeshij = Convert.ToInt32(this.nud_weiboJiange.Value);
                        AppenWeiboCmd("发布微博出错，错误信息：" + ex.Message);
                        AppenWeiboCmd("间隔" + jiangeshij.ToString() + "分钟继续");
                        isSendSuc = false;
                        Thread.Sleep(jiangeshij * 60 * 1000);
                    }
                    #region 微博发布后处理
                    //string newdir = nextFolder.FullName.Replace("待发布", "已发布");
                    //nextFolder.MoveTo(newdir);//移动文件夹至“已发布”
                    nextFolder.Delete(true);
                    if (!isSendSuc && mid != "") WeiboHandler.DeleteWeibo(mid, weibocc);
                    accountnum++;
                    if (accountnum == DEWeiboAccounts.Count) continue;//如果是最后一个账号，则不间隔

                    AppenWeiboCmd("[" + deweiboaccount.Nickname + "]发布完成!账号间 间隔2分钟");
                    Thread.Sleep(2 * 60 * 1000);//账号间间隔3分钟
                    #endregion
                }//foreach账号
                int jiange = Convert.ToInt32(this.nud_weiboJiange.Value);
                AppenWeiboCmd("全部微博账号发布完成，间隔" + jiange.ToString() + "分钟！！！");
                Thread.Sleep(jiange * 60 * 1000);
            }//while
        }

        private void btn_SignelWeibo_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.SelectedPath = Directory.GetCurrentDirectory() + "\\待发布";
            this.folderBrowserDialog1.ShowDialog();
            
            string nextFolderPath = this.folderBrowserDialog1.SelectedPath;
            bool iscomment = false;
            string commentstr = "";
            
            string[] weiboAccounts = File.ReadAllLines("config/weiboAccounts.txt");


            int accountnum = 0;
            if (DEWeiboAccounts == null) InitWeiboAccounts();
            foreach (DEWeiboAccount deweiboaccount in DEWeiboAccounts)
            {
                string curweibo = this.lvwWeiboAccountList.SelectedItems[0].SubItems[1].Text;
                if (deweiboaccount.Nickname != curweibo) continue;
                #region 初始定义
                bool isSendSuc = false;//是否发布成功
                string mid = "";
                string ouid = "";
                #endregion
                DirectoryInfo TheFolder = new DirectoryInfo("待发布");
                while (TheFolder.GetDirectories().Length == 0)
                {
                    AppenWeiboCmd("没有找到发布素材，等待10分钟！");
                    Thread.Sleep(10 * 60 * 1000);
                }
                //DirectoryInfo nextFolder = TheFolder.GetDirectories()[0];//发布选品库第一条
                DirectoryInfo nextFolder = new DirectoryInfo(nextFolderPath);
                try
                {
                    #region 初始化微博账号
                    string username = deweiboaccount.Username;
                    string password = deweiboaccount.Password;
                    string nickname = deweiboaccount.Nickname;
                    AppenWeiboCmd("[" + nickname + "]开始发布微博");
                    #endregion
                    #region 验证登陆并获取refer
                    string strresult = "";
                    CookieContainer weibocc = WeiboHandler.InitWeiboCookie(deweiboaccount.Username);
                    if (!WeiboHandler.TestLogin(weibocc, ref strresult))
                    {
                        //如果账号未登录，先自动登录
                        AppenWeiboCmd("账号:[" + nickname + "]未登录，开始登录！！！");
                        weibocc = WeiboHandler.Login(username, password);
                        if (weibocc == null)
                        {
                            AppenWeiboCmd("账号:[" + nickname + "]登录失败,跳过");
                            continue;
                        }
                        InitWeiboAccounts();//登录之后初始化账号
                    }

                    string userid = "";
                    HttpHelper1.GetStringInTwoKeyword(strresult, ref userid, "$CONFIG['watermark']='", "$CONFIG['domain']", 0);
                    userid = userid.Replace("';", "").Trim();
                    string refer = "http://www.weibo.com/" + userid + "/home?wvr=5";
                    deweiboaccount.Userid = userid.Replace("u/", "");

                    #endregion
                    #region 上传图片文件并获取图片picid
                    //遍历选品库文件夹

                    AppenWeiboCmd("共找到待发布素材 " + TheFolder.GetDirectories().Length.ToString() + "个");
                    //遍历文件夹，上传九图，组成图片ID参数
                    while (TheFolder.GetDirectories().Length == 0)
                    {
                        AppenWeiboCmd("暂时没有待发布素材，等待10分钟");
                        Thread.Sleep(10 * 60 * 1000);

                        TheFolder = new DirectoryInfo("待发布");
                    }


                    string picids = "";
                    ArrayList commentlist = new ArrayList();
                    string weibotext = nextFolder.Name;//微博正文
                    int picnum = 0;


                    foreach (FileInfo NextFile in nextFolder.GetFiles())
                    {

                        if (NextFile.Extension == ".jpg" || NextFile.Extension == ".gif")
                        {
                            picnum++;
                            AppenWeiboCmd("上传图片" + picnum.ToString());
                            string picpath = NextFile.FullName;
                            string uploadresult = WeiboHandler.uploadWeiboImage(picpath, deweiboaccount.Nickname, weibocc);
                            string picid = "";
                            HttpHelper1.GetStringInTwoKeyword(uploadresult, ref picid, "pid\":\"", "\"", 0);
                            picids = picids + picid + " ";
                        }
                        else if (NextFile.Name.Contains("comment.txt"))
                        {
                            iscomment = true;
                            commentstr = File.ReadAllText(NextFile.FullName);
                            commentlist.Add(commentstr);
                        }
                    }
                    picids = picids.Trim().Replace(" ", "%20");
                    #endregion
                    #region 发布微博
                    AppenWeiboCmd("发布微博，正文：" + weibotext);


                    //string result = WeiboHandler.SendWeiboFromM(weibotext, picids, refer, deweiboaccount, weibocc);
                    string result = WeiboHandler.SendWeibo(weibotext, picids, refer, weibocc);

                    if (result.Contains("{\"code\":\"100000\""))
                    //if (result.Contains("ok\":1"))
                    {
                        AppenWeiboCmd("微博发布成功");
                        HttpHelper1.GetStringInTwoKeyword(result, ref ouid, "ouid=", "\\\"", 0);
                        HttpHelper1.GetStringInTwoKeyword(result, ref mid, "action-data=\\\"mid=", "&from", 0);
                    }
                    else if (result.Contains("{\"code\":\"100004\""))
                    {
                        //提示100004，重试发布
                        result = WeiboHandler.SendWeiboFromM(weibotext, picids, refer, deweiboaccount, weibocc);
                        if (result.Contains("ok\":1"))
                        {
                            AppenWeiboCmd("微博发布成功");
                        }
                        else
                        {
                            AppenWeiboCmd("微博发布失败，返回：result=" + result);
                            continue;
                        }
                    }
                    #endregion
                    #region 添加评论
                    if (!iscomment)
                    {
                        AppenWeiboCmd("无需发布评论");
                    }
                    else
                    {
                        AppenWeiboCmd("发布评论");
                        while (mid == "")
                        {
                            //http://m.weibo.cn/container/getIndex?type=uid&value=5249378691&containerid=1005055249378691
                            string quanbuweibourl = "http://m.weibo.cn/container/getIndex?uid=" + deweiboaccount.Userid + "&luicode=10000011&lfid=100103type%3D3%26q%3D" + HttpUtility.UrlEncode(deweiboaccount.Nickname) + "&type=uid&value=" + deweiboaccount.Userid;
                            string containerid = "";
                            string result1 = HttpHelper1.SendDataByGET(quanbuweibourl, ref weibocc);
                            HttpHelper1.GetStringInTwoKeyword(result1, ref containerid, "weibo\",\"containerid\":\"", "\",", 0);
                            containerid = containerid.Replace(",\"containerid\":\"", "").Replace("\",", "");
                            quanbuweibourl = quanbuweibourl + "&containerid=" + containerid;
                            string result2 = HttpHelper1.SendDataByGET(quanbuweibourl, ref weibocc);
                            MblogData mbloglist1 = Newtonsoft.Json.JsonConvert.DeserializeObject<MblogData>(result2);
                            foreach (Card card in mbloglist1.Cards)
                            {
                                Mblog mblog1 = card.Mblog;
                                if (mblog1.Mblogtype != 0)
                                    continue;
                                if (picids.StartsWith(mblog1.Pics[0].Pid))
                                    mid = mblog1.Id;
                                break;
                            }
                            //如果Mid为空，直接读取微博第一条Mid
                        }



                        //if (commentstr.StartsWith("图1") || commentstr.Contains("粉丝优惠购"))
                        //{
                        //    //牺牲发布效率，把每个微博的渠道数据进行监控，找出不那么赚钱的渠道
                        //    Regex reg = new Regex(@"[a-zA-z]+://[^\s]*");
                        //    MatchCollection mc = reg.Matches(commentstr);
                        //    foreach (Match m in mc)
                        //    {
                        //        string weiboshortlink = m.Value;
                        //        string tbrealitem = "";
                        //        Alimama.GetItemResultWithWeiboShortUrl(weiboshortlink, alimamacc, ref tbrealitem);
                        //        string targetweibourl = GetWeiboShortUrlByTbItem(tbrealitem, deweiboaccount.Siteid, deweiboaccount.Adzoneid, alimamacc);
                        //        if (targetweibourl != "")
                        //        {
                        //            commentstr = commentstr.Replace(m.Value, targetweibourl);
                        //        }
                        //    }
                        //}
                        string commentresult = WeiboHandler.Comment(mid, ouid, commentstr, refer, weibocc);
                        string decode_commentresult = Regex.Unescape(commentresult);
                        if (!decode_commentresult.Contains("抱歉"))
                        {
                            if (!commentresult.Contains("{\"code\":\"100000\""))
                            {
                                //评论失败之后，再次评论
                                commentresult = WeiboHandler.Comment(mid, ouid, "地址 " + commentstr, refer, weibocc);
                            }
                            if (!commentresult.Contains("{\"code\":\"100000\""))
                            {
                                //第二次评论尝试
                                commentresult = WeiboHandler.Comment(mid, ouid, "链接 " + commentstr, refer, weibocc);
                            }
                            if (!commentresult.Contains("{\"code\":\"100000\""))
                            {
                                //第三次评论尝试
                                commentresult = WeiboHandler.Comment(mid, ouid, "购 " + commentstr, refer, weibocc);
                            }
                            //else
                            //{
                            //    //评论发布失败
                            //    string code = "";
                            //    HttpHelper1.GetStringInTwoKeyword(commentresult, ref code, "{\"code\":\"", "\"", 0);
                            //    AppenWeiboCmd("评论发布失败，code:" + code);
                            //}
                        }
                        if (commentresult.Contains("{\"code\":\"100000\""))
                            AppenWeiboCmd("评论发布成功");
                        else
                        {
                            AppenWeiboCmd("评论发布失败，返回值：" + commentresult);
                            int insertcomment_result = SQLiteHelper.ExecuteNonQuery("insert into MissComment(mid,ouid,uid,text)values(@mid,@ouid,@uid,@text)", new[] {
                            mid,
                            ouid,
                            username,
                            commentstr
                            });

                        }
                    }

                    #endregion
                    isSendSuc = true;
                }
                catch (Exception ex)
                {
                    int jiangeshij = Convert.ToInt32(this.nud_weiboJiange.Value);
                    AppenWeiboCmd("发布微博出错，错误信息：" + ex.Message);

                    isSendSuc = false;

                }
                #region 微博发布后处理
                //string newdir = nextFolder.FullName.Replace("待发布", "已发布");
                //nextFolder.MoveTo(newdir);//移动文件夹至“已发布”
                //nextFolder.Delete(true);
                if (!isSendSuc && mid != "") WeiboHandler.DeleteWeibo(mid, weibocc);
                accountnum++;
                

                AppenWeiboCmd("[" + deweiboaccount.Nickname + "]发布完成!");

                #endregion
            }//foreach账号


        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
