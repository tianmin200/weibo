using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;
using Weibo.Common;
using Weibo.Models;

namespace Weibo
{

    public static class Alimama
    {
        public static string appkey = "23596105";
        public static string secret = "5cf839a91d5d86a953debf913f2702ec";

        public static CookieContainer Login()
        {
            string posturl = "https://login.taobao.com/member/login.jhtml?redirectURL=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1";
            string poststr = "TPL_username=tianmin200&TPL_password=tianmin3216779236&ncoSig=&ncoSessionid=&ncoToken=8bb84c14acdeac1ea8a25bd93ff9b3e8141bc70e&slideCodeShow=false&useMobile=false&lang=zh_CN&loginsite=0&newlogin=&TPL_redirect_url=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1&from=alimama&fc=default&style=mini&css_style=&keyLogin=false&qrLogin=true&newMini=false&newMini2=true&tid=&loginType=3&minititle=&minipara=&pstrong=&sign=&need_sign=&isIgnore=&full_redirect=true&sub_jump=&popid=&callback=&guf=&not_duplite_str=&need_user_id=&poy=&gvfdcname=&gvfdcre=&from_encoding=&sub=false&TPL_password_2=&loginASR=1&loginASRSuc=0&allp=&oslanguage=&sr=&osVer=&naviVer=&osACN=&osAV=&osPF=&miserHardInfo=&appkey=&bind_login=false&bind_token=&nickLoginLink=&mobileLoginLink=https%3A%2F%2Flogin.taobao.com%2Fmember%2Flogin.jhtml%3Fstyle%3Dmini%26newMini2%3Dtrue%26from%3Dalimama%26redirectURL%3Dhttp%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1%26full_redirect%3Dtrue%26disableQuickLogin%3Dtrue%26useMobile%3Dtrue";
            CookieContainer cc = new CookieContainer();
            string refer = "http://login.taobao.com/member/taobaoke/login.htm/is_login=1";
            string result = HttpHelper1.SendDataByPost(posturl, poststr, refer, ref cc);
            string strurl = "https://www.alimama.com/membersvc/my.htm?domain=taobao&service=user_on_taobao&sign_account=a49f9f62502fc96517a626ea7e685eb4";
            string resultstr = HttpHelper1.SendDataByGET(strurl, ref cc);
            bool isLogin = Alimama.TestLogin(cc);
            return cc;
        }
        /// <summary>
        /// 阿里妈妈搜索店铺
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pv_id"></param>
        /// <param name="alimamacc"></param>
        /// <returns></returns>
        public static AlimamaSearchShopData GetSearchData(string query,string pv_id, CookieContainer alimamacc)
        {
            AlimamaSearchShopData alimamasearchdata = new AlimamaSearchShopData();
            string tbtoken = Alimama.GetTbToken(alimamacc);
            string url = "http://pub.alimama.com/shopsearch/shopList.json?spm=a2320.7388781.a214tr8.d006.QkMiw1&q="+HttpUtility.UrlEncode(query)+"&toPage=1&perPagesize=40&t="+HttpHelper1.GetTicks()+"&pvid="+pv_id+"&_tb_token_="+tbtoken+"&_input_charset=utf-8";
            string result = HttpHelper1.SendDataByGET(url, ref alimamacc);
            alimamasearchdata = Newtonsoft.Json.JsonConvert.DeserializeObject<AlimamaSearchShopData>(result);
            return alimamasearchdata;
        }

        /// <summary>
        /// 获取店铺热销单品数据
        /// </summary>
        /// <param name="oriMemberId"></param>
        /// <param name="pv_id"></param>
        /// <param name="alimamacc"></param>
        /// <returns></returns>
        public static AlimamaShopHotData GetShopHotItem(string oriMemberId,string pv_id, CookieContainer alimamacc)
        {
            AlimamaShopHotData alimamashophot = new AlimamaShopHotData();
            string url = "http://pub.alimama.com/shopdetail/hotProducts.json?sortField=_totalnum&oriMemberId="+ oriMemberId + "&t="+HttpHelper1.GetTicks()+"&pvid="+ pv_id + "&_tb_token_="+GetTbToken(alimamacc)+"&_input_charset=utf-8";
            string result = HttpHelper1.SendDataByGET(url, ref alimamacc);
            alimamashophot = Newtonsoft.Json.JsonConvert.DeserializeObject<AlimamaShopHotData>(result);
            return alimamashophot;
        }

        
        public static TbkUatmFavoritesGetResponse GetXuanpinku()
        {
            string url = "http://gw.api.taobao.com/router/rest";
            
            ITopClient client = new DefaultTopClient(url, Alimama.appkey, Alimama.secret);
            TbkUatmFavoritesGetRequest req = new TbkUatmFavoritesGetRequest();
            req.PageNo = 1L;
            req.PageSize = 50L;
            req.Fields = "favorites_title,favorites_id,type";
            //req.Type = 1L;
            return client.Execute(req);
        }

