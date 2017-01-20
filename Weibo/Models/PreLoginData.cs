using System;
using System.Collections.Specialized;
using Weibo.Core;


namespace Weibo.Models
{
    public struct PreLoginData
    {
        const string entry = "weibo";
        const string callback = "sinaSSOController.preloginCallBack";
        const string rsakt = "mod";
        const string checkpin = "1";
        const string client = "ssologin.js(v1.4.18)";

        public static NameValueCollection Create(string username)
        {
            NameValueCollection data = new NameValueCollection();
            data.Add("entry", entry);
            data.Add("callback", callback);
            data.Add("rsakt", rsakt);
            data.Add("checkpin", checkpin);
            data.Add("client", client);
            data.Add("su", username);
            data.Add("_", DateTime.Now.TimeStamp().ToString());
            return data;
        }
    }

    public class PreLoginResponseData
    {
        public int retcode { get; set; }
        public long servertime { get; set; }
        public string pcid { get; set; }
        public string nonce { get; set; }
        public string pubkey { get; set; }
        public string rsakv { get; set; }
        public int exectime { get; set; }
        public int showpin { get; set; }
        public string cookies { get; set; }
    }
}
