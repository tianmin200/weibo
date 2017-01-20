using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Weibo.Core
{
    public class PostHelper
    {
        private string m_url = string.Empty;
        private string postString = string.Empty;
        private NameValueCollection m_values = new NameValueCollection();
        private PostTypeEnum m_type = PostTypeEnum.Get;
        private NameValueCollection header_values = new NameValueCollection();
        private bool _https = false;
        private string m_content_type = "application/x-www-form-urlencoded";
        private string m_useragent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:41.0) Gecko/20100101 Firefox/41.0";
        private string m_host = "";
        private CookieContainer cc;
        private WebProxy m_proxy;
        private string m_referer;

        #region constructor

        public PostHelper() { }

        public PostHelper(string url) : this() { m_url = url; }

        public PostHelper(string url, NameValueCollection values) : this(url) { m_values = values; }

        public PostHelper(string url, NameValueCollection values, NameValueCollection headerValues) : this(url, values) { header_values = headerValues; }

        public PostHelper(string url, NameValueCollection values, NameValueCollection headerValues, CookieContainer cookieContainer) : this(url, values, headerValues) { cc = cookieContainer; }

        public PostHelper(string url, string value) : this(url) { postString = value; }

        #endregion

        #region properties

        public string Url { get { return m_url; } set { m_url = value; } }

        public bool Https { get { return _https; } set { _https = value; } }

        public NameValueCollection PostItems { get { return m_values; } set { m_values = value; } }

        public NameValueCollection HeaderItems { get { return header_values; } set { header_values = value; } }

        public PostTypeEnum Type { get { return m_type; } set { m_type = value; } }

        public string ContentType { get { return m_content_type; } set { m_content_type = value; } }

        public string UserAgent { get { return m_useragent; } set { m_useragent = value; } }

        public string Host { get { return m_host; } set { m_host = value; } }

        public CookieContainer Cookies { get { return cc; } set { cc = value; } }

        public WebProxy Proxy { get { return m_proxy; } set { m_proxy = value; } }

        public string Referer { get { return m_referer; } set { m_referer = value; } }

        #endregion

        public string Post()
        {
            StringBuilder parameters = new StringBuilder();
            for (int i = 0; i < m_values.Count; i++)
            {
                EncodeAndAddItem(ref parameters, m_values.GetKey(i), m_values[i]);
            }
            string result = PostData(m_url, parameters.ToString());
            return result;
        }

        public async Task<string> PostAsync()
        {
            StringBuilder parameters = new StringBuilder();
            for (int i = 0; i < m_values.Count; i++)
            {
                EncodeAndAddItem(ref parameters, m_values.GetKey(i), m_values[i]);
            }
            return await PostDataAsync(m_url, parameters.ToString());
        }

        public string Post(string url)
        {
            m_url = url;
            return Post();
        }

        public async Task<string> PostAsync(string url)
        {
            m_url = url;
            return await PostAsync();
        }

        public string Post(string url, NameValueCollection values)
        {
            m_url = url;
            m_values = values;
            return Post(url);
        }

        public async Task<string> PostAsync(string url, NameValueCollection values)
        {
            m_values = values;
            return await PostAsync(url);
        }

        public string Post(string url, string data)
        {
            postString = data;
            return PostData(url, data);
        }

        public async Task<string> PostAsync(string url, string data)
        {
            postString = data;
            return await PostDataAsync(url, data);
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }

        public string PostData(string url, string data)
        {
            HttpWebRequest request = null;
            if (m_type == PostTypeEnum.Post || m_type == PostTypeEnum.Put)
            {
                Uri uri = new Uri(url);
                if (_https)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version11;
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(uri);
                }
                switch (m_type)
                {
                    case PostTypeEnum.Post:
                        request.Method = "POST";
                        break;
                    case PostTypeEnum.Put:
                        request.Method = "PUT";
                        break;
                }
                request.ContentType = m_content_type;
                request.ContentLength = data.Length;
                if (cc != null && cc.Count > 0)
                {
                    request.Headers.Add("Cookie", GetAllCookies(Cookies));
                    request.CookieContainer = cc;
                }
                else
                {
                    request.CookieContainer = new CookieContainer();
                    cc = request.CookieContainer;
                }
                if (!string.IsNullOrEmpty(m_referer))
                {
                    request.Referer = m_referer;
                }
                if (header_values != null)
                {
                    request.Headers.Add(header_values);
                }
                if (m_proxy != null)
                {
                    request.Proxy = m_proxy;
                }
                request.UserAgent = m_useragent;
                if (!string.IsNullOrEmpty(m_host))
                {
                    request.Host = m_host;
                }
                using (var stream = request.GetRequestStream())
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] bytes = encoding.GetBytes(data);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {
                Uri uri = new Uri(url + (string.IsNullOrEmpty(data) ? string.Empty : "?" + data));
                if (_https)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version11;
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(uri);
                }
                request.Method = "GET";
                if (cc != null && cc.Count > 0)
                {
                    request.CookieContainer = cc;
                }
                else
                {
                    request.CookieContainer = new CookieContainer();
                    cc = request.CookieContainer;
                }
                if (!string.IsNullOrEmpty(m_referer))
                {
                    request.Referer = m_referer;
                }
                if (header_values != null)
                {
                    request.Headers.Add(header_values);
                }
                if (m_proxy != null)
                {
                    request.Proxy = m_proxy;
                }
                request.UserAgent = m_useragent;
                if (!string.IsNullOrEmpty(m_host))
                {
                    request.Host = m_host;
                }
            }
            string result = string.Empty;
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            { }
            return result;
        }

        public async Task<string> PostDataAsync(string url, string data)
        {
            HttpWebRequest request = null;
            if (m_type == PostTypeEnum.Post || m_type == PostTypeEnum.Put)
            {
                Uri uri = new Uri(url);
                if (_https)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version11;
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(uri);
                }
                switch (m_type)
                {
                    case PostTypeEnum.Post:
                        request.Method = "POST";
                        break;
                    case PostTypeEnum.Put:
                        request.Method = "PUT";
                        break;
                }
                request.ContentType = m_content_type;
                request.ContentLength = data.Length;
                if (cc != null && cc.Count > 0)
                {
                    request.Headers.Add("Cookie", GetAllCookies(Cookies));
                    request.CookieContainer = cc;
                }
                else
                {
                    request.CookieContainer = new CookieContainer();
                    cc = request.CookieContainer;
                }
                if (!string.IsNullOrEmpty(m_referer))
                {
                    request.Referer = m_referer;
                }
                if (header_values != null)
                {
                    request.Headers.Add(header_values);
                }
                if (m_proxy != null)
                {
                    request.Proxy = m_proxy;
                }
                request.UserAgent = m_useragent;
                if (!string.IsNullOrEmpty(m_host))
                {
                    request.Host = m_host;
                }
                using (var stream = await request.GetRequestStreamAsync())
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] bytes = encoding.GetBytes(data);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {
                Uri uri = new Uri(url + (string.IsNullOrEmpty(data) ? string.Empty : "?" + data));
                if (_https)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version11;
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(uri);
                }
                request.Method = "GET";
                if (cc != null && cc.Count > 0)
                {
                    request.CookieContainer = cc;
                }
                else
                {
                    request.CookieContainer = new CookieContainer();
                    cc = request.CookieContainer;
                }
                if (!string.IsNullOrEmpty(m_referer))
                {
                    request.Referer = m_referer;
                }
                if (header_values != null)
                {
                    request.Headers.Add(header_values);
                }
                if (m_proxy != null)
                {
                    request.Proxy = m_proxy;
                }
                request.UserAgent = m_useragent;
                if (!string.IsNullOrEmpty(m_host))
                {
                    request.Host = m_host;
                }
            }
            string result = string.Empty;
            try
            {
                using (var response = await request.GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            { }
            return result;
        }

        public string PostData(string url, byte[] data)
        {
            HttpWebRequest request = null;
            Uri uri = new Uri(url);
            if (_https)
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(uri);
            }
            if (cc != null && cc.Count > 0)
            {
                request.Headers.Add("Cookie", GetAllCookies(Cookies));
                request.CookieContainer = cc;
            }
            else
            {
                request.CookieContainer = new CookieContainer();
                cc = request.CookieContainer;
            }
            if (!string.IsNullOrEmpty(m_referer))
            {
                request.Referer = m_referer;
            }
            if (header_values != null)
            {
                request.Headers.Add(header_values);
            }
            if (m_proxy != null)
            {
                request.Proxy = m_proxy;
            }
            if (!string.IsNullOrEmpty(m_host))
            {
                request.Host = m_host;
            }
            if (m_type == PostTypeEnum.Post)
            {
                request.Method = "POST";
            }
            else if (m_type == PostTypeEnum.Put)
            {
                request.Method = "PUT";
            }
            request.UserAgent = m_useragent;
            request.ContentType = m_content_type;
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            string result = string.Empty;
            try
            {
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            { }
            return result;
        }

        public async Task<string> PostDataAsync(string url, byte[] data)
        {
            HttpWebRequest request = null;
            Uri uri = new Uri(url);
            if (_https)
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(uri);
            }
            if (cc != null && cc.Count > 0)
            {
                request.Headers.Add("Cookie", GetAllCookies(Cookies));
                request.CookieContainer = cc;
            }
            else
            {
                request.CookieContainer = new CookieContainer();
                cc = request.CookieContainer;
            }
            if (!string.IsNullOrEmpty(m_referer))
            {
                request.Referer = m_referer;
            }
            if (header_values != null)
            {
                request.Headers.Add(header_values);
            }
            if (m_proxy != null)
            {
                request.Proxy = m_proxy;
            }
            if (!string.IsNullOrEmpty(m_host))
            {
                request.Host = m_host;
            }
            if (m_type == PostTypeEnum.Post)
            {
                request.Method = "POST";
            }
            else if (m_type == PostTypeEnum.Put)
            {
                request.Method = "PUT";
            }
            request.UserAgent = m_useragent;
            request.ContentType = m_content_type;
            request.ContentLength = data.Length;
            using (var stream = await request.GetRequestStreamAsync())
            {
                stream.Write(data, 0, data.Length);
            }
            string result = string.Empty;
            try
            {
                using (var response = await request.GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            { }
            return result;
        }

        private void EncodeAndAddItem(ref StringBuilder baseRequest, string key, string dataItem)
        {
            if (baseRequest == null)
            {
                baseRequest = new StringBuilder();
            }
            if (baseRequest.Length != 0)
            {
                baseRequest.Append("&");
            }
            baseRequest.Append(key);
            baseRequest.Append("=");
            baseRequest.Append(HttpUtility.UrlEncode(dataItem));
        }

        public static string GetAllCookies(CookieContainer cc)
        {
            StringBuilder sb = new StringBuilder();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, cc, new object[] { });
            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                {
                    foreach (Cookie c in colCookies)
                    {
                        sb.Append(string.Format("{0}={1},", c.Name, c.Value));
                    }
                }
            }
            return sb.ToString();
        }
    }

    public enum PostTypeEnum
    {
        Get, Post, Put
    }
}
