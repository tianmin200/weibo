using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Weibo;
using Weibo.Common;
using Weibo.Models;
using System.Net;
using System.Threading;
using Weibo.Core;
using Newtonsoft.Json;

namespace Weibo.Common
{
    public static class WeiboHandler
    {
        public static string GetWeiboShorturl(string url)
        {
            string shorturl = "";
            string encodeurl = HttpUtility.UrlEncode(url);
            string poststring = "url=" + encodeurl + "&d=1";
            string geturl = "http://api.t.sina.com.cn/short_url/shorten.json?source=2465536411&url_long=" + encodeurl;
            CookieContainer cc = new CookieContainer();
            string json = HttpHelper1.SendDataByGET(geturl, ref cc);
            HttpHelper1.GetStringInTwoKeyword(json, ref shorturl, "url_short\":\"", "\"", 0);
            return shorturl;
        }

        /// <summary>
        /// 获取新浪微博打码图片
        /// </summary>
        /// <param name="cc">新浪微博Cookie</param>
        /// <returns>图片存储路径</returns>
        public static string GetVcodePic(CookieContainer cc)
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
            string randomname = Guid.NewGuid().ToString();
            if (!Directory.Exists("pincode"))
                Directory.CreateDirectory("pincode");
            string filepath = "pincode/" + randomname + ".png";
            img.Save(filepath);
            img.Dispose();

            return filepath;
        }

        /// <summary>
        /// 测试登录
        /// </summary>
        /// <param name="cc">微博Cookie</param>
        /// <returns></returns>
        public static bool TestLogin(CookieContainer cc, ref string strresult)
        {
            string url = "http://www.weibo.com/";
            strresult = HttpHelper1.SendDataByGET(url, ref cc);
            if (!strresult.Contains("我的首页"))
            {
                return false;
            }
            return true;
        }
        public static bool TestLogin(CookieContainer cc)
        {
            string url = "http://www.weibo.com/";
            string strresult = HttpHelper1.SendDataByGET(url, ref cc);
            if (!strresult.Contains("我的首页"))
            {
                return false;
            }
            return true;
        }

        public static bool TestLogin(string username)
        {
            CookieContainer cc = InitWeiboCookie(username);
            if (cc == null) return false;
            string url = "http://www.weibo.com/";
            string strresult = HttpHelper1.SendDataByGET(url, ref cc);
            if (!strresult.Contains("我的首页"))
            {
                return false;
            }
            return true;
        }
        public static bool TestLogin(string username, ref CookieContainer cc)
        {
            cc = InitWeiboCookie(username);
            if (cc == null) return false;
            string url = "http://www.weibo.com/";
            string strresult = HttpHelper1.SendDataByGET(url, ref cc);
            if (!strresult.Contains("我的首页"))
            {
                return false;
            }
            return true;
        }
        public static bool TestLogin(string username, ref CookieContainer cc, ref string uid)
        {
            cc = InitWeiboCookie(username);
            if (cc == null) return false;
            string url = "http://www.weibo.com/";
            string strresult = HttpHelper1.SendDataByGET(url, ref cc);
            if (!strresult.Contains("我的首页"))
            {

                return false;
            }
            HttpHelper1.GetStringInTwoKeyword(strresult, ref uid, "$CONFIG['uid']='", "';", 0);
            return true;
        }

        /// <summary>
        /// 初始化本地Cookie
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public static CookieContainer InitWeiboCookie(string AccountUsername)
        {
            if (!File.Exists("weibocookie/" + AccountUsername + ".txt")) return null;
            string cookiestr = File.ReadAllText("weibocookie/" + AccountUsername + ".txt");
            CookieCollection ccl = CookieHelper.GetCookieCollectionByString(cookiestr, "weibo.com");
            CookieContainer weibocc = new CookieContainer();
            weibocc.MaxCookieSize = 100000;
            weibocc.Add(ccl);
            return weibocc;
        }
        /// <summary>
        /// 初始化本地Cookie
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public static CookieContainer InitWeiboCookie(string Username, string cookiestr)
        {
            CookieCollection ccl = CookieHelper.GetCookieCollectionByString(cookiestr, "weibo.com");
            CookieContainer weibocc = new CookieContainer();
            weibocc.MaxCookieSize = 100000;
            weibocc.Add(ccl);
            return weibocc;
        }

