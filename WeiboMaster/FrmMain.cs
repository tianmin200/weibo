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

namespace WeiboMaster
{
    public partial class FrmMain : Form
    {
        private Thread weiboThread;
        private CookieContainer alimamacc;
        private CookieContainer weibocc;
        private ArrayList DEWeiboAccounts;
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            Mblog mblog = null;
            string mid = "";
            string ss = "";
            string url = "";
            string colresult = "";
            MblogData mbloglist = null;
            int isHave = 0;
            WeiboComment comment = null;
            string links = "";//多链接以,分割
            int j = 1;
            int linknum = 0;
            bool isCoupon = false;
            string couponlinks = "";

            string[] urls = File.ReadAllLines(("config/colweibo.txt"));
            string[] xiaohaos = File.ReadAllLines(("config/xiaohao.txt"));
            alimamacc = Alimama.Login();
            InitWeiboAccounts();
            weibocc = WeiboHandler.InitWeiboCookie(((DEWeiboAccount)DEWeiboAccounts[0]).Username, "config", 1);
            while (true)
            {
                foreach (string urltemp in urls)
                {
                    #region 抓取微博列表
                    url = urltemp + "1";
                    ss = "";
                    colresult = HttpHelper1.GetHttpsHtml(url, "", ref ss);
                    if (colresult.Contains(""))
                    {
                        colresult = colresult.Replace("page\":null", "page\":1");
                    }
                    mbloglist = Newtonsoft.Json.JsonConvert.DeserializeObject<MblogData>(colresult);
                    #endregion
                    #region 遍历处理需评论微博
                    foreach (Card card in mbloglist.Cards)
                    {
                        if (card.Mblog == null) continue;
                        mblog = card.Mblog;
                        mid = card.Mblog.Id;

                        isHave = Convert.ToInt32(SQLiteHelper.ExecuteScalar("select count(*) from mblog where id=" + mid));
                        if (isHave > 0)
                            continue;//如果数据已存在，则跳过

                        //先查看评论是否包含链接
                        comment = WeiboHandler.GetComment(mid, weibocc);
                        if (comment == null) continue;
                        
                        if (comment.Data.Html.Contains("图1："))
                        {
                            string tbklinks_html = "";
                            linknum = j = 1;
                            couponlinks = "";
                            HttpHelper1.GetStringInTwoKeyword(comment.Data.Html, ref tbklinks_html, "图1：", "<!-- 评论图片的处理 -->", 0);
                            Hashtable tbklinks = new Hashtable();
                            Regex reg = new Regex(@"[a-zA-z]+://[^\s]*");
                            MatchCollection mc = reg.Matches(tbklinks_html);
                            
                            foreach (Match m in mc)
                            {
                                linknum++;
                                string weiboshortlink = m.Value.Replace("\"", "");
                                if (!weiboshortlink.StartsWith("http://t.cn")) continue;
                                links = links + weiboshortlink + ",";
                                //转换淘客微博评论链接为自有ID优惠券链接
                                //获取代理IP

                                //通过代理IP登录微博
                                //通过代理IP发布评论

                                string tbrealitem = "";
                                string result = Alimama.GetItemResultWithWeiboShortUrl(weiboshortlink, alimamacc, ref tbrealitem);

                                if (tbrealitem.Contains("http://ai.taobao.com"))
                                {
                                    int staresult = SQLiteHelper.ExecuteNonQuery("insert into mblog(id,Source,Text,CreateAt,TbkLinks)values(@id,@Source,@Text,@CreateAt,@TbkLinks)", new[] {
                                                    mblog.Id,
                                                    mblog.Source,
                                                    mblog.Text,
                                                    mblog.CreatedAt,
                                                    ""
                                                 });
                                    break;
                                }



                                DEWeiboAccount deweiboaccount = WeiboHandler.GetOneAccount();
                                //string own_tbklink = Weibo.Common.Alimama.GetTbkLink(tbrealitem, deweiboaccount.Siteid, deweiboaccount.Adzoneid, alimamacc);//把淘宝客链接更换为自己链接       
                                //if (own_tbklink == "" || own_tbklink == "链接不支持转化")
                                //{
                                //    int staresult = SQLiteHelper.ExecuteNonQuery("insert into mblog(id,Source,Text,CreateAt,TbkLinks)values(@id,@Source,@Text,@CreateAt,@TbkLinks)", new[] {
                                //                        mblog.Id,
                                //                        mblog.Source,
                                //                        mblog.Text,
                                //                        mblog.CreatedAt,
                                //                        ""
                                //                     });
                                //    break;//没有淘宝客链接，当前内容不可用，直接退出循环
                                //}
                                //else if (own_tbklink == "阿里妈妈登录失效")
                                //{

                                //    alimamacc = Alimama.Login();
                                //    if (Alimama.TestLogin(alimamacc))
                                //        ;
                                //    else
                                //    {

                                //        return;
                                //    }

                                //}
                                string searchresult = Alimama.SearchItem(tbrealitem, alimamacc);
                                bool isSuc = Alimama.ApplyCampaign(searchresult, tbrealitem, alimamacc);//申请定向计划

                                //获取优惠券信息
                                if (!searchresult.Contains("couponInfo\":\"无"))
                                {
                                    string couponshorturl = Alimama.GetCouponInfo(searchresult, tbrealitem, alimamacc);
                                    string couponweiboShortlink = WeiboHandler.GetWeiboShorturl(couponshorturl);//微博短地址
                                    if(couponweiboShortlink != "")
                                        couponlinks += "图" + j.ToString() + ":" + couponweiboShortlink + " ";
                                    isCoupon = true;
                                }

                                //string weiboShortlink = WeiboHandler.GetWeiboShorturl(own_tbklink);//微博短地址
                                //own_tbklinks += "图" + j.ToString() + ":" + weiboShortlink + " ";

                                j++;
                            }

                        }

                        if (!couponlinks.Contains("图")) continue;
                        
                        //WebProxy proxy = DailiHelper.GetProxy();
                        //while (!DailiHelper.VerIP(proxy))
                        //{
                        //    proxy = DailiHelper.GetProxy();
                        //}
                        Random r = new Random();
                        string xiaohao = xiaohaos[r.Next(xiaohaos.Length)];
                        string xiaohaousername = xiaohao.Split(',')[0];
                        string xiaohaopassword = xiaohao.Split(',')[1];

                        string uid = "";
                        HttpHelper1.GetStringInTwoKeyword(url, ref uid, "uid=", "&", 0);
                        string refer = "http://www.weibo.com/u/" + uid + "/home?wvr=5";

                        CookieContainer xiaohaocc = WeiboHandler.InitWeiboCookie(xiaohaousername);
                        if(!WeiboHandler.TestLogin(xiaohaocc))
                            xiaohaocc = WeiboHandler.Login(xiaohaousername, xiaohaopassword);
                        while (xiaohaocc == null)
                        {

                            //取新号
                            xiaohao = xiaohaos[r.Next(xiaohaos.Length)];
                            xiaohaousername = xiaohao.Split(',')[0];
                            xiaohaopassword = xiaohao.Split(',')[1];
                            //proxy = DailiHelper.GetProxy();
                            xiaohaocc = WeiboHandler.InitWeiboCookie(xiaohaousername);
                            if (!WeiboHandler.TestLogin(xiaohaocc))
                                xiaohaocc = WeiboHandler.Login(xiaohaousername, xiaohaopassword);
                        }
                        couponlinks = couponlinks.Replace("图","");
                        string commentresult = WeiboHandler.Comment(mblog.Id, uid, couponlinks, refer, xiaohaocc);
                        string decode_commentresult = Regex.Unescape(commentresult);
                        if (!decode_commentresult.Contains("抱歉") && !decode_commentresult.Contains("广告"))
                        {
                            if (!commentresult.Contains("{\"code\":\"100000\""))
                            {
                                //评论失败之后，再次评论
                                commentresult = WeiboHandler.Comment(mid, uid, "地址 " + couponlinks, refer, weibocc);
                            }
                            if (!commentresult.Contains("{\"code\":\"100000\""))
                            {
                                //第二次评论尝试
                                commentresult = WeiboHandler.Comment(mid, uid, "链接 " + couponlinks, refer, weibocc);
                            }
                            if (!commentresult.Contains("{\"code\":\"100000\""))
                            {
                                //第三次评论尝试
                                commentresult = WeiboHandler.Comment(mid, uid, "购 " + couponlinks, refer, weibocc);
                            }
                        }
                        if (commentresult.Contains("{\"code\":\"100000\""))
                        {
                            int statu = SQLiteHelper.ExecuteNonQuery("insert into mblog(id,Source,Text,CreateAt,TbkLinks)values(@id,@Source,@Text,@CreateAt,@TbkLinks)", new[] {
                            mblog.Id,
                            mblog.Source,
                            mblog.Text,
                            mblog.CreatedAt,
                            couponlinks
                            });//选品库下载完成后，保存微博数据到数据库
                        }
                    }//单个mblog处理结束
                    #endregion
                }
                Thread.Sleep(10 * 60 * 1000);
            }

        }

        public void InitWeiboAccounts()
        {

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
                        File.AppendAllText("config/weibocookie/" + username + ".txt", loginData.cookies);

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

                    }

                    rownum++;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
