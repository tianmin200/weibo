using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Weibo.Core
{
    public partial class WebDownload
    {
        public static bool ClientDownload(string url, string path)
        {
            try
            {
                WebClient mywebclient = new WebClient();
                mywebclient.DownloadFile(url, path);
                return true;
            }
            catch
            { }
            return false;
        }

        public async static Task<bool> ClientDownloadAsync(string url, string path)
        {
            return await Task.Run(() =>
            {
                try
                {
                    WebClient mywebclient = new WebClient();
                    mywebclient.DownloadFileTaskAsync(new Uri(url), path);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        public static byte[] ClientDownload(string url)
        {
            try
            {
                WebClient mywebclient = new WebClient();
                return mywebclient.DownloadData(url);
            }
            catch
            {
                return null;
            }
        }

        public async static Task<byte[]> ClientDownLoadAsync(string url)
        {
            return await new WebClient().DownloadDataTaskAsync(url);
        }

        public static bool HttpDownload(string url, string path)
        {
            WebResponse response = null;
            Stream reader = null;
            FileStream writer = null;
            try
            {
                WebRequest request = WebRequest.Create(url);
                response = request.GetResponse();
                reader = response.GetResponseStream();
                writer = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                byte[] buff = new byte[512];
                int c = 0; //实际读取的字节数
                while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                {
                    writer.Write(buff, 0, c);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (response != null) { response.Close(); }
                if (reader != null) { reader.Close(); reader.Dispose(); }
                if (writer != null) { writer.Close(); writer.Dispose(); }
            }
        }

        public async static Task<bool> HttpDownloadAsync(string url, string path)
        {
            WebResponse response = null;
            Stream reader = null;
            FileStream writer = null;
            try
            {
                WebRequest request = WebRequest.Create(url);
                response = await request.GetResponseAsync();
                reader = response.GetResponseStream();
                writer = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                byte[] buff = new byte[512];
                int c = 0; //实际读取的字节数
                while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                {
                    writer.Write(buff, 0, c);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (response != null) { response.Close(); }
                if (reader != null) { reader.Close(); reader.Dispose(); }
                if (writer != null) { writer.Close(); writer.Dispose(); }
            }
        }

        public static Stream HttpDownload(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                return stream;
            }
            catch
            {
                return null;
            }
        }

        public async static Task<Stream> HttpDownloadAsync(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                var response = await request.GetResponseAsync();
                var stream = response.GetResponseStream();
                return stream;
            }
            catch
            {
                return null;
            }
        }
    }
}