        public static TbkUatmFavoritesItemGetResponse GetXuanpinkuItem(long AdzonId,long FavoritesId)
        {
            string url = "http://gw.api.taobao.com/router/rest";
            ITopClient client = new DefaultTopClient(url, appkey, secret);
            TbkUatmFavoritesItemGetRequest req = new TbkUatmFavoritesItemGetRequest();
            //req.Platform = 1L;
            req.PageSize = 20L;
            req.AdzoneId = 70220294L;
            req.Unid = "3456";
            req.FavoritesId = FavoritesId;
            //req.PageNo = 2L;
            req.Fields = "num_iid,title,pict_url,small_images,reserve_price,zk_final_price,user_type,provcity,item_url,seller_id,volume,nick,shop_title,zk_final_price_wap,event_start_time,event_end_time,tk_rate,status,type";
            return client.Execute(req);
            
        }
        /// <summary>
        /// 获取淘宝联盟选品库列表地址
        /// </summary>
        public static string url_xuanpinku = "http://pub.alimama.com/favorites/group/newList.json?toPage=1&perPageSize=50";
        public static string fileDownLoadUrl = "http://pub.alimama.com/favorites/item/export.json?adzoneId=69828234&siteId=20546221&groupId={0}";
        /// <summary>
        /// 获取当前Cookie
        /// </summary>
        /// <returns></returns>
        public static CookieContainer GetCookieContainer()
        {
            CookieContainer cc = new CookieContainer();
            //初始化Cookie
            string str3 = Directory.GetCurrentDirectory();
            string cookiestr = File.ReadAllText(str3 +"/alimamacookie.txt");
            CookieCollection ccl = CookieHelper.GetCookieCollectionByString(cookiestr, "alimama.com");

            cc.Add(ccl);
            return cc;
        }

        public static bool TestLogin(CookieContainer cc)
        {
            bool isLogin = false;
            //验证登陆
            string m_url = "http://www.alimama.com/getLogInfo.htm?callback=__jp0";//地址获取淘宝联盟选品库列表
            string jsonlist = HttpHelper1.SendDataByGET(m_url,"http://www.alimama.com/index.htm", ref cc);
            if (jsonlist.Contains("success\":true"))//未登陆则打开登陆接口
            {
                isLogin = true;
            }
            else
                isLogin = false;
            return isLogin;
        }

        /// <summary>
        /// 搜索淘宝客商品
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static TbkItemGetResponse GetItemsWithQuery(string query)
        {
            string url = "http://gw.api.taobao.com/router/rest";
            ITopClient client = new DefaultTopClient(url, appkey, secret);
            TbkItemGetRequest req = new TbkItemGetRequest();
            req.Fields = "num_iid,title,pict_url,small_images,reserve_price,zk_final_price,user_type,provcity,item_url,seller_id,volume,nick";
            req.Q = query;
            //req.Cat = "16,18";
            //req.Itemloc = "杭州";
            req.Sort = "tk_rate_des";
            req.IsTmall = true;
            req.IsOverseas = false;
            //req.StartPrice = 10L;
            //req.EndPrice = 10L;
            //req.StartTkRate = 123L;
            //req.EndTkRate = 123L;
            //req.Platform = 1L;
            //req.PageNo = 123L;
            //req.PageSize = 20L;
            TbkItemGetResponse rsp = client.Execute(req);
            return rsp;
        }

        /// <summary>
        /// 获取淘宝客链接
        /// </summary>
        /// <param name="itemurl">淘宝商品URL</param>
        /// <param name="adzonid">推广组ID</param>
        /// <param name="alimamacc">Cookie</param>
        /// <returns></returns>
        public static string GetTbkLink(string itemurl,string siteid,string adzoneid,CookieContainer alimamacc)
        {
            string tbkLink = "";
            CookieCollection ccl = alimamacc.GetCookies(new Uri("http://alimama.com"));
            string tbtoken = ccl["_tb_token_"].Value;
            string url = "http://pub.alimama.com/urltrans/urltrans.json?adzoneid="+ adzoneid + "&promotionURL="+System.Web.HttpUtility.UrlEncode(itemurl);
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1);
            string ticks = ts.Ticks.ToString().Substring(0, 13);
            url = "http://pub.alimama.com/urltrans/urltrans.json?siteid="+siteid+"&adzoneid=" + adzoneid + "&promotionURL=" + System.Web.HttpUtility.UrlEncode(itemurl) +"&t="+ ticks+ "&_tb_token_="+ tbtoken + "&_input_charset=utf-8";
            string result = HttpHelper1.SendDataByGET(url, ref alimamacc);
            if (result.Contains("链接不支持转化")) return "链接不支持转化";
            if (result.Contains("阿里妈妈")) return "阿里妈妈登录失效";
            if (result.Contains("亲，访问受限了"))
            {
                Thread.Sleep(1 * 60 * 1000);
                GetTbkLink(itemurl,siteid, adzoneid, alimamacc);
                
                //访问受限，等待1分钟
            }
            HttpHelper1.GetStringInTwoKeyword(result, ref tbkLink, "shortLinkUrl\":\"", "\"}", 0);
            
