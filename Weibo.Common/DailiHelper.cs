using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Weibo.Common
{
    public class DailiHelper
    {
        public static string GetDailiIP()
        {
            string url = "http://dev.kuaidaili.com/api/getproxy/?orderid=988306037973723&num=1&area=北京&b_pcchrome=1&b_pcie=1&b_pcff=1&protocol=1&method=2&an_an=1&an_ha=1&sp1=1&sep=1";
            CookieContainer cc = new CookieContainer();
            string result = HttpHelper1.SendDataByGET(url,ref cc);
            return result;
        }

        public static WebProxy GetProxy()
        {
            string dailiip = DailiHelper.GetDailiIP();
            string proxyip = dailiip.Split(':')[0];
            int proxyport = Convert.ToInt32(dailiip.Split(':')[1]);
            WebProxy proxy = new WebProxy(proxyip, proxyport);
            return proxy;
        }

        public static bool VerIP(WebProxy proxyObject)
        {
            try
            {
                HttpWebRequest Req;
                HttpWebResponse Resp;
                //WebProxy proxyObject = new WebProxy(ip, port);// port为端口号 整数型
                Req = WebRequest.Create("https://www.baidu.com") as HttpWebRequest;
                Req.Proxy = proxyObject; //设置代理
                Req.Timeout = 5000;   //超时
                Resp = (HttpWebResponse)Req.GetResponse();
                Encoding bin = Encoding.GetEncoding("UTF-8");
                using (StreamReader sr = new StreamReader(Resp.GetResponseStream(), bin))
                {
                    string str = sr.ReadToEnd();
                    if (str.Contains("百度"))
                    {
                        Resp.Close();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
