using System;
using System.Collections.Generic;
using System.Text;
using MSScriptControl;
using Microsoft.CSharp;
using System.Text.RegularExpressions;
using Weibo.Common;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;
using System.Collections;

namespace QQSpace
{
    class QQSpaceHelper
    {
        /// <summary>
        /// 空间js加密
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string QQspace_password(string password, string salt, string code)
        {

            MSScriptControl.ScriptControl com = new MSScriptControl.ScriptControl();
            com.UseSafeSubset = true;
            com.Language = "JScript";
            com.AddCode(QQSpace.Properties.Resources.a);
            string Method = "getEncryption(\"{pass}\",\"{salt}\",\"{code}\")";//getEncryption("{pass}","{salt}","{code}")         
            Method = Method.Replace("{pass}", password);
            Method = Method.Replace("{salt}", salt);
            Method = Method.Replace("{code}", code);
            string re = com.Eval(Method);
            return (re);
        }

        /// <summary>
        /// 获取当前登录Cookie的Token
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static string GetToken(CookieContainer spacecc, string qqhaoma)
        {
            string url = "http://user.qzone.qq.com/" + qqhaoma;

            HttpHelper http = new HttpHelper();
            string cookie = spacecc.GetCookieHeader(new Uri("http://qzone.qq.com")).ToString();
            string resulthtml = HttpHelper1.SendDataByGET(url, ref spacecc);

            string token = "";
            HttpHelper1.GetStringInTwoKeyword(resulthtml, ref token, "g_qzonetoken = '", "';", 0);
            return token;
        }

        public static string GetUserInfo(QQHao qqhao, CookieContainer spacecc)
        {
            string url = "http://s.web2.qq.com/api/get_self_info2?t="+HttpHelper1.GetTicks();
            string refer = "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1";
            string loginurl = "http://ptlogin4.web2.qq.com/check_sig?pttype=1&uin=3213257524&service=ptqrlogin&nodirect=0&ptsigx=iVBORw0KGgoAAAANSUhEUgAAAKUAAAClAQAAAAAVUAB3AAAACXBIWXMAAAsTAAALEwEAmpwYAAABnUlEQVRIib2XQY6EMAwEjTjkyBP4CXwMiZHmY/ATnsAxBzS93U5W2t17L4eZoXJI3LHbngg+cww3v9Z7v8rBHyVctADPXBctLdMz4wRgpEdsM47p4d7T+8J5x2ymjFjBkpb/oAs/6oA/ZzBQ6Vtwb3P5TK9fqhto5g7j3PrHj4wyUOjhReJixC8dSe8uqmCzJBBK1xoxpg4eCgiseZHABYocsFEmqdKGNZ+nKafWfZTbgzU/ZsRRVx3ER7m9VM1SHIEjRiddb21PVYtshiLT1Hx0kKpH7MpQmprC9tHMHUUczWH06qOMeOtLSzrM3fW10KxxSSuH2XkGFb6NshCyBT3tV8169NGDVlY+sV+skNccrS5ctIV4ZltXulLf9DMP5fYjejPnelXssFH07sOOLvcsPXdcVJbS9ZXDIG/TRfMOWRc5ifE2o/uZh3732LQyVki2BxttM20wYiq9o+vrojkbQe5J46TSaJnqopr7WroOXO/6Wmld1foyd5S4btpv832l3Eba/4uwD2kI43qb+zy05Y46qy7yygmwuOgXSeSL4v+A80wAAAAASUVORK5CYII=&s_url=http%3A%2F%2Fw.qq.com%2Fproxy.html%3Flogin2qq%3D1%26webqq_type%3D10&f_url=&ptlang=2052&ptredirect=100&aid=501004106&daid=164&j_later=0&low_login_hour=0&regmaster=0&pt_login_type=3&pt_aid=0&pt_aaid=16&pt_light=0&pt_3rd_aid=0";
            CookieContainer newcc = new CookieContainer();
            string resultstr = HttpHelper1.SendDataByGET(loginurl,ref newcc);

            string result = HttpHelper1.SendDataByGET(url,refer, ref newcc);
            return result;
        }
        internal static void SendShuoshuoWithPic(string content, ArrayList imgs, CookieContainer spacecc, QQHao qqhao)
        {
            string richval = "";
            string pic_bo = "";
            string poststr = "";
            string refer = "http://user.qzone.qq.com/" + qqhao.Qqhaoma;
            string url = "http://h5.qzone.qq.com/proxy/domain/taotao.qzone.qq.com/cgi-bin/emotion_cgi_publish_v6?g_tk=" + qqhao.G_tk + "&qzonetoken=" + qqhao.Token;
            foreach (UploadImageData imgdata in imgs)
            {
                string imgrichval = string.Format(",{0},{1},{2},{3},{4},{5},,{6},{7}", imgdata.data.albumid, imgdata.data.lloc, imgdata.data.sloc, imgdata.data.type, imgdata.data.height, imgdata.data.width, imgdata.data.height, imgdata.data.width);
                richval += imgrichval + "%09";
                string imgpicbo = "";
                HttpHelper1.GetStringInTwoKeyword(imgdata.data.pre, ref imgpicbo, "bo=", "!", 0);
                pic_bo += imgpicbo + "!,";
            }
            richval = richval.Substring(0, richval.Length - 3);//去除最后一个%09
            pic_bo = pic_bo.Substring(0, pic_bo.Length - 1);//去除最后一个逗号,
            pic_bo = pic_bo + "%09" + pic_bo;
            string encodecontent = HttpUtility.UrlEncode(content);
            string encoderichval = richval.Replace("!", "%21").Replace(",", "%2c");
            string encodepicbo = pic_bo.Replace("!", "%21").Replace(",", "%2c");
            poststr = "qzreferrer=http%3A%2F%2Fuser.qzone.qq.com%2F3213257524&syn_tweet_verson=1&paramstr=1&pic_template=&richtype=1&richval=" + encoderichval + "&special_url=&subrichtype=1&pic_bo=" + encodepicbo + "&who=1&con=" + encodecontent + "&feedversion=1&ver=1&ugc_right=1&to_sign=0&hostuin=" + qqhao.Qqhaoma + "&code_version=1&format=fs";
            string resulthtml = HttpHelper1.SendDataByPost(url, poststr, refer, ref spacecc);
        }

