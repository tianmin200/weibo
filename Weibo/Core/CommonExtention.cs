using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Weibo.Core
{
    /// <summary>
    /// 类型扩展类
    /// </summary>
    public static class CommonExtention
    {
        /// <summary>
        /// 返回int类型，非int类型返回0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ToInt(this string s)
        {
            int result;
            if (!int.TryParse(s, out result))
            {
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// 返回long类型，非long类型返回0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static long ToLong(this string s)
        {
            long result;
            if (!long.TryParse(s, out result))
            {
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// 返回short类型，非short类型返回0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static short ToShort(this string s)
        {
            short result;
            if (!short.TryParse(s, out result))
            {
                result = 0;
            }
            return result;
        }
        /// <summary>
        /// 返回bool类型，非bool类型返回false
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool ToBool(this string s)
        {
            bool result;
            if (!bool.TryParse(s, out result))
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 返回float类型，非float类型返回0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float ToFloat(this string s)
        {
            float result;
            if (!float.TryParse(s, out result))
            {
                result = 0f;
            }
            return result;
        }
        /// <summary>
        /// 返回double类型，非double类型返回0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double ToDouble(this string s)
        {
            double result;
            if (!double.TryParse(s, out result))
            {
                result = 0f;
            }
            return result;
        }
        /// <summary>
        /// 返回int类型数组，非int类型数组，返回同样个数的以0填充的数组
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int[] ToIntArray(this string[] array)
        {
            int[] temp = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                temp[i] = array[i].ToInt();
            }
            return temp;
        }
        /// <summary>
        /// 返回long类型数组，非long类型数组，返回同样个数的以0填充的数组
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static long[] ToLongArray(this string[] array)
        {
            long[] temp = new long[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                temp[i] = array[i].ToLong();
            }
            return temp;
        }
        /// <summary>
        /// 转换字符串到指定类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ToType(this string value, Type type)
        {
            return System.ComponentModel.TypeDescriptor.GetConverter(type).ConvertFrom(value);
        }
        /// <summary>
        /// 转换字符串到指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToType<T>(this string value)
        {
            return (T)ToType(value, typeof(T));
        }
        /// <summary>
        /// 返回datetime类型，失败返回minvalue
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string target)
        {
            DateTime result;
            if (!DateTime.TryParse(target, out result))
            {
                result = DateTime.MinValue;
            }
            return result;
        }
        /// <summary>
        /// 返回字符串类型，逗号分隔
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ToString(this List<int> list)
        {
            string[] temp = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                temp[i] = list[i].ToString();
            }
            return String.Join(",", temp);
        }
        /// <summary>
        /// 返回中文字符数
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int CnLength(this string target)
        {
            return Encoding.Default.GetByteCount(target);
        }
        /// <summary>
        /// 截取中文字符
        /// </summary>
        /// <param name="target"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CnSubstring(this string target, int startIndex, int length)
        {
            byte[] subbyte = Encoding.Default.GetBytes(target);
            string sub = Encoding.Default.GetString(subbyte, startIndex, length);
            if (sub.Substring(sub.Length - 1, 1) == "?")
            {
                sub = sub.Substring(0, sub.Length - 1);
            }
            return sub;
        }
        /// <summary>
        /// 清除html标签
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string RemoveHtml(this string target)
        {
            Regex reg = new Regex(@"<(.+?)>");
            target = reg.Replace(target, "");
            return target;
        }
        /// <summary>
        /// 清除多余空格
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string RemoveSpace(this string target)
        {
            Regex reg = new Regex(@"\s{2,}");
            target = reg.Replace(target, " ");
            return target;
        }
        /// <summary>
        /// 过滤非法字符
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string FilterChar(this string target)
        {
            Regex reg = new Regex("[^a-zA-Z0-9\\u4E00-\\u9FA5\\s\\.,\"':;!@\\(\\)-_\\{\\}\\[\\]，。？！：；“”‘’『』【】`·&%￥$（）]");
            target = reg.Replace(target, "");
            return target;
        }
        /// <summary>
        /// 验证手机号码
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CheckMobile(this string target)
        {
            Regex reg = new Regex(@"^1[3|4|5|7|8]\d{9}$");
            return reg.IsMatch(target);
        }
        /// <summary>
        /// 验证电子邮箱
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CheckEmail(this string target)
        {
            Regex reg = new Regex(@"^([a-zA-Z0-9_-])+@([a-zA-Z0-9-])+((\.[a-zA-Z0-9-]{2,3}){1,2})$");
            return reg.IsMatch(target);
        }
        /// <summary>
        /// 自定义64位加密
        /// </summary>
        /// <param name="target"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Encode64(this long target, int length = 0)
        {
            string result = String.Empty;
            string code = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-=";
            if (target >= 0 && target < 64)
            {
                result = code.Substring((int)(target % 64), 1);
            }
            else if (target >= 64)
            {
                while (target > 0)
                {
                    result = code.Substring((int)(target % 64), 1) + result;
                    target = target / 64;
                }
            }
            if (!String.IsNullOrEmpty(result) && length > 0)
            {
                result = result.PadLeft(length, '0');
            }
            return result;
        }
        /// <summary>
        /// 自定义32位加密
        /// </summary>
        /// <param name="target"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Encode32(this long target, int length = 0)
        {
            string result = String.Empty;
            string code = "123456789ABCDEFGHJKLMNPQRSTUVWXY";
            target = Math.Abs(target);
            if (target >= 0 && target < 32)
            {
                result = code.Substring((int)(target % 32), 1);
            }
            else if (target >= 32)
            {
                while (target > 0)
                {
                    result = code.Substring((int)(target % 32), 1) + result;
                    target = target / 32;
                }
            }
            if (!String.IsNullOrEmpty(result) && length > 0)
            {
                result = result.PadLeft(length, '0');
            }
            return result;
        }
        /// <summary>
        /// 自定义16位加密
        /// </summary>
        /// <param name="target"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Encode16(this long target, int length = 0)
        {
            string result = String.Empty;
            string code = "0123456789abcdef";
            if (target >= 0 && target < 16)
            {
                result = code[(int)target % 16].ToString();
            }
            else if (target >= 16)
            {
                while (target > 0)
                {
                    result += code.Substring((int)(target % 16));
                    target = target / 16;
                }
            }
            if (!String.IsNullOrEmpty(result) && length > 0)
            {
                result = result.PadLeft(length, '0');
            }
            return result;
        }
        /// <summary>
        /// 替换换行符，返回<br>标签
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string ReplaceWrap(this string target)
        {
            if (String.IsNullOrEmpty(target))
            {
                return target;
            }
            else
            {
                Regex reg = new Regex(@"((\\r\\n)|(\\r)|(\\n))+?");
                return reg.Replace(target, "<br/>");
            }
        }
        /// <summary>
        /// 替换换行符，返回\r\n
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string ReplaceBr(this string target)
        {
            if (String.IsNullOrEmpty(target))
            {
                return target;
            }
            else
            {
                Regex reg = new Regex(@"<(br)+?>");
                return reg.Replace(target, "\r\n");
            }
        }
        /// <summary>
        /// 返回时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long TimeStamp(this DateTime time)
        {
            if (time != null)
            {
                return (DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000;
            }
            else
            {
                return (time.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000;
            }
        }
        /// <summary>
        /// 返回datetime类型
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime? TimeStamp(this long timeStamp)
        {
            if (timeStamp > 0)
            {
                return new DateTime((timeStamp * 10000000 + new DateTime(1970, 1, 1).Ticks));
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 过滤超出长度字段
        /// </summary>
        /// <param name="target"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Truncate(this string target, int length)
        {
            int len = target.CnLength();
            if (len <= length)
            {
                return target;
            }
            else
            {
                return target.CnSubstring(0, length) + "...";
            }
        }
        /// <summary>
        /// 隐藏手机号码
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string EncodeMobile(this string target)
        {
            int len = target.Length;
            if (len != 11)
            {
                return String.Empty;
            }
            else
            {
                char[] chars = target.ToCharArray();
                for (int i = 0; i < chars.Length; i++)
                {
                    if (i == 3 || i == 4 || i == 5 || i == 6)
                    {
                        chars[i] = '*';
                    }
                }
                return String.Join("", chars);
            }
        }
        /// <summary>
        /// web请求query转换成字典类型
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Dictionary<string, string> HttpQuery(this string query)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (string s in query.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] strArray = s.Split(new char[] { '=' });
                if (strArray.Length == 2)
                {
                    dictionary[strArray[0]] = strArray[1];
                }
                else
                {
                    dictionary[s] = null;
                }
            }
            return dictionary;
        }
        /// <summary>
        /// 转换成web请求query
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string BuildQuery(this Dictionary<string, string> dic)
        {
            List<string> list = new List<string>();
            foreach (var item in dic)
            {
                list.Add(item.Key + "=" + item.Value);
            }
            return String.Join("&", list);
        }
        /// <summary>
        /// 判断列表是否有值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool HasValue<T>(this IList<T> list)
        {
            return list != null && list.Count > 0;
        }
        /// <summary>
        /// 转换datetime类型到指定格式
        /// </summary>
        /// <param name="target"></param>
        /// <param name="format">指定格式</param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string target, string format)
        {
            try
            {
                return DateTime.ParseExact(target, format, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 16位转换
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToHexString(this string s)
        {

            byte[] bytes = Encoding.UTF8.GetBytes(s);
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }
        public static string To16String(this Guid id)
        {
            long i = 1;
            foreach (byte b in id.ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        public static long ToLongID(this Guid id)
        {
            byte[] buffer = id.ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }
    }
}