        /// <summary>
        /// 提交验证码验证
        /// </summary>
        /// <param name="vcode">验证码</param>
        /// <param name="refer"></param>
        /// <param name="weibocc"></param>
        /// <returns></returns>
        public static string GetVcodeRetcode(string vcode, string refer, CookieContainer weibocc)
        {
            string retcode = "";
            string yanzhengurl = "http://weibo.com/aj/pincode/verified?ajwvr=6";

            string vcode_urlencode = HttpUtility.UrlEncode(vcode);
            string yanzhengpost = "secode=" + vcode_urlencode + "&type=rule";
            string yanzhengresult = HttpHelper1.SendDataByPost(yanzhengurl, yanzhengpost, refer, ref weibocc);
            if (yanzhengresult.Contains("{\"code\":\"100000\""))
            {
                //验证通过
                HttpHelper1.GetStringInTwoKeyword(yanzhengresult, ref retcode, "retcode\":\"", "\"}}", 0);
                return retcode;
            }
            return retcode;
        }



        /// <summary>
        /// 发布单条微博
        /// </summary>
        /// <param name="weibotext">微博正文</param>
        /// <param name="picids">图片ID，空格连接</param>
        /// <param name="refer">来源地址</param>
        /// <param name="weibocc">Cookie</param>
        /// <returns></returns>
        public static string SendWeibo(string weibotext, string picids, string refer, CookieContainer weibocc)
        {

            string ticks = HttpHelper1.GetTicks();

            //PC端发布微博
            string posturl = "http://www.weibo.com/aj/mblog/add?ajwvr=6&__rnd=" + ticks;
            string poststr = "location=v6_content_home&text=" + System.Web.HttpUtility.UrlEncode(weibotext) + "&appkey=&style_type=1&pic_id=" + picids + "&pdetail=&gif_ids=&rank=0&rankid=&module=stissue&pub_source=main_&pub_type=dialog&_t=0";

            poststr = poststr.Replace("+", "%20");

            //移动端发布微博

            //string posturl = "http://m.weibo.cn/mblogDeal/addAMblog";
            //string poststr = "content=" + System.Web.HttpUtility.UrlEncode(weibotext) + "&picId="+picids+"&annotations=&st=12283f";

            string result = HttpHelper1.SendDataByPost(posturl, poststr, refer, ref weibocc);
            //判断是否发布成功
            //{"code":"100000"
            return result;
        }

        public static string SendWeibo(string weibotext, string picids, string refer, DEWeiboAccount deweiboaccount)
        {

            return SendWeibo(weibotext, picids, refer, deweiboaccount.Cookie);
        }

        public static string GetSTFromM(CookieContainer cc)
        {
            string url = "http://m.weibo.com";

            CookieCollection ccl = cc.GetCookies(new Uri("http://weibo.com"));
            CookieCollection newccl = ccl;
            for (int i = 0; i < newccl.Count; i++)
            {
                newccl[i].Domain = "weibo.cn";
            }
            CookieContainer newcc = new CookieContainer();
            newcc.Add(new Uri("http://weibo.cn"), newccl);
            string result = HttpHelper1.SendDataByGET(url, ref newcc);
            string st = "";
            HttpHelper1.GetStringInTwoKeyword(result, ref st, "st\":\"", "\"", 0);
            for (int i = 0; i < newccl.Count; i++)
            {
                newccl[i].Domain = "weibo.com";
            }
            return st;
        }

