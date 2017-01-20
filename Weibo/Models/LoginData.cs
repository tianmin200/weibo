using System.Collections.Generic;
using System.Collections.Specialized;

namespace Weibo.Models
{
    public struct LoginData
    {
        const string entry = "weibo";
        const string gateway = "1";
        const string from = "";
        const string savestate = "7";
        const string userticket = "1";
        const string vsnf = "1";
        const string service = "miniblog";
        const string encoding = "UTF-8";
        const string pwencode = "rsa2";
        const string sr = "1280*800";
        const string prelt = "529";
        const string url = "http://weibo.com/ajaxlogin.php?framelogin=1&callback=parent.sinaSSOController.feedBackUrlCallBack";
        const string returntype = "TEXT";

        public static NameValueCollection Create(PreLoginResponseData data, string username, string password, string code)
        {
            NameValueCollection items = new NameValueCollection();
            items.Add("entry", entry);
            items.Add("gateway", gateway);
            items.Add("from", from);
            items.Add("savestate", savestate);
            items.Add("userticket", userticket);
            items.Add("vsnf", vsnf);
            items.Add("service", service);
            items.Add("encoding", encoding);
            items.Add("pwencode", pwencode);
            items.Add("sr", sr);
            items.Add("prelt", prelt);
            items.Add("url", url);
            items.Add("rsakv", data.rsakv);
            items.Add("servertime", data.servertime.ToString());
            items.Add("nonce", data.nonce);
            items.Add("su", username);
            items.Add("sp", password);
            items.Add("returntype", returntype);
            items.Add("pcid", data.pcid);
            items.Add("door", code);
            return items;
        }
    }

    public class LoginResponseData
    {
        public int retcode { get; set; }
        public string ticket { get; set; }
        public string reason { get; set; }
        public string uid { get; set; }
        public string nick { get; set; }
        public string cookies { get; set; }
        public List<string> crossDomainUrlList { get; set; }
    }
}
