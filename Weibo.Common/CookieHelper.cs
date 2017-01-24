using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Weibo.Common
{
    public class CookieHelper
    {

        ///
        /// 从包含多个 Cookie 的字符串读取到 CookieCollection 集合
        ///
        /// Cookie字符串。
        /// 域名（或主机名称）
        ///
        public static CookieCollection GetCookieCollectionByString(string cookieHead, string defaultDomain)
        {
            CookieCollection collection = new CookieCollection();
            if (cookieHead == null) return null;
            if (defaultDomain == null) return null;
            string[] ary = cookieHead.Split(';');
            if (ary.Length < 2) ary = cookieHead.Split(',');
            for (int i = 0; i < ary.Length; i++)
            {
                Cookie ck = GetCookieFromString(ary[i].Trim(), defaultDomain);
                if (defaultDomain.Contains("qq.com"))
                {
                    if (ck != null && ck.Value != "")
                        collection.Add(ck);
                }
                else if (ck != null)
                {
                    collection.Add(ck);
                }
            }
            return collection;
        }

        public void ConvertCookie()
        {
            Cookie cookie = new Cookie();

        }

        #region 读取某一个 Cookie 字符串到 Cookie 变量中
        ///
        /// 从字符串中获取Cookie
        ///
        /// 字符串
        /// 域名（或主机名称）
        ///
        public static Cookie GetCookieFromString(string cookieString, string defaultDomain)
        {
            if (cookieString == null || defaultDomain == null) return null;
            string[] ary = cookieString.Split(';');
            Hashtable hs = new Hashtable();
            for (int i = 0; i < ary.Length; i++)
            {
                string s = ary[i].Trim();
                int index = s.IndexOf("=", System.StringComparison.Ordinal);
                if (index > 0)
                {
                    hs.Add(s.Substring(0, index), s.Substring(index + 1));
                }
            }
            Cookie ck = new Cookie();
            foreach (object key in hs.Keys)
            {
                if (key.ToString() == "path") ck.Path = hs[key].ToString();

                else if (key.ToString() == "expires")
                {
                    //ck.Expires=DateTime.Parse(hs[Key].ToString();
                }
                else if (key.ToString() == "domain") ck.Domain = hs[key].ToString();
                else
                {
                    ck.Name = key.ToString();
                    ck.Value = hs[key].ToString();
                }
            }
            if (ck.Name == "") return null;
            if (ck.Domain == "") ck.Domain = defaultDomain;
            return ck;
        }
        #endregion
        public static List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
            System.Reflection.BindingFlags.Instance, null, cc, new object[] { });
            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;
        }
        public static void SaveCookie(string path, CookieContainer cc)
        {
            string cookie = cc.GetCookieHeader(new Uri("http://qzone.qq.com")).ToString();
            File.WriteAllText(path, cookie, System.Text.Encoding.Default);
        }

        public static CookieContainer ReadCookieFromFile(string path)
        {
            CookieContainer CookieC = new CookieContainer();
            CookieCollection ccl = new CookieCollection();
            string cookies = File.ReadAllText(path, System.Text.Encoding.Default);

            ccl = CookieHelper.GetCookieCollectionByString(cookies, "qzone.qq.com");
            CookieC.Add(ccl);
            return CookieC;
        }
    }



    /// <summary>
    /// WinInet.dll wrapper
    /// </summary>
    internal static class CookieReader
    {


        public const int INTERNET_COOKIE_HTTPONLY = 0x00002000;


        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetGetCookieEx(
            string url,
            string cookieName,
            StringBuilder cookieData,
            ref int size,
            int flags,
            IntPtr pReserved);
        public static string GetGlobalCookies(string uri)
        {
            int datasize = 1024;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (CookieReader.InternetGetCookieEx(uri, null, cookieData, ref datasize, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero)
            && cookieData.Length > 0)
            {
                return cookieData.ToString();
            }
            else
            {
                return null;
            }
        }

        public static string GetCookie(string url)
        {
            int size = 512;
            StringBuilder sb = new StringBuilder(size);
            if (!InternetGetCookieEx(url, null, sb, ref size, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero))
            {
                if (size < 0)
                {
                    return null;
                }
                sb = new StringBuilder(size);
                if (!InternetGetCookieEx(url, null, sb, ref size, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero))
                {
                    return null;
                }
            }
            return sb.ToString();
        }
    }
}