        public static CookieContainer GetMCookie(CookieContainer cc)
        {
            CookieCollection ccl = cc.GetCookies(new Uri("http://weibo.com"));

            for (int i = 0; i < ccl.Count; i++)
            {
                ccl[i].Domain = "weibo.cn";
            }
            CookieContainer newcc = new CookieContainer();
            newcc.Add(new Uri("http://weibo.cn"), ccl);
            return newcc;
        }
        /// <summary>
        /// 上传微博图片
        /// </summary>
        /// <param name="filepath">图片本地地址</param>
        /// <param name="ccl">Cookie</param>
        /// <returns></returns>
        public static string uploadWeiboImage(string filepath, string nickname, CookieContainer cc)
        {
            //filepath = "D://1.jpg";
            CookieCollection ccl = cc.GetCookies(new Uri("http://weibo.com"));
            FileStream file = new FileStream(filepath, FileMode.Open);
            byte[] bb = new byte[file.Length];
            file.Read(bb, 0, (int)file.Length);
            file.Close();
            //pictureBox2.Image = byteArrayToImage(bb);

            MsMultiPartFormData form = new MsMultiPartFormData();
            form.AddFormField("Filename", Path.GetFileName(filepath));
            form.AddStreamFile("filename", Path.GetFileName(filepath), bb);
            form.AddFormField("Upload", "Submit Query");
            form.PrepareFormData();

            form.GetFormData();
            string host = "weibo.com";
            string url = string.Format("http://picupload.service.weibo.com/interface/pic_upload.php?app=miniblog&data=1&url=0&markpos=0&logo=&nick=%40%E7%BE%8E%E5%88%B0%E5%BF%83%E7%A2%8E%E7%9A%84%E6%AE%B5%E5%AD%90&marks=0&url=0&markpos=0&logo=&nick=%40%E7%BE%8E%E5%88%B0%E5%BF%83%E7%A2%8E%E7%9A%84%E6%AE%B5%E5%AD%90&marks=0&mime=image/jpeg&ct=0.5615201024338603");
            url = "http://picupload.service.weibo.com/interface/pic_upload.php?app=miniblog&data=1&url=0&markpos=1&logo=&nick=0&marks=0&url=0&markpos=1&logo=&nick=0&marks=0&mime=image/jpeg&ct=0.31995245022699237";
            string ContentType = "application/octet-stream";

            HttpHelper help = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = url,
                CookieCollection = ccl,
                ResultCookieType = ResultCookieType.CookieCollection,
                Host = host,
                Accept = "*/*",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36 QIHU 360SE",
                ContentType = ContentType,
                Method = "POST",
                PostDataType = PostDataType.Byte,
                PostdataByte = form.GetFormData().ToArray(),
                Encoding = Encoding.UTF8,
                Referer = "http://js.t.sinajs.cn/t6/home/static/swf/MultiFilesUpload.swf?version=5253619b64983021",
                ProxyIp = "ieproxy"
            };