        /// <summary>
        /// 获取当前登录Cookie的g_tk值
        /// </summary>
        /// <param name="cookiestr"></param>
        /// <returns></returns>
        public static string Getgtk(string cookiestr)
        {

            //Set-Cookie: skey=@HR3etVm80; PATH=/; DOMAIN=qq.com;
            string p_sKey = cookiestr.Substring(cookiestr.IndexOf("p_skey=") + 7, 44);//提取skey-后面的10个字符用于算出g_tk值


            long hash = 5381;
            for (int o = 0; o < p_sKey.Length; o++)
            {
                hash += (hash << 5) + p_sKey[o];
            }
            hash = hash & 0x7fffffff;//hash就是算出的g_tk值了.




            return hash.ToString(); ;
        }
        /// <summary>
        /// 取出中间文本
        /// </summary>
        /// <param name="str">全文本</param>
        /// <param name="leftstr">文本左边</param>
        /// <param name="rightstr">文本右边</param>
        /// <returns></returns>
        public static string GetStringMid(string str, string leftstr, string rightstr)
        {
            int i = str.IndexOf(leftstr) + leftstr.Length;
            string temp = str.Substring(i, str.IndexOf(rightstr, i) - i);
            return temp;
        }

        /// <summary>
        /// 取文本中间
        /// </summary>
        /// <param name="strYuan">全文本</param>
        /// <param name="strQian">文本左边</param>
        /// <param name="strHou">文本右边</param>
        /// <returns></returns>
        public static string GetStringMiddle(string strYuan, string strQian, string strHou)
        {

            int int1 = strYuan.IndexOf(strQian);
            int int2 = strYuan.IndexOf(strHou);
            //获得中间代码的长度
            int lengmid = strYuan.Substring(int1).Length - strYuan.Substring(int2).Length - strQian.Length;
            //获得中间代码的index
            int intmid = int1 + strQian.Length;
            //
            string strM = strYuan.Substring(intmid, lengmid);
            return strM;
        }
        /// <summary>
        /// 取出文本右边
        /// </summary>
        /// <param name="str">全文本</param>
        /// <param name="s">欲寻找的文本</param>
        /// <returns></returns>
        public static string GetStringRight(string str, string s)
        {

            string temp = str.Substring(str.IndexOf(s) + 1, str.Length - str.Substring(0, str.IndexOf(s) + 1).Length);
            if (temp != str)
            {
                return temp;
            }
            else
            {
                return "-1";
            }

        }


