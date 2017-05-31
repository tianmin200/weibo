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

namespace Weibo.Common
{

    public static class Alimama
    {
        public static string appkey = "23866281";//"23596105"; 
        public static string secret = "a31cae562bc3de05e824ab9fa2d035b7";//5cf839a91d5d86a953debf913f2702ec";

        public static CookieContainer Login()
        {
            string alimamaAccount = File.ReadAllText("config/alimamaAccount.txt");

            string posturl = "https://login.taobao.com/member/login.jhtml?redirectURL=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1";
            string poststr = alimamaAccount.Split(',')[0];
            //string poststr = "TPL_username=13811277391&TPL_password=tianmin3216779236&ncoSig=&ncoSessionid=&ncoToken=b642ddda7d991ecd54929e776b8a2e5341006d8a&slideCodeShow=false&useMobile=false&lang=zh_CN&loginsite=0&newlogin=&TPL_redirect_url=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1&from=alimama&fc=default&style=mini&css_style=&keyLogin=false&qrLogin=true&newMini=false&newMini2=true&tid=&loginType=3&minititle=&minipara=&pstrong=&sign=&need_sign=&isIgnore=&full_redirect=true&sub_jump=&popid=&callback=&guf=&not_duplite_str=&need_user_id=&poy=&gvfdcname=10&gvfdcre=687474703A2F2F7075622E616C696D616D612E636F6D2F3F73706D3D61323139742E373636343535342E613231347472382E372E767753705056&from_encoding=&sub=&TPL_password_2=&loginASR=1&loginASRSuc=0&allp=&oslanguage=zh-CN&sr=1366*768&osVer=&naviVer=chrome%7C45.02454101&osACN=Mozilla&osAV=5.0+%28Windows+NT+10.0%3B+WOW64%29+AppleWebKit%2F537.36+%28KHTML%2C+like+Gecko%29+Chrome%2F45.0.2454.101+Safari%2F537.36&osPF=Win32&miserHardInfo=&appkey=&bind_login=false&bind_token=&nickLoginLink=&mobileLoginLink=https%3A%2F%2Flogin.taobao.com%2Fmember%2Flogin.jhtml%3Fstyle%3Dmini%26newMini2%3Dtrue%26from%3Dalimama%26redirectURL%3Dhttp%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1%26full_redirect%3Dtrue%26disableQuickLogin%3Dtrue%26useMobile%3Dtrue&showAssistantLink=&um_token=HV01PAAZ0be2fcc8769100dc58abe2630034bd29&ua=086%23vM3XKrXlms3XZrm6XXXXXnN70qbE80hRLkSB0NsL1OUnpSiYiIEw6f6gR7nniPZRCkHFySiYyMA37aKh8rvRg5tgz5iLtsnmXmOsTmfmyvvgH3uVP21SbwmHvE9xT5d9mr3X1WwIyO37XA5qyQGU5O3aoIJQdBDUCa6EVk31X8XOwItTyRyYRMphdoGCyT62MqdHbWqua%2BsTo8fmXmzIwv1TU8scoOsJwp1TukjPe4B9wSaaSQ1Mmr3X1WwIyO37rLbeyQGU5O3aoIJQdBDUCa6EVk31X8XOwItTyRAYI1hhdoGCyT62MqdHbWqua%2BsTo8fmXmzIwv1TUfiidxsJwp1TukjPe4B9wSaaSQ1Mmr3X1WwIyO378ejyyQGU5O3aoIJQdBDUCa6EVk31X8XOwItTyRymkLlhdoGCyT62MqdHbWqua%2BsTo8fmXmzIwv1TUfP%2BLxsJwp1TukjPe4B9wSaaSQ1MXX3XuPVV0GgUXKRLFNlk9GA7eiSm%2FaHsXX3XuPV90GKZWJRLFNeWDsn7%2BEWJ%2FuHsmr3XuWwyyO37BgY%2FyQGU7m6ho4KLw0RUCRSmXXT%2B60BBmr3XuWwByO37B0%2BEyQGU7m6ho4KLw0RUC8fmXmDIBO1TUD53%2FvsJwp8ayePHSgGVwS6tX8X%2BRVRvX40kHPMmae5mQpf2kw8dfzhIGTuVPz%2FXX8X%2FHRhB02Ei7UIB%2BQMWoLclvM78agoqX8X36ceBBGnmXmxFTTomGUMgH3usA21Sbt9fiY4oIsPuP6ySvGnmXmOsTh3mGKbgH3uvMr1SbwmHvE9xT5d9UX3X35db22mMT2mS0xPD3MrrhdIT5M55X8XRLHVmXPOPX1jzy4WR1YmsWMPLPl2mXX8f11myUr3Xmux%2BBBg%2BX8X1f6ACBymIyr3XYXXX2THi6uRTCr3Xm0YzBBoVZ%2BfmXYCpi5QrXXXbF65eAbXXXjdckE0kgXXXFL7pib02XXXB7N5eNBXXXjbWrX3mnAqiH%2FWd8wmXPh4hHRZWnVSXsrcSIunchyXL3zb2PhuIj1yFnjZWnVSXsC4BiVfSuM9xm1rqmtcXX1ckZRxZY6XX%2B%2Fbti9yMa3rpjNXtjtavmNSWWR5fZcIif2blF3rT8tnHu63e6NuR6Nrg169f4LmsJ%2FCq1D%2FeRX4x%2FtWK%2FhOPXhgJWRnD4yZW1Cm9IcXC%2FVQ7hPSOkhaij1VhJRSH4yZW1CuKUu3lmXOt3jaQ6PWMmd8ERLmDs%2FZsJ%2FCGU2xEmmQd3%2F5QhYmvPPyJ%2BAn6ZyIssTc2s3VAhrQ%2BY13ga2CzY6XRsEQOIa81Wrc7O%2Fxe1MQ7%2Fwnw6EcnXNvRIjm4%2B984kTXQFrM9RmSVhzMxDDu36z8WsA7%2BI3v6J3xyia5DOw7SEr3XY9TaBrQkJGNTjwTOQX3X1221nNtIvN4o4zn9TQ%2FJhyVyZVCm96%2FtsL8wTwrmXmUZBW3X43WXfIAT9MIfn3Qw3JIJUX3Xugdbm2mgT2mS2vPDaXnp19o8nC9fXt5kUX3X35dbEXmqT2mSf7PD3MrrhdIT5M5qX8X36ceBBwSmXXT%2B60BBUr3Xmux%2BBBgqX8X36ceBB2fmXmzIwv1TU%2FYPV7sJwp1TukjPe4B9wSaaSQ1Mmr3X1WwIyO378Sj%2FyQGU5O3aoIJQdBDUCa6EVk3XX8X%2FHR6B0MET7UIB%2Bx0DpkAolruTago%3D";
            CookieContainer cc = new CookieContainer();
            string refer = "http://login.taobao.com/member/taobaoke/login.htm/is_login=1";
            string result = HttpHelper1.SendDataByPost(posturl, poststr, refer, ref cc);
            string strurl = alimamaAccount.Split(',')[1];
            //string strurl = "https://www.alimama.com/membersvc/my.htm?domain=taobao&service=user_on_taobao&sign_account=63f4941326712960165e9e278c31a7ad";
            string resultstr = HttpHelper1.SendDataByGET(strurl, ref cc);
            bool isLogin = Alimama.TestLogin(cc);
            return cc;
        }
        public static CookieContainer Login(string username, string password)
        {
            string posturl = "https://login.taobao.com/member/login.jhtml?redirectURL=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1";
            string poststr = "TPL_username=" + username + "&TPL_password=" + password + "&ncoSig=&ncoSessionid=&ncoToken=8bb84c14acdeac1ea8a25bd93ff9b3e8141bc70e&slideCodeShow=false&useMobile=false&lang=zh_CN&loginsite=0&newlogin=&TPL_redirect_url=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1&from=alimama&fc=default&style=mini&css_style=&keyLogin=false&qrLogin=true&newMini=false&newMini2=true&tid=&loginType=3&minititle=&minipara=&pstrong=&sign=&need_sign=&isIgnore=&full_redirect=true&sub_jump=&popid=&callback=&guf=&not_duplite_str=&need_user_id=&poy=&gvfdcname=&gvfdcre=&from_encoding=&sub=false&TPL_password_2=&loginASR=1&loginASRSuc=0&allp=&oslanguage=&sr=&osVer=&naviVer=&osACN=&osAV=&osPF=&miserHardInfo=&appkey=&bind_login=false&bind_token=&nickLoginLink=&mobileLoginLink=https%3A%2F%2Flogin.taobao.com%2Fmember%2Flogin.jhtml%3Fstyle%3Dmini%26newMini2%3Dtrue%26from%3Dalimama%26redirectURL%3Dhttp%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1%26full_redirect%3Dtrue%26disableQuickLogin%3Dtrue%26useMobile%3Dtrue";
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
        public static AlimamaSearchShopData GetSearchData(string query, string pv_id, CookieContainer alimamacc)
        {
            AlimamaSearchShopData alimamasearchdata = new AlimamaSearchShopData();
            string tbtoken = Alimama.GetTbToken(alimamacc);
            string url = "http://pub.alimama.com/shopsearch/shopList.json?spm=a2320.7388781.a214tr8.d006.QkMiw1&q=" + HttpUtility.UrlEncode(query) + "&toPage=1&perPagesize=40&t=" + HttpHelper1.GetTicks() + "&pvid=" + pv_id + "&_tb_token_=" + tbtoken + "&_input_charset=utf-8";
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
        public static AlimamaShopHotData GetShopHotItem(string oriMemberId, string pv_id, CookieContainer alimamacc)
        {
            AlimamaShopHotData alimamashophot = new AlimamaShopHotData();
            string url = "http://pub.alimama.com/shopdetail/hotProducts.json?sortField=_totalnum&oriMemberId=" + oriMemberId + "&t=" + HttpHelper1.GetTicks() + "&pvid=" + pv_id + "&_tb_token_=" + GetTbToken(alimamacc) + "&_input_charset=utf-8";
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

        public static string GetTbkShortUrl(string tbkurl)
        {
            //tbkurl= "https://uland.taobao.com/coupon/edetail?activityId=cca299fffaee4c45bbfa541ac208dd16&pid=mm_122033678_24252915_81212616&itemId=547482547612&src=fklm_hltk&dx=1";
            string url = "http://gw.api.taobao.com/router/rest";

            ITopClient client = new DefaultTopClient(url, Alimama.appkey, Alimama.secret);

            TbkSpreadGetRequest req = new TbkSpreadGetRequest();
            List<TbkSpreadGetRequest.TbkSpreadRequestDomain> list2 = new List<TbkSpreadGetRequest.TbkSpreadRequestDomain>();
            TbkSpreadGetRequest.TbkSpreadRequestDomain obj3 = new TbkSpreadGetRequest.TbkSpreadRequestDomain();
            list2.Add(obj3);
            obj3.Url = tbkurl;
            req.Requests_ = list2;
            TbkSpreadGetResponse rsp1 = client.Execute(req);
            string tbkshorturl = "";
            HttpHelper1.GetStringInTwoKeyword(rsp1.Body, ref tbkshorturl, "<content>", "</content>", 0);
            return tbkshorturl;
        }

        public static string GetTbkTaokouling(string tburl)
        {
            //tbkurl= "https://uland.taobao.com/coupon/edetail?activityId=cca299fffaee4c45bbfa541ac208dd16&pid=mm_122033678_24252915_81212616&itemId=547482547612&src=fklm_hltk&dx=1";
            string url = "http://gw.api.taobao.com/router/rest";

            ITopClient client = new DefaultTopClient(url, appkey, secret);
            WirelessShareTpwdCreateRequest req = new WirelessShareTpwdCreateRequest();
            WirelessShareTpwdCreateRequest.GenPwdIsvParamDtoDomain obj1 = new WirelessShareTpwdCreateRequest.GenPwdIsvParamDtoDomain();
            obj1.Ext = "{\"xx\":\"xx\"}";
            obj1.Logo = "http://m.taobao.com/xxx.jpg";
            obj1.Url = "http://m.taobao.com";
            obj1.Text = "超值活动，惊喜活动多多";
            obj1.UserId = 24234234234L;
            req.TpwdParam_ = obj1;
            WirelessShareTpwdCreateResponse rsp = client.Execute(req);
            
            string tbkshorturl = "";
            HttpHelper1.GetStringInTwoKeyword(rsp.Body, ref tbkshorturl, "<content>", "</content>", 0);
            return rsp.Body;
        }

        public static TbkUatmFavoritesItemGetResponse GetXuanpinkuItem(long AdzonId, long FavoritesId)
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
            string cookiestr = File.ReadAllText(str3 + "/config/alimamacookie.txt");
            CookieCollection ccl = CookieHelper.GetCookieCollectionByString(cookiestr, "alimama.com");

            cc.Add(ccl);
            return cc;
        }

        public static bool TestLogin(CookieContainer cc)
        {
            bool isLogin = false;
            //验证登陆
            string m_url = "https://www.alimama.com/getLogInfo.htm?callback=__jp0";//地址获取淘宝联盟选品库列表
            string jsonlist = HttpHelper1.SendDataByGET(m_url, "https://www.alimama.com/index.htm", ref cc);
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
        public static string GetTbkLink(string itemurl, string siteid, string adzoneid, CookieContainer alimamacc)
        {
            string tbkLink = "";
            CookieCollection ccl = alimamacc.GetCookies(new Uri("http://alimama.com"));
            string tbtoken = ccl["_tb_token_"].Value;
            string url = "http://pub.alimama.com/urltrans/urltrans.json?adzoneid=" + adzoneid + "&promotionURL=" + System.Web.HttpUtility.UrlEncode(itemurl);
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1);
            string ticks = ts.Ticks.ToString().Substring(0, 13);
            url = "http://pub.alimama.com/urltrans/urltrans.json?siteid=" + siteid + "&adzoneid=" + adzoneid + "&promotionURL=" + System.Web.HttpUtility.UrlEncode(itemurl) + "&t=" + ticks + "&_tb_token_=" + tbtoken + "&_input_charset=utf-8";
            string result = HttpHelper1.SendDataByGET(url, ref alimamacc);
            if (result.Contains("链接不支持转化")) return "链接不支持转化";
            if (result.Contains("阿里妈妈")) return "阿里妈妈登录失效";
            if (result.Contains("亲，访问受限了"))
            {
                Thread.Sleep(1 * 60 * 1000);
                GetTbkLink(itemurl, siteid, adzoneid, alimamacc);

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
        public static string SearchItem(string itemurl, CookieContainer alimamacc)
        {
            AlimamaSearchData alimama_searchdata = new AlimamaSearchData();
            CookieCollection ccl = alimamacc.GetCookies(new Uri("http://alimama.com"));
            if (ccl["_tb_token_"] == null) return "阿里妈妈未登录";
            string tbtoken = ccl["_tb_token_"].Value;
            string searchurl = "http://pub.alimama.com/items/search.json?queryType=2&q=" + HttpUtility.UrlEncode(itemurl) + "&auctionTag=&perPageSize=40&shopTag=&t=" + HttpHelper1.GetTicks() + "&_tb_token_=" + tbtoken + "&pvid=10_118.145.0.50_464_1484650244904&yxjh=-1";


            string refer = "http://pub.alimama.com/promo/search/index.htm?queryType=2&q=" + HttpUtility.UrlEncode(itemurl);
            string resulthtml = HttpHelper1.SendDataByGET(searchurl, refer, ref alimamacc);
            //alimama_searchdata = Newtonsoft.Json.JsonConvert.DeserializeObject<AlimamaSearchData>(resulthtml);
            //string campaignstr = "";

            //HttpHelper1.GetStringInTwoKeyword(resulthtml, ref campaignstr, "", "", 0);

            return resulthtml;
        }
        public static bool IsCampaignExits(CampainsData campains, CookieContainer alimamacc, ref string keeperid)
        {
            try
            {
                bool isExits = false;
                foreach (CampainsData.Datum campain in campains.data)
                {
                    if (campain.Exist)
                    {
                        isExits = true;
                        break;
                    }

                }
                return isExits;
            }
            catch (Exception ex)
            { return false; }
            //    string resulthtml = "";
            //    if (!resulthtml.Contains("Exist")) return true;
            //    if (resulthtml.Contains("Exist\":true")) return true;
            //    HttpHelper1.GetStringInTwoKeyword(resulthtml, ref keeperid, "ShopKeeperID\":", ",\"", 0);
            //    Regex reg = new Regex("CampaignName\":\".*\",\"");
            //    MatchCollection mc = reg.Matches(resulthtml);
            //    foreach (Match match in mc)
            //    {
            //        string campaignname = match.Value.Replace("CampaignName\":\"", "").Replace("\",\"CampaignType", "") + "\r\n";
            //        File.AppendAllText("taobaoke定向计划.txt", campaignname);
            //    }
            //    return false;
            //}
            //catch (Exception)
            //{
            //    return true;
            //}

        }

        public static CampainsData GetCampainByTBItemID(string itemid, string tbtoken, string pv_id, ref CookieContainer alimamacc, ref string keeperid)
        {
            string url = "http://pub.alimama.com/pubauc/getCommonCampaignByItemId.json?itemId=" + itemid + "&t=" + HttpHelper1.GetTicks() + "&_tb_token_=" + tbtoken + "&pvid=" + pv_id;
            string resulthtml = HttpHelper1.SendDataByGET(url, ref alimamacc);
            while (resulthtml.Contains("查询商品佣金出错"))
            {
                Thread.Sleep(30 * 1000);
                resulthtml = HttpHelper1.SendDataByGET(url, ref alimamacc);
            }
            if (resulthtml.Contains("为空")) return null;
            else if (resulthtml.Contains("阿里妈妈"))//返回阿里妈妈首页，登录失效
            {
                alimamacc = Alimama.Login();
                resulthtml = HttpHelper1.SendDataByGET(url, ref alimamacc);
            }
            HttpHelper1.GetStringInTwoKeyword(resulthtml,ref keeperid, "ShopKeeperID\":", ",\"Properties",0);
            CampainsData campains = Newtonsoft.Json.JsonConvert.DeserializeObject<CampainsData>(resulthtml);
            return campains;
        }
        /// <summary>
        /// 自动申请定向计划
        /// </summary>
        /// <param name="campaignid">定向计划ID</param>
        /// <returns>true:申请成功，false:没有定向计划</returns>
        public static bool ApplyCampaign(string resulthtml, string itemurl, ref CookieContainer alimamacc,ref double maxRate)
        {
            try
            {
                string campaignstrs = "";
                HttpHelper1.GetStringInTwoKeyword(resulthtml, ref campaignstrs, "tkSpecialCampaignIdRateMap\":", ",\"eventCreatorId", 0);
                if (campaignstrs.StartsWith("null")) return false;//如果没有定向计划，退出

                string pv_id = "";
                HttpHelper1.GetStringInTwoKeyword(resulthtml, ref pv_id, "pvid\":\"", "\",\"docsfound", 0);
                string itemid = "";
                HttpHelper1.GetStringInTwoKeyword(resulthtml, ref itemid, "auctionId\":", ",\"", 0);
                string keeperid = "";
                string maxratestr = "";
                HttpHelper1.GetStringInTwoKeyword(resulthtml, ref maxratestr, "tkRate\":", "\"", 0);
                maxratestr = maxratestr.Replace(",", "");

                CookieCollection ccl = alimamacc.GetCookies(new Uri("http://alimama.com"));
                string tbtoken = ccl["_tb_token_"].Value;
                string refer = "http://pub.alimama.com/promo/search/index.htm?queryType=2&q=" + HttpUtility.UrlEncode(itemurl);
                CampainsData campainsdata = Alimama.GetCampainByTBItemID(itemid, tbtoken, pv_id,ref alimamacc,ref keeperid);
                if (campainsdata == null) return false;
                //if (IsCampaignExits(campainsdata, alimamacc,ref keeperid)) return false;//如果申请过定向计划，退出
                double maxCommission = 0;
                maxCommission = Convert.ToDouble(maxratestr);
                double maxManualCommission = 0;


                CampainsData.Datum maxCampaign = new CampainsData.Datum();
                CampainsData.Datum maxManualCampaign = new CampainsData.Datum();
                //挑选出自动审核计划中，佣金最高的一个
                foreach (CampainsData.Datum campain in campainsdata.data)
                {
                    if (campain.AvgCommission == "-") continue;
                    double commission = Convert.ToDouble(campain.AvgCommission.Replace(" %", ""));
                    if (campain.manualAudit == 1)//如果需要人工审核
                    {
                        if (commission > maxManualCommission)
                        {
                            maxManualCommission = commission;
                            maxManualCampaign = campain;
                        }
                    }
                    else
                    {

                        if (commission > maxCommission)
                        {
                            maxCommission = commission;
                            maxCampaign = campain;
                        }
                    }
                }
                if (!maxCampaign.Exist && maxCampaign.CampaignID != 0)
                    ApplyCampaign(maxCampaign.CampaignID.ToString(), keeperid, pv_id, refer, alimamacc);//自动审核最高分成比例的计划一定要申请
                maxRate = maxCommission;
                if (maxCommission < maxManualCommission)
                {
                    if (!maxManualCampaign.Exist)
                    {
                        ApplyCampaign(maxManualCampaign.CampaignID.ToString(), keeperid, pv_id, refer, alimamacc);//如果人工审核的计划分成比例要更高，则再申请人工审核计划
                        if (maxManualCampaign.CampaignName.Contains("QQ"))
                            File.AppendAllText("taobaoke定向计划.txt", "分成比例：" + maxManualCampaign.AvgCommission + " 计划名称：" + maxManualCampaign.CampaignName + "\r\n");
                    }
                    else
                    {
                        //如果人工审核计划已经通过审核
                        maxRate = maxManualCommission;
                    }
                }


                //string[] campains = campaignstrs.Split(',');
                //foreach (string campain in campains)
                //{
                //    string[] campainpro = campain.Split(':');
                //    string campaignid = campainpro[0].Replace("{","").Replace("\"","");
                //    string campaignbili = campainpro[1].Replace("\"","").Replace("}", "");
                //    bool isSuc = ApplyCampaign(campaignid, keeperid,pv_id,refer, alimamacc);
                //}
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
        public static bool ApplyCampaign(AlimamaSearchData alimama_searchdata)
        {
            //alimama_searchdata.Data.PageList[0].TkSpecialCampaignIdRateMap
            return true;
        }
        public static bool ApplyCampaign(string campaignid, string keeperid, string pv_id, string refer, CookieContainer alimamacc)
        {
            //alimama_searchdata.Data.PageList[0].TkSpecialCampaignIdRateMap
            string url = "http://pub.alimama.com/pubauc/applyForCommonCampaign.json";
            CookieCollection ccl = alimamacc.GetCookies(new Uri("http://alimama.com"));
            string tbtoken = ccl["_tb_token_"].Value;
            string poststr = "campId=" + campaignid + "&keeperid=" + keeperid + "&applyreason=%e5%be%ae%e5%8d%9a%e5%a4%a7%e5%8f%b7%e5%af%bc%e8%b4%ad%ef%bc%8cQQ%ef%bc%9a1145837517&t=" + HttpHelper1.GetTicks() + "&_tb_token_=" + tbtoken + "&pvid=" + pv_id;

            string resulthtml = HttpHelper1.SendDataByPost(url, poststr, refer, ref alimamacc);
            return true;
        }
        /// <summary>
        /// 获取商品优惠券信息
        /// </summary>
        public static string GetCouponInfo(string resulthtml, string itemurl, CookieContainer alimamacc)
        {
            string pv_id = "";
            HttpHelper1.GetStringInTwoKeyword(resulthtml, ref pv_id, "pvid\":\"", "\",\"", 0);
            string itemid = "";
            HttpHelper1.GetStringInTwoKeyword(resulthtml, ref itemid, "auctionId\":", ",\"", 0);
            CookieCollection ccl = alimamacc.GetCookies(new Uri("http://alimama.com"));
            string tbtoken = ccl["_tb_token_"].Value;
            DEWeiboAccount deweiboaccount = WeiboHandler.GetOneAccount();
            string url = "http://pub.alimama.com/common/code/getAuctionCode.json?auctionid=" + itemid + "&adzoneid="+ deweiboaccount .Adzoneid+ "&siteid="+deweiboaccount.Siteid+"&scenes=1&t=" + HttpHelper1.GetTicks() + "&_tb_token_=" + tbtoken + "&pvid=" + pv_id+"&yxjh=-1";
            string result = HttpHelper1.SendDataByGET(url, ref alimamacc);
            string shorturl = "";
            HttpHelper1.GetStringInTwoKeyword(result, ref shorturl, "couponShortLinkUrl\":\"", "\",\"qrCodeUrl", 0);
            return shorturl;
        }
        public static string GetCouponInfo(string resulthtml, string itemurl, CookieContainer alimamacc,ref string tbkshortlink)
        {
            string pv_id = "";
            HttpHelper1.GetStringInTwoKeyword(resulthtml, ref pv_id, "pvid\":\"", "\",\"", 0);
            string itemid = "";
            HttpHelper1.GetStringInTwoKeyword(resulthtml, ref itemid, "auctionId\":", ",\"", 0);
            CookieCollection ccl = alimamacc.GetCookies(new Uri("http://alimama.com"));
            string tbtoken = ccl["_tb_token_"].Value;
            DEWeiboAccount deweiboaccount = WeiboHandler.GetOneAccount();

            //高佣搜索，先查看是否有高佣商品链接
            string url = "http://pub.alimama.com/common/code/getAuctionCode.json?auctionid=" + itemid + "&adzoneid=" + deweiboaccount.Adzoneid + "&siteid=" + deweiboaccount.Siteid + "&scenes=3&channel=tk_qqhd&t=" + HttpHelper1.GetTicks() + "&_tb_token_=" + tbtoken+"&pvid="+pv_id;
            string result = HttpHelper1.SendDataByGET(url, ref alimamacc);
            if (result.Contains("参数错误"))
            {
                url = "http://pub.alimama.com/common/code/getAuctionCode.json?auctionid=" + itemid + "&adzoneid=" + deweiboaccount.Adzoneid + "&siteid=" + deweiboaccount.Siteid + "&scenes=1&t=" + HttpHelper1.GetTicks() + "&_tb_token_=" + tbtoken + "&pvid=" + pv_id + "&yxjh=-1";
                result = HttpHelper1.SendDataByGET(url, ref alimamacc);
            }
            //正常搜索
            
            string shorturl = "";
            HttpHelper1.GetStringInTwoKeyword(result, ref shorturl, "couponShortLinkUrl\":\"", "\",\"qrCodeUrl", 0);
            HttpHelper1.GetStringInTwoKeyword(result, ref tbkshortlink, "shortLinkUrl\":\"", "\"", 0);

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
            string tbkurl = "";
            if (weiboshorturl.Contains("s.click.taobao.com"))
            {
                tbkurl = weiboshorturl;
                goto buzhou3; }
            //一次跳转，微博短链接 -> 淘宝客短链接
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(weiboshorturl);
            req.AllowAutoRedirect = false;
            WebResponse rsp = req.GetResponse();
            tbkurl = rsp.Headers["location"];
            req.Abort();
            rsp.Close();
            rsp.Dispose();

            
            //二次跳转，淘宝客短链接 -> 淘宝客长链接
            req = (HttpWebRequest)HttpWebRequest.Create(tbkurl);
            if (tbkurl.Contains("uland.taobao.com"))
            {
                //string result1 = HttpHelper1.GetHttpsHtml(tbkurl, "", ref tbrealurl);
                return tbkurl;
            }
            req.AllowAutoRedirect = false;
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
            rsp = req.GetResponse();
            
            tbkurl = rsp.Headers["location"];
            req.Abort();
            rsp.Close();
            rsp.Dispose();
            buzhou3:
            //三次跳转，淘宝客长链接 -> 淘宝Item跳转地址
            req = (HttpWebRequest)HttpWebRequest.Create(tbkurl);
            req.AllowAutoRedirect = false;
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
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
        /// 创建选品库
        /// </summary>
        /// <param name="title">选品库名称</param>
        /// <param name="alimamacc"></param>
        /// <returns>选品库ID</returns>
        public static string CreatXuanpinku(string title, ref CookieContainer alimamacc)
        {
            string result = "";
            string groupId = "";
            string url = "http://pub.alimama.com/favorites/group/save.json";
            string tbtoken = Alimama.GetTbToken(alimamacc);
            string encodetitle = HttpUtility.UrlEncode(title);
            string poststr = "groupTitle="+ encodetitle + "&groupType=1&t="+HttpHelper1.GetTicks()+"&_tb_token_="+tbtoken+"&pvid=";
            string refer = "http://pub.alimama.com/promo/search/index.htm?q=1&t="+HttpHelper1.GetTicks();
            result = HttpHelper1.SendDataByPost(url, poststr, refer, ref alimamacc);
            HttpHelper1.GetStringInTwoKeyword(result, ref groupId, "{\"data\":{\"data\":", "},\"", 0);
            return groupId;
        }

        public static string AddItemToXuanpinku(string groupid, System.Collections.ArrayList itemids, ref CookieContainer alimamacc)
        {
            string result = "";
            string url = "http://pub.alimama.com/favorites/item/batchAdd.json";
            string tbtoken = Alimama.GetTbToken(alimamacc);
            string encodeids = "";
            for (int i = 0; i < itemids.Count; i++)
            {
                if (i + 1 != itemids.Count)
                    encodeids += itemids[i] + "%2C";
                else
                    encodeids += itemids[i];
            }

            string poststr = "groupId="+ groupid + "&itemListStr="+encodeids+"&t="+HttpHelper1.GetTicks()+"&_tb_token_="+tbtoken+"&pvid=10_118.145.0.220_431_1496130793991";
            string refer = "http://pub.alimama.com/promo/search/index.htm?q=1&t=" + HttpHelper1.GetTicks();
            result = HttpHelper1.SendDataByPost(url, poststr, refer, ref alimamacc);
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