            //item.Header.Add("Pragma", "no-cache");
            //item.Header.Add("DNT", "1");
            HttpResult result = help.GetHtml(item);
            //this.BeginInvoke(updateLog, "图片上传结果：" + result.Html);
            return result.Html;
        }
        public static string uploadWeiboImage(string filepath, DEWeiboAccount deweiboaccount)
        {
            return uploadWeiboImage(filepath, deweiboaccount.Nickname, deweiboaccount.Cookie);
        }

        /// <summary>
        /// 微博预登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public static PreLoginResponseData PreLogin(string username)
        {
            try
            {
                string userNameBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(HttpUtility.UrlEncode(username)));
                string url = "http://login.sina.com.cn/sso/prelogin.php";
                PostHelper post = new PostHelper(url);
                post.Cookies = new CookieContainer();
                post.PostItems = PreLoginData.Create(userNameBase64);
                string result = post.Post();
                Regex re = new Regex("{.*}");
                if (re.IsMatch(result))
                {
                    var data = re.Match(result).ToString();
                    var responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<PreLoginResponseData>(data);
                    if (responseData != null)
                    {
                        responseData.cookies = PostHelper.GetAllCookies(post.Cookies);
                        return responseData;
                    }
                }
            }
            catch
            { }
            return null;
        }

        /// <summary>
        /// 登录微博
        /// </summary>
        /// <param name="data">预登录数据</param>
        /// <param name="username">微博账号</param>
        /// <param name="password">微博密码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public static LoginResponseData Login(PreLoginResponseData data, string username, string password, string code)
        {
            try
            {
                string userNameBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(HttpUtility.UrlEncode(username)));
                password = EncryptPassword(data, password);
                string url = "http://login.sina.com.cn/sso/login.php?client=ssologin.js(v1.4.18)&_=" + DateTime.Now.TimeStamp();
                PostHelper post = new PostHelper(url);
                post.Type = PostTypeEnum.Post;
                post.Cookies = new CookieContainer();
                post.Cookies.SetCookies(new Uri("http://weibo.com"), data.cookies);
                post.PostItems = LoginData.Create(data, userNameBase64, password, code);
                string result = post.Post();
                var responseData = JsonConvert.DeserializeObject<LoginResponseData>(result);
                if (responseData != null)
                {
                    responseData.cookies = PostHelper.GetAllCookies(post.Cookies);
                    return responseData;
                }
            }
            catch
            { }
            return null;
        }

        public static CookieContainer Login(string username, string password)
        {
            var preData = WeiboHandler.PreLogin(username);
            CookieContainer weibocc = new CookieContainer();
            if (preData != null)
            {
                string code = null;
                var img = WeiboHandler.GetLoginCodePic(preData.pcid);
                var loginData = WeiboHandler.Login(preData, username, password, code);
                weibocc = WeiboHandler.InitWeiboCookie(username, loginData.cookies);
                bool isLogin = WeiboHandler.TestLogin(weibocc);
                if (isLogin)
                {
                    //登录成功保存Cookie
                    File.AppendAllText("weibocookie/" + username + ".txt", loginData.cookies);
                    return weibocc;
                }
                else
                {
                    return null;
                }
            }
            else
            {

                return null;
            }

        }

        /// <summary>
        /// 发布微博评论
        /// </summary>
        /// <param name="mid">需要评论的微博mid</param>
        /// <param name="ouid">需要评论的微博ouid</param>
        /// <param name="commentstr">评论正文（不需要编码</param>
        /// <param name="cc">Cookie</param>
        /// <returns></returns>
        public static string Comment(string mid, string ouid, string commentstr, string refer, CookieContainer weibocc)
        {
            string commenturl = "http://weibo.com/aj/v6/comment/add?ajwvr=6&__rnd=" + HttpHelper1.GetTicks();
            commentstr = System.Web.HttpUtility.UrlEncode(commentstr.Trim());
            commentstr = commentstr.Trim().Replace("+", "%20");

            string commentpoststr = "act=post&mid=" + mid + "&uid=" + ouid + "&forward=0&isroot=0&content=" + commentstr + "&location=&module=scommlist&group_source=&pdetail=&_t=0";
            string commentresult = HttpHelper1.SendDataByPost(commenturl, commentpoststr, refer, ref weibocc);
            if (commentresult.Contains("{\"code\":\"100001\""))
            {
                //Thread.Sleep(10 * 60 * 1000);//如果第一次评论就提示发布相同内容，则将线程停顿10分钟
                return commentresult;//如果第一次评论就报错，一般认为是mid为空，直接返回不处理
            }

            if (commentresult.Contains("{\"code\":\"100027\""))
            {
                //需要输入验证码
                string retcode = "";
                string vcode = "";

                while (retcode == "")
                {
                    while (vcode == "" || vcode == "IERROR" || vcode == "ERROR")
                    {
                        vcode = HttpHelper1.AutoGetVcode(weibocc);
                    }
                    retcode = WeiboHandler.GetVcodeRetcode(vcode, refer, weibocc);
                    if (retcode == "") vcode = "";
                }//拿到验证码，并验证通过，通不过死循环

                commentpoststr = commentpoststr + "&retcode=" + retcode;
                commentresult = HttpHelper1.SendDataByPost(commenturl, commentpoststr, refer, ref weibocc);//再次发送评论
            }
            return commentresult;
        }
        public static WeiboComment GetComment(string mid, CookieContainer cc)
        {
            try
            {
                string commenturl = "http://weibo.com/aj/v6/comment/small?ajwvr=6&act=list&mid=" + mid + "&uid=";
                string result = HttpHelper1.SendDataByGET(commenturl, ref cc);
                WeiboComment comment = Newtonsoft.Json.JsonConvert.DeserializeObject<WeiboComment>(result);
                return comment;
            }
            catch (Exception)
            {

                return null;
            }

        }
        /// <summary>
        /// 微博密码加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static string EncryptPassword(PreLoginResponseData data, string password)
        {
            RSAHelper rsa = new RSAHelper();
            rsa.RSASetPublic(data.pubkey, "10001");
            return rsa.RSAEncrypt(data.servertime + "\t" + data.nonce + "\n" + password);
        }

        /// <summary>
        /// 获取微博登录验证码图片
        /// </summary>
        /// <param name="pcid">预登录pcid</param>
        /// <returns></returns>
        public static Image GetLoginCodePic(string pcid)
        {
            if (!string.IsNullOrEmpty(pcid))
            {
                try
                {
                    string url = string.Format("http://login.sina.com.cn/cgi/pin.php?r={0}&s=0&p={1}", DateTime.Now.TimeStamp(), pcid);
                    using (var stream = WebDownload.HttpDownload(url))
                    {
                        if (stream != null)
                        {
                            return Image.FromStream(stream);
                        }
                    }
                }
                catch
                { }
            }
            return null;
        }

        public static DEWeiboAccount GetOneAccount()
        {
            DEWeiboAccount deweiboAccount = new DEWeiboAccount();
            string[] weiboAccounts = File.ReadAllLines("weiboAccounts.txt");
            string[] weiboAccount = weiboAccounts[0].Split(',');
            string username = weiboAccount[0];
            string password = weiboAccount[1];
            string nickname = weiboAccount[2];
            string siteid = weiboAccount[3];
            string adzoneid = weiboAccount[4];

            deweiboAccount.Username = username;
            deweiboAccount.Password = password;
            deweiboAccount.Nickname = nickname;
            deweiboAccount.Islogin = true;

            deweiboAccount.Siteid = siteid;
            deweiboAccount.Adzoneid = adzoneid;
            return deweiboAccount;
        }
        /// <summary>
        /// 删除指定微博
        /// </summary>
        /// <param name="mid"></param>
        /// <param name="weibocc"></param>
        /// <returns></returns>
        public static bool DeleteWeibo(string mid, CookieContainer weibocc)
        {
            string url = "http://weibo.com/aj/mblog/del?ajwvr=6";
            string poststr = "mid=" + mid;
            string result = HttpHelper1.SendDataByPost(url, poststr, "", ref weibocc);
            if (result.Contains("100000")) return true;
            return false;
        }

        public static SignalWeiboData GetSignalWeibo(string weibourl)
        {
            SignalWeiboData weibo = new SignalWeiboData();
            CookieContainer weibocc = new CookieContainer();
            string result = HttpHelper1.SendDataByGET(weibourl,ref weibocc);
            string weibojson = "";
            HttpHelper1.GetStringInTwoKeyword(result, ref weibojson, "$render_data = [", "][0] || {};", 0);
            weibo = Newtonsoft.Json.JsonConvert.DeserializeObject<SignalWeiboData>(weibojson);
            return weibo;
        }
    }

    public class MsMultiPartFormData
    {
        private List<byte> formData;
        public String Boundary = "----------cH2cH2gL6GI3ae0Ef1ae0ae0ae0cH2";
        private String fieldName = "Content-Disposition: form-data; name=\"{0}\"";
        private String fileContentType = "Content-Type: {0}";
        private String fileField = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"";
        private Encoding encode = Encoding.GetEncoding("UTF-8");
        public MsMultiPartFormData()
        {
            formData = new List<byte>();
        }
        public void AddFormField(String FieldName, String FieldValue)
        {
            String newFieldName = "";
            newFieldName = string.Format("", "");
            //formData.AddRange(encode.GetBytes("--" + Boundary + "\r\n"));
            //formData.AddRange(encode.GetBytes(newFieldName + "\r\n\r\n"));
            //formData.AddRange(encode.GetBytes(FieldValue + "\r\n"));
        }
        public void AddFile(String FieldName, String FileName, byte[] FileContent, String ContentType)
        {
            String newFileField = fileField;
            String newFileContentType = fileContentType;
            newFileField = string.Format(newFileField, FieldName, FileName);
            newFileContentType = string.Format(newFileContentType, ContentType);
            //formData.AddRange(encode.GetBytes("--" + Boundary + "\r\n"));
            //formData.AddRange(encode.GetBytes(newFileField + "\r\n"));
            //formData.AddRange(encode.GetBytes(newFileContentType + "\r\n\r\n"));
            formData.AddRange(FileContent);
            //formData.AddRange(encode.GetBytes("\r\n"));
        }
        public void AddStreamFile(String FieldName, String FileName, byte[] FileContent)
        {
            AddFile(FieldName, FileName, FileContent, "application/octet-stream");
        }
        public void PrepareFormData()
        {
            formData.AddRange(encode.GetBytes("--" + Boundary + "--"));
        }
        public List<byte> GetFormData()
        {
            return formData;
        }
    }
}