            return tbkLink;
        }

        public static string GetTbToken(CookieContainer alimamacc)
        {
            if (alimamacc == null) return "";
            CookieCollection ccl = alimamacc.GetCookies(new Uri("http://alimama.com"));
            string tbtoken = ccl["_tb_token_"].Value;
            return tbtoken;
        }

        /// <summary>
        /// 获取阿里妈妈搜索商品url内容
        /// </summary>
        /// <param name="itemurl"></param>
        /// <param name="alimamacc"></param>
        /// <returns></returns>
        public static string SearchItem(string itemurl,CookieContainer alimamacc)
        {
            AlimamaSearchData alimama_searchdata = new AlimamaSearchData();
            CookieCollection ccl = alimamacc.GetCookies(new Uri("http://alimama.com"));
            string tbtoken = ccl["_tb_token_"].Value;
            string searchurl = "http://pub.alimama.com/items/search.json?queryType=2&q="+HttpUtility.UrlEncode(itemurl)+"&auctionTag=&perPageSize=40&shopTag=&t="+HttpHelper1.GetTicks()+"&_tb_token_="+tbtoken+"&pvid=10_118.145.0.50_464_1484650244904";

            
            string refer = "http://pub.alimama.com/promo/search/index.htm?queryType=2&q=" + HttpUtility.UrlEncode(itemurl);
            string resulthtml = HttpHelper1.SendDataByGET(searchurl,refer, ref alimamacc);
            //alimama_searchdata = Newtonsoft.Json.JsonConvert.DeserializeObject<AlimamaSearchData>(resulthtml);
            //string campaignstr = "";
            
            //HttpHelper1.GetStringInTwoKeyword(resulthtml, ref campaignstr, "", "", 0);
            
            return resulthtml;
        }
        public static bool IsCampaignExits(string itemid,string tbtoken, string pv_id,CookieContainer alimamacc,ref string keeperid)
        {
            try
            {
                string url = "http://pub.alimama.com/pubauc/getCommonCampaignByItemId.json?itemId=" + itemid + "&t=" + HttpHelper1.GetTicks() + "&_tb_token_=" + tbtoken + "&pvid=" + pv_id;
                string resulthtml = HttpHelper1.SendDataByGET(url, ref alimamacc);
                if (!resulthtml.Contains("Exist")) return true;
                if (resulthtml.Contains("Exist\":true")) return true;
                HttpHelper1.GetStringInTwoKeyword(resulthtml, ref keeperid, "ShopKeeperID\":", ",\"", 0);
                Regex reg = new Regex("CampaignName\":\".*\",\"");
                MatchCollection mc = reg.Matches(resulthtml);
                foreach (Match match in mc)
                {
                    string campaignname = match.Value.Replace("CampaignName\":\"", "").Replace("\",\"CampaignType", "") + "\r\n";
                    File.AppendAllText("taobaoke定向计划.txt", campaignname);
                }
                return false;
            }
            catch (Exception)
            {
                return true;
            }
           
        }
        /// <summary>
        /// 自动申请定向计划
        /// </summary>
        /// <param name="campaignid">定向计划ID</param>
        /// <returns>true:申请成功，false:没有定向计划</returns>
        public static bool ApplyCampaign(string resulthtml,string itemurl, CookieContainer alimamacc)
        {
            string campaignstrs = "";
            HttpHelper1.GetStringInTwoKeyword(resulthtml, ref campaignstrs, "tkSpecialCampaignIdRateMap\":", ",\"eventCreatorId", 0);
            if (campaignstrs == "null") return false;//如果没有定向计划，退出
            
            string pv_id = "";
            HttpHelper1.GetStringInTwoKeyword(resulthtml, ref pv_id, "pvid\":\"", "\",\"docsfound", 0);
            string itemid = "";
            HttpHelper1.GetStringInTwoKeyword(resulthtml, ref itemid, "auctionId\":",",\"", 0);
            string keeperid = "";
            
            CookieCollection ccl = alimamacc.GetCookies(new Uri("http://alimama.com"));
            string tbtoken = ccl["_tb_token_"].Value;
            string refer = "http://pub.alimama.com/promo/search/index.htm?queryType=2&q=" + HttpUtility.UrlEncode(itemurl);
            if (IsCampaignExits(itemid, tbtoken, pv_id, alimamacc,ref keeperid)) return false;//如果申请过定向计划，退出

            string[] campains = campaignstrs.Split(',');
            foreach (string campain in campains)
            {
                string[] campainpro = campain.Split(':');
                string campaignid = campainpro[0].Replace("{","").Replace("\"","");
                string campaignbili = campainpro[1].Replace("\"","").Replace("}", "");
                bool isSuc = ApplyCampaign(campaignid, keeperid,pv_id,refer, alimamacc);
            }
            return true;
        }
        public static bool ApplyCampaign(AlimamaSearchData alimama_searchdata)
        {
            //alimama_searchdata.Data.PageList[0].TkSpecialCampaignIdRateMap
            return true;
        }
        public static bool ApplyCampaign(string campaignid,string keeperid,string pv_id,string refer,CookieContainer alimamacc)
        {
            //alimama_searchdata.Data.PageList[0].TkSpecialCampaignIdRateMap
            string url = "http://pub.alimama.com/pubauc/applyForCommonCampaign.json";
            CookieCollection ccl = alimamacc.GetCookies(new Uri("http://alimama.com"));
            string tbtoken = ccl["_tb_token_"].Value;
            string poststr = "campId="+campaignid+"&keeperid="+ keeperid + "&applyreason=%E5%BE%AE%E5%8D%9A%E5%AF%BC%E8%B4%AD&t="+HttpHelper1.GetTicks()+"&_tb_token_="+ tbtoken + "&pvid="+pv_id;
            
            string resulthtml = HttpHelper1.SendDataByPost(url,poststr,refer,ref alimamacc);
            return true;
        }
        /// <summary>
        /// 获取商品优惠券信息
        /// </summary>
        public static string GetCouponInfo(string resulthtml,string itemurl,CookieContainer alimamacc)
        {
            string pv_id = "";
            HttpHelper1.GetStringInTwoKeyword(resulthtml, ref pv_id, "pvid\":\"", "\",\"docsfound", 0);
            string itemid = "";
            HttpHelper1.GetStringInTwoKeyword(resulthtml, ref itemid, "auctionId\":",",\"", 0);
            CookieCollection ccl = alimamacc.GetCookies(new Uri("http://alimama.com"));
            string tbtoken = ccl["_tb_token_"].Value;
            string url = "http://pub.alimama.com/common/code/getAuctionCode.json?auctionid=" + itemid + "&adzoneid=69828234&siteid=20546221&scenes=1&t=" + HttpHelper1.GetTicks() + "&_tb_token_=" + tbtoken + "&pvid=" + pv_id;
            string result = HttpHelper1.SendDataByGET(url,ref alimamacc);
            string shorturl = "";
            HttpHelper1.GetStringInTwoKeyword(result,ref shorturl, "couponShortLinkUrl\":\"", "\",\"qrCodeUrl", 0);
            return shorturl;
        }
        /// <summary>
        /// 根据微博短地址获取淘宝真实地址及网页数据
        /// </summary>
        /// <param name="weiboshorturl"></param>
        /// <param name="alimamacc"></param>
        /// <param name="tbrealurl"></param>
        /// <returns></returns>
        public static string GetItemResultWithWeiboShortUrl(string weiboshorturl, CookieContainer alimamacc, ref string tbrealurl)
        {
            //一次跳转，微博短链接 -> 淘宝客短链接
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(weiboshorturl);
            req.AllowAutoRedirect = false;
            WebResponse rsp = req.GetResponse();
            string tbkurl = rsp.Headers["location"];
            req.Abort();
            rsp.Close();
            rsp.Dispose();

            //二次跳转，淘宝客短链接 -> 淘宝客长链接
            req = (HttpWebRequest)HttpWebRequest.Create(tbkurl);
            req.AllowAutoRedirect = false;
            rsp = req.GetResponse();
            tbkurl = rsp.Headers["location"];
            req.Abort();
            rsp.Close();
            rsp.Dispose();

            //三次跳转，淘宝客长链接 -> 淘宝Item跳转地址
            req = (HttpWebRequest)HttpWebRequest.Create(tbkurl);
            req.AllowAutoRedirect = false;
            rsp = req.GetResponse();
            tbkurl = rsp.Headers["location"];
            if (tbkurl == null) tbkurl = rsp.ResponseUri.ToString();
            //释放资源
            req.Abort();
            rsp.Close();
            rsp.Dispose();

            //四次跳转，淘宝Item真实地址
            string tu = tbkurl.Replace("https://s.click.taobao.com/t_js?tu=", "");
            tu = HttpUtility.UrlDecode(tu);

            string result = HttpHelper1.GetHttpsHtml(tu, tbkurl, ref tbrealurl);
            return result;
        }

        
        /// <summary>
        /// 抓取完微博内容之后，截取想要的内容
        /// </summary>
        /// <returns></returns>
        public static string CutWeiboText()
        {
            return "";
        }
    }
}