        /// <summary>
        /// 取随机数
        /// </summary>
        /// <param name="iLength">长度</param>
        /// <returns></returns>
        public static string GetRandomString(int iLength)
        {
            string buffer = "0123456789";// 随机字符中也可以为汉字（任何）
            StringBuilder sb = new StringBuilder();
            Random r = new Random();
            int range = buffer.Length;
            for (int i = 0; i < iLength; i++)
            {
                sb.Append(buffer.Substring(r.Next(range), 1));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Cookie合并更新
        /// </summary>
        /// <param name="_cookie_old">旧Cookie</param>
        /// <param name="_cookie_New">新Cookie</param>
        /// <returns>返回新的Cookie</returns>
        internal static string GetMergeCookie(string _cookie_old, string _cookie_New)
        {
            if (string.IsNullOrWhiteSpace(_cookie_New))//新的是空的
            {
                return _cookie_old;//返回老的
            }
            if (string.IsNullOrWhiteSpace(_cookie_old))//老的是空的
            {
                return _cookie_New;//返回新的
            }
            List<string> cookielist = new List<string>();//结果
            string[] list_New = _cookie_New.ToString().Split(';');
            string[] list_old = _cookie_old.ToString().Split(';');
            foreach (string item in list_New)
            {
                //排除重复项
                if (cookielist.Contains(item)) continue;
                //对接Cookie基本的Key和Value串
                cookielist.Add(string.Format("{0} ", item));
            }
            foreach (string item in list_old)
            {
                //排除重复项
                if (cookielist.Contains(item)) continue;
                //对接Cookie基本的Key和Value串
                cookielist.Add(string.Format("{0}", item));
            }
            return string.Join("; ", cookielist);
        }

        public static string updateCookie(string oldcookie, string newcookie)
        {
            List<string> oldcookielist = new List<string>();
            if (oldcookie.Contains(";"))
                oldcookielist = new List<string>(oldcookie.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
            else
                oldcookielist.Add(oldcookie);

            List<string> newcookielist = new List<string>();
            if (newcookie.Contains(";"))
                newcookielist = new List<string>(newcookie.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
            else
                newcookielist.Add(newcookie);

            foreach (string cookie in newcookielist)
            {
                //Console.WriteLine("cookie:" + cookie);
                if (!string.IsNullOrWhiteSpace(cookie))
                {
                    if (!string.IsNullOrWhiteSpace(cookie.Split('=')[1])) //判断cookie的value值是为空,不为空时才进行操作
                    {
                        bool isFind = false; //判断是否是新值
                        for (int i = 0; i < oldcookielist.Count; i++)
                        {
                            if (cookie.Split('=')[0] == oldcookielist[i].Split('=')[0])
                            {
                                //oldcookielist[i].Split('=')[1] = cookie.Split('=')[1];
                                oldcookielist[i] = cookie;
                                isFind = true;
                                break;
                            }
                        }

                        if (!isFind) //如果计算后还是false,则表示newcookie里出现新值了,将新值添加到老cookie里
                            oldcookielist.Add(cookie);
                    }
                }
            }

            oldcookie = string.Empty;

            for (int i = 0; i < oldcookielist.Count; i++)
                oldcookie += oldcookielist[i] + ";";

            return oldcookie;

        }
        public static CookieContainer Login(string qqhaoma, string password)
        {
            string cookie = "";
            string salt = "";
            bool isVer = false;
            string pt_verifysession_v1 = "";
            string sig = "";
            CookieContainer spacecc = new CookieContainer();
            string URL = "http://xui.ptlogin2.qq.com/cgi-bin/xlogin?proxy_url=http%3A//qzs.qq.com/qzone/v6/portal/proxy.html&daid=5&&hide_title_bar=1&low_login=0&qlogin_auto_login=1&no_verifyimg=1&link_target=blank&appid=549000912&style=22&target=self&s_url=http%3A%2F%2Fqzs.qq.com%2Fqzone%2Fv5%2Floginsucc.html%3Fpara%3Dizone&pt_qr_app=手机QQ空间&pt_qr_link=http%3A//z.qzone.com/download.html&self_regurl=http%3A//qzs.qq.com/qzone/v6/reg/index.html&pt_qr_help_link=http%3A//z.qzone.com/download.html";//URL     必需项    
            string resulthtml = HttpHelper1.SendDataByGET(URL, ref spacecc);


            string Login_sig = QQSpaceHelper.GetStringMid(resulthtml, "pt_login_sig=", ";");
            string code = GetVerification_Code(qqhaoma, cookie, ref salt, ref isVer, ref pt_verifysession_v1, ref sig);
            spacecc = LoginQQspace(qqhaoma, password, code, salt, isVer, pt_verifysession_v1, Login_sig, sig);

            return spacecc;
        }


        /// <summary>
        /// QQ空间登陆
        /// </summary>
        /// <param name="u">账号</param>
        /// <param name="p">密码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        private static CookieContainer LoginQQspace(string u, string p, string code, string salt, bool isver, string pt_verifysession_v1, string Login_sig, string sig)
        {
            string html;
            string password;
            string v;
            string newcode;

            string Loginstatus;//登陆状态
            if (!isver)
            {
                password = QQSpaceHelper.QQspace_password(p, salt, code);
                CookieContainer spacecc = new CookieContainer();
                string url = "http://ptlogin2.qq.com/login?u=" + u + "&verifycode=" + code + "&pt_vcode_v1=0&pt_verifysession_v1=" + pt_verifysession_v1 + "&p=" + password + "&pt_randsalt=0&u1=http%3A%2F%2Fqzs.qq.com%2Fqzone%2Fv5%2Floginsucc.html%3Fpara%3Dizone&ptredirect=0&h=1&t=1&g=1&from_ui=1&ptlang=2052&action=2-6-1466753336279&js_ver=10165&js_type=1&login_sig=" + Login_sig + "&pt_uistyle=40&aid=549000912&daid=5&";//URL     必需项    
                string resulthtml = HttpHelper1.SendDataByGET(url, ref spacecc);

                string cookieAddres = QQSpaceHelper.GetStringMid(resulthtml, "ptuiCB('0','0','", "','");



                HttpHelper1.SendDataByGET(cookieAddres, ref spacecc);
                return spacecc;
                //HttpHelper http_A = new HttpHelper();
                //HttpItem item_A = new HttpItem()
                //{
                //    URL = cookieAddres,//URL     必需项    
                //    Method = "get",//URL     可选项 默认为Get   
                //    IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                //    Cookie = Cookie_A,//字符串Cookie     可选项   
                //    Referer = "",//来源URL     可选项   
                //    Postdata = "",//Post数据     可选项GET时不需要写   
                //    Timeout = 100000,//连接超时时间     可选项默认为100000    
                //    ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                //    UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                //    ContentType = "text/html",//返回类型    可选项有默认值   
                //    Allowautoredirect = false,//是否根据301跳转     可选项   
                //                              //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                //                              //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                //    ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
                //                 //ProxyPwd = "123456",//代理服务器密码     可选项    
                //                 //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                //    ResultType = ResultType.String
                //};
                //HttpResult result_A = http_A.GetHtml(item_A);
                //string spaceCookie = result_A.Cookie;
                //return spaceCookie;

                //HttpHelper http_B = new HttpHelper();
                //HttpItem item_B = new HttpItem()
                //{
                //    URL = "http://user.qzone.qq.com/3213257524",//URL     必需项    
                //    Method = "get",//URL     可选项 默认为Get   
                //    IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                //    Cookie = Cookie_A,//字符串Cookie     可选项   
                //    Referer = "",//来源URL     可选项   
                //    Postdata = "",//Post数据     可选项GET时不需要写   
                //    Timeout = 100000,//连接超时时间     可选项默认为100000    
                //    ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                //    UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                //    ContentType = "text/html",//返回类型    可选项有默认值   
                //    Allowautoredirect = true,//是否根据301跳转     可选项   
                //                              //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                //                              //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                //    ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
                //                 //ProxyPwd = "123456",//代理服务器密码     可选项    
                //                 //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                //    ResultType = ResultType.String
                //};
                //HttpResult result_B = http_B.GetHtml(item_B);

                //Cookie = Cookie.Replace(",", "");
                //return Cookie;
                //File.WriteAllText("qqcookie.txt", spaceCookie);

            }
            else
            {

                CookieContainer spacecc = new CookieContainer();
                string URL = "http://captcha.qq.com/cap_union_verify_new?clientype=2&uin=" + u + "&aid=549000912&cap_cd=" + pt_verifysession_v1 + "&pt_style=40&0.018421005630953724&rand=0.3566402584289032&capclass=0&sig=" + sig + "&collect=QJ8EZyxDDp69G3zb59eR-Mnz-5F08BdC-P9fzPDRe-BaZ5GqAriI5D2LH0DHscZ4JCS-yYz5_HPyVoopkm2gwe9Mgziaxhrp9doZoY9CFl7k2rbzaiegdx-eTyE3IVQkNwRTACxDHBRg3mHxM0-gf_MAAs4H7ieIfH1vgxORw0P-XrwPsqwKLhQiJFVa6KM4Gt8jF1BKJe6_WbSdP495lIa1FddYoi8yBYIdigtYuCC8zBFB98WfItynhsdFoMN9U9utX9eXltsFSBAqfiDPwChTe0zVG7cIKo38wAvQjE6ejZlqekFBa2Cl_mTDzg6QrGv_At2frSgH1ZdQIHO5ADsCa1u_Y552KotzqcDX8kLdzQxDXG4E-wteroUKZ5WG-zkQ89xCT9aWegigU4gvHrFHG9xpxTAnVA_mxaewMsUxqFOhRafPCHzyIw2eaTQl7D7AopK4WdMU3EJ8gLoRdDEAS7EXYIsqy0GWUl3m33QuJw1ocsnp42WG_V2pAbN-8PIqEoS-Z86Hj2kKQ-fdVMMVGj0BOo5FfNQaUHh6CvnPqwwiVGlUGKy9Ih_WC7G8312sN55MKWbJOONgZhrJxZbf42xks4YrlXDyvfMNe_TKzkW_jyHKJQ43PLmtvCqkQ_vslP2BCZwAEVAc9Xwd2K-y694nxK6EhVtclCC_m7fkbQWv19_uZoxIEczOqlvv-0icf_xQlzX9nzgYmgbLGsTLzW7H5YBYe7Sd-Udi2i_zW3p05ZL5Sr_IQpJpSRkckTLDi-4L5DX0aGVq_xblhotlZ4J7EpoyekA1uyMTkz7OIyVa8bAlFO7WOUwOJ5m8Kb65yv9Vsrb5bx8w-3CWvfBV9Nz4RX0QL_v-ISGHcwfN3cgJ6nUXZ6xXHpMYVZZBA15U-t4nVHGg0dagA7HAPcWi9Am-ScFah4o7xUMpJ5pTFfbizCHKVQZ42BMT56mC&ans=" + code;//URL     必需项    
                string resulthtml = HttpHelper1.SendDataByGET(URL, ref spacecc);




                v = QQSpaceHelper.GetStringMid(resulthtml, "\"ticket\" : \"", "\" , \"");
                newcode = QQSpaceHelper.GetStringMid(resulthtml, "\"randstr\" : \"", "\" , \"");
                password = QQSpaceHelper.QQspace_password(p, salt, newcode);
                URL = "http://ptlogin2.qq.com/login?u=" + u + "&verifycode=" + newcode + "&pt_vcode_v1=1&pt_verifysession_v1=" + v + "&p=" + password + "&pt_randsalt=0&u1=http%3A%2F%2Fqzs.qq.com%2Fqzone%2Fv5%2Floginsucc.html%3Fpara%3Dizone&ptredirect=0&h=1&t=1&g=1&from_ui=1&ptlang=2052&action=2-16-1466756588355&js_ver=10165&js_type=1&login_sig=ydH8cF-2O0R-QwimXsoVitiSBDo3WLir-zDAh5zEhsu7xGH4pDuGXleaieU8ySM0&pt_uistyle=40&aid=549000912&daid=5&";//URL     必需项    
                resulthtml = HttpHelper1.SendDataByGET(URL, ref spacecc);


                string cookieAddres = QQSpaceHelper.GetStringMid(resulthtml, "ptuiCB('0','0','", "','");

                HttpHelper1.SendDataByGET(cookieAddres, ref spacecc);
                return spacecc;

            }
            //判断登录是否成功
            //Loginstatus = QQSpaceHelper.GetStringMiddle(html, "ptuiCB('", "','0','");

            //if (Loginstatus == "0")
            //{
            //    Console.Write(QQSpaceHelper.GetStringMiddle(html, "aid=0','0','", "', '") + "\r\n" + QQSpaceHelper.GetStringMid(html, QQSpaceHelper.GetStringMiddle(html, "aid=0','0','", "', '") + "', '", "')"));


            //}
            //if (Loginstatus == "7")
            //{
            //    Console.Write("验证码错误！");
            //}
            //if (Loginstatus == "3")
            //{
            //    Console.Write("账号或密码错误!", "登陆失败");
            //}

        }
        /// <summary>
        /// 发布文字说说
        /// </summary>
        /// <param name="content"></param>
        /// <param name="spacecc"></param>
        /// <param name="qqhao"></param>
        /// <returns></returns>
        public static string SendShuoshuo(string content, CookieContainer spacecc, QQHao qqhao)
        {
            string url = "http://h5.qzone.qq.com/proxy/domain/taotao.qzone.qq.com/cgi-bin/emotion_cgi_publish_v6?g_tk=" + qqhao.G_tk + "&qzonetoken=" + qqhao.Token;

            string poststr = "qzreferrer=http%3A%2F%2Fuser.qzone.qq.com%2F" + qqhao.Qqhaoma + "%2F&syn_tweet_verson=1&paramstr=1&pic_template=&richtype=&richval=&special_url=&subrichtype=&who=1&con=" + HttpUtility.UrlEncode(content) + "&feedversion=1&ver=1&ugc_right=1&to_sign=0&hostuin=" + qqhao.Qqhaoma + "&code_version=1&format=fs";
            string refer = "http://qzone.qq.com/" + qqhao.Qqhaoma;
            string resulthtml = HttpHelper1.SendDataByPost(url, poststr, refer, ref spacecc);
            return "";
        }
        public static UploadImageData UploadPic(string filepath, CookieContainer spacecc, QQHao qqhao)
        {
            string picpath = filepath;
            CookieCollection ccl = spacecc.GetCookies(new Uri("http://qzone.qq.com"));
            string url = string.Format("http://shup.photo.qzone.qq.com/cgi-bin/upload/cgi_upload_image?g_tk={0}", qqhao.G_tk);
            FileStream file = new FileStream(picpath, FileMode.Open);
            byte[] bb = new byte[file.Length];
            file.Read(bb, 0, (int)file.Length);
            file.Close();
            Image img = byteArrayToImage(bb);
            string skey = ccl[6].Value;
            string p_skey = ccl[1].Value;

            MsMultiPartFormData form = new MsMultiPartFormData();
            form.AddFormField("Filename", Path.GetFileName(filepath));
            form.AddFormField("qzonetoken", qqhao.Token);
            form.AddFormField("photoData", "filename");
            form.AddFormField("exif_info", "extendXml:");
            form.AddFormField("backUrls", "http://upbak.photo.qzone.qq.com/cgi-bin/upload/cgi_upload_image,http://119.147.64.75/cgi-bin/upload/cgi_upload_image");
            form.AddFormField("skey", skey);
            form.AddFormField("refer", "shuoshuo");
            form.AddFormField("output_charset", "utf-8");
            form.AddFormField("zzpaneluin", qqhao.Qqhaoma);
            form.AddFormField("uploadtype", "1");
            form.AddFormField("exttype", "0");
            form.AddFormField("hd_width", "2048");
            form.AddFormField("albumtype", "7");
            form.AddFormField("hd_height", "10000");
            form.AddFormField("zzpanelkey", "");
            form.AddFormField("charset", "utf-8");
            form.AddFormField("filename", "filename");
            form.AddFormField("p_uin", qqhao.Qqhaoma);
            form.AddFormField("hd_quality", "96");
            form.AddFormField("output_type", "json");
            form.AddFormField("uin", qqhao.Qqhaoma);
            form.AddFormField("p_skey", p_skey);
            form.AddFormField("upload_hd", "1");
            form.AddFormField("Filename", "xml");
            form.AddFormField("output_type", "xml");
            form.AddStreamFile("filename", Path.GetFileName(picpath), bb);
            form.AddFormField("Upload", "Submit Query");
            form.PrepareFormData();

            form.GetFormData();
            string host = "shup.photo.qzone.qq.com";
            //url = string.Format("http://upload.t.qq.com/asyn/uploadpicCommon.php?call=2&uin={0}&g_tk={1}&rand={2}&_ps1={3}&_ps2=null", uin, weiboGtk, rand.NextDouble(), _ps1);
            string ContentType = "multipart/form-data; boundary=" + form.Boundary;

            HttpHelper help = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = url,
                CookieCollection = ccl,
                ResultCookieType = ResultCookieType.CookieCollection,
                Host = host,
                Accept = "*/*",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36",
                ContentType = ContentType,
                Method = "POST",
                PostDataType = PostDataType.Byte,
                PostdataByte = form.GetFormData().ToArray(),
                Encoding = Encoding.UTF8,
                ProxyIp = "ieproxy"
            };
            item.Header.Add("Origin", "http://qzs.qq.com");

            HttpResult result = help.GetHtml(item);
            string resulthtml = result.Html;//上传图片，测试成功
            string jsonstr = resulthtml.Replace("_Callback(", "").Replace(");", "");
            UploadImageData imagedata2 = Newtonsoft.Json.JsonConvert.DeserializeObject<UploadImageData>(jsonstr);
            return imagedata2;
        }
        public static string GetJCookie(string cookie)
        {
            string skey = QQSpaceHelper.GetStringMid(cookie, "skey=", "';");
            string p_skey = "";
            string uin = "";
            string p_uin = "";
            string pt4_token = "";
            string pt2gguin = "";
            HttpHelper1.GetStringInTwoKeyword(cookie, ref skey, "skey=@", ";", 0);
            HttpHelper1.GetStringInTwoKeyword(cookie, ref p_skey, "p_skey=", ";", 0);
            HttpHelper1.GetStringInTwoKeyword(cookie, ref uin, "uin=", ";", 0);
            HttpHelper1.GetStringInTwoKeyword(cookie, ref p_uin, "p_uin=", ";", 0);
            HttpHelper1.GetStringInTwoKeyword(cookie, ref pt4_token, "pt4_token=", ";", 0);
            HttpHelper1.GetStringInTwoKeyword(cookie, ref pt2gguin, "pt2gguin=", ";", 0);
            string jcookie = "uin=" + uin + "; skey=" + skey + ";pt4_token=" + pt4_token + "; p_uin=" + p_uin + "; p_skey=" + p_skey + "; ";
            return jcookie;
        }
        /// <summary>
        /// 取验证码
        /// </summary>
        private static string GetVerification_Code(string qqhaoma, string cookie, ref string salt, ref bool isver, ref string pt_verifysession_v1, ref string sig)
        {
            // AllocConsole();
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = "http://check.ptlogin2.qq.com/check?regmaster=&pt_tea=2&pt_vcode=1&uin=" + qqhaoma + "&appid=549000912&js_ver=10165&js_type=1&login_sig=-TLaeVf9AOagLEQm323nKPYN*FyHmJmDAWjXxdV9ej7KL0uC3bbCSsvM9uPuRyyV&u1=http%3A%2F%2Fqzs.qq.com%2Fqzone%2Fv5%2Floginsucc.html%3Fpara%3Dizone&r=0.3361908566720933&pt_uistyle=40",//URL     必需项    
                Method = "get",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = cookie,//字符串Cookie     可选项   
                Referer = "",//来源URL     可选项   
                Postdata = "",//Post数据     可选项GET时不需要写   
                Timeout = 100000,//连接超时时间     可选项默认为100000
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "text/html",//返回类型    可选项有默认值   
                Allowautoredirect = false,//是否根据301跳转     可选项   
                //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
                //ProxyPwd = "123456",//代理服务器密码     可选项    
                //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                ResultType = ResultType.String
            };
            HttpResult result = http.GetHtml(item);
            string html = result.Html;
            string Cookie_A = result.Cookie;
            string code = QQSpaceHelper.GetStringMid(html, "'0','", "','");
            salt = "\\" + QQSpaceHelper.GetStringMid(html, "','\\", "','");

            pt_verifysession_v1 = QQSpaceHelper.GetStringRight(html, code);
            pt_verifysession_v1 = QQSpaceHelper.GetStringRight(pt_verifysession_v1, "\\");
            pt_verifysession_v1 = QQSpaceHelper.GetStringMid(pt_verifysession_v1, "','", "','");
            string Verification_Code_A = code;

            #region 判断文本或图片验证码
            if (code != "_checkVC('1")
            {
                isver = false;
                //textBox2.Text = code;
                return code;
                //textBox2.ReadOnly = true;
            }
            else
            {
                isver = true;
                Image img = QQSpaceHelper.GetVcodeImg(html, qqhaoma, cookie, ref sig);
                string vcode = Weibo.Common.Dama2.GetVcode(img);
                return vcode;
            }
            #endregion
            //System.Console.WriteLine("pt_verifysession_v1:" + pt_verifysession_v1 + "sig" + sig);
        }

        public static Image GetVcodeImg(string resulthtml, string qqhaoma, string cookie, ref string sig)
        {
            string pt_verifysession_v1 = QQSpaceHelper.GetStringMid(resulthtml, "'1','", "','");

            HttpHelper http_A = new HttpHelper();
            HttpItem item_A = new HttpItem()
            {
                URL = "http://captcha.qq.com/cap_union_show?clientype=2&uin=" + qqhaoma + "&aid=549000912&cap_cd=" + pt_verifysession_v1 + "&pt_style=40&0.4430906427668041",
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = "",//字符串Cookie     可选项   
                Referer = "",//来源URL     可选项   
                Postdata = "",//Post数据     可选项GET时不需要写   
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "text/html",//返回类型    可选项有默认值   
                Allowautoredirect = false,//是否根据301跳转     可选项   
                                          //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                                          //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
                             //ProxyPwd = "123456",//代理服务器密码     可选项    
                             //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                ResultType = ResultType.String
            };
            HttpResult result_A = http_A.GetHtml(item_A);
            sig = result_A.Html;
            sig = QQSpaceHelper.GetStringMid(sig, "var g_vsig = \"", "\";");


            //取出验证码地址
            HttpHelper http_code = new HttpHelper();
            HttpItem item_code = new HttpItem()
            {
                URL = "http://captcha.qq.com/getimgbysig?clientype=2&uin=" + qqhaoma + "&aid=549000912&cap_cd=" + pt_verifysession_v1 + "&pt_style=40&0.018421005630953724&rand=0.24943880787089034&sig=" + sig,
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = cookie,//字符串Cookie     可选项   
                Referer = "",//来源URL     可选项   
                Postdata = "",//Post数据     可选项GET时不需要写   
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "text/html",//返回类型    可选项有默认值   
                Allowautoredirect = false,//是否根据301跳转     可选项   
                                          //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                                          //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
                             //ProxyPwd = "123456",//代理服务器密码     可选项    
                             //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                ResultType = ResultType.Byte
            };
            HttpResult result_code = http_code.GetHtml(item_code);
            Image img = byteArrayToImage(result_code.ResultByte);
            return img;
        }
        private static Image byteArrayToImage(byte[] Bytes)
        {
            MemoryStream ms = new MemoryStream(Bytes);
            return Bitmap.FromStream(ms, true);
        }
    }
    class MsMultiPartFormData
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
            String newFieldName = fieldName;
            newFieldName = string.Format(newFieldName, FieldName);
            formData.AddRange(encode.GetBytes("--" + Boundary + "\r\n"));
            formData.AddRange(encode.GetBytes(newFieldName + "\r\n\r\n"));
            formData.AddRange(encode.GetBytes(FieldValue + "\r\n"));
        }
        public void AddFile(String FieldName, String FileName, byte[] FileContent, String ContentType)
        {
            String newFileField = fileField;
            String newFileContentType = fileContentType;
            newFileField = string.Format(newFileField, FieldName, FileName);
            newFileContentType = string.Format(newFileContentType, ContentType);
            formData.AddRange(encode.GetBytes("--" + Boundary + "\r\n"));
            formData.AddRange(encode.GetBytes(newFileField + "\r\n"));
            formData.AddRange(encode.GetBytes(newFileContentType + "\r\n\r\n"));
            formData.AddRange(FileContent);
            formData.AddRange(encode.GetBytes("\r\n"));
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