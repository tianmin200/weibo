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
        public static string appkey = "23596105";
        public static string secret = "5cf839a91d5d86a953debf913f2702ec";

        public static CookieContainer Login()
        {
            string posturl = "https://login.taobao.com/member/login.jhtml?redirectURL=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1";
            string poststr = "TPL_username=tianmin200&TPL_password=tianmin3216779236&ncoSig=&ncoSessionid=&ncoToken=78d7c13a4541bc73502f4890604b1044ad4f67b2&slideCodeShow=false&useMobile=false&lang=zh_CN&loginsite=0&newlogin=&TPL_redirect_url=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1&from=tb&fc=default&style=default&css_style=&keyLogin=false&qrLogin=true&newMini=false&newMini2=false&tid=&loginType=3&minititle=&minipara=&pstrong=&sign=&need_sign=&isIgnore=&full_redirect=&sub_jump=&popid=&callback=&guf=&not_duplite_str=&need_user_id=&poy=&gvfdcname=10&gvfdcre=&from_encoding=&sub=&TPL_password_2=&loginASR=1&loginASRSuc=0&allp=&oslanguage=zh-CN&sr=1366*768&osVer=&naviVer=chrome%7C45.02454101&osACN=Mozilla&osAV=5.0+%28Windows+NT+10.0%3B+WOW64%29+AppleWebKit%2F537.36+%28KHTML%2C+like+Gecko%29+Chrome%2F45.0.2454.101+Safari%2F537.36&osPF=Win32&miserHardInfo=&appkey=&bind_login=false&bind_token=&nickLoginLink=&mobileLoginLink=https%3A%2F%2Flogin.taobao.com%2Fmember%2Flogin.jhtml%3FredirectURL%3Dhttp%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1%26useMobile%3Dtrue&showAssistantLink=&um_token=HV01PAAZ0b82763e769100dc58a260300005329a&ua=084%23gYE1sl1MXXs1KLXS11111b4oNIsrbFfsCcfXPFseGl7kCwg9bB15NnMySCW9bFcbrg%2BwApxrG1rhUlXJKrPM2bRQXzg3bZLX1XpgCIQ8yHVSSs0SeiPLWqP0iPqHDelX1XtLr9HNfKtwLb7EB3omoD53idWvqjZy3Q48a1E1BkFGVSo12Ny1YslkbdfdU6y8eOEPem6KU4kS1l1QxbuwlmNI4%2FFEvXxCTN6AxTZtsVztrx8AuvLX11pUgfSwB1E1XynUXl2%2F1l1SCnv8PRzxHwNoJyZIWBXzZTIVJkj%2F1l1SCna8PR8jHwNoJy0Dz7XdWkMGJknn1l1kN1sjqeu%2FtDxBDrmTxC32OIH%2BVr71w5Ob2rkL1l1G1x10yJL1VBkjggt1ZqqYhLE1tbvn11XijWFfd%2FqaeIw9%2B4RtOs%2FmPBjH5dkwTk99nDqcRmuyeIw9%2B4RtOE9KjIhWDTMA3NJzkDA3Un5gD8JeLGFiz6QtVOMBc4RcDEW6KTojDc1irpKeaARRC5zwkq7Q0GMGVmvCkDUTMP5%2BV3xzmmkDY%2BfmPqNViGMGVmvYPgG6DxLK3UsRY0GT07JdQqBZ4hqGDEW6yoNXyLE1h1EXfZdpxxyF1lqEmgyOLjsX1n7f%2B6h1L1EXYoNpxxwQ1lqEmgyOLjs71l1s1lwSEFDqL5%2FS1l1BXG4Jc%2FkttLE1BBJSs1XAVqV61zlh78VNKsWEb%2BB6gBiTQRLX11W1XsD3ogmW1l1EeCnwX71X11Vg7ZcLMLE1B%2FRh9iDdkn%2BHu%2Fpc0FHVeF7L%2Bd5vK7tPHMBqTR1X1XbELNcT21XEYVL%2BtWQmQqEVsokCx1E1NI51oriW1EptjR1X11a4LNcT21XEv62L1l1gA%2F10yJL1V1jiq1E1BLLNtgLwjgRkX3SvsZ9UCTemQauiD1rT38YtB1E1XynUXl2%2F1l1SCnv8Pa1PHwNoJy0Dz7XdWkMGJknW1l1EeCnwXQlX1XtLr9HNfgj2Lb7EB3omoD53idWvqjZy3Q48B1E1XynUXl2S1l1QxbuwlmNIUWFEvXxCTN6AxTZtsVztrx8AuDlX1XtLr9HNfgAeLb7EB3omoD53idWvqjZy3Q48a1E1BkFGVSo1BGk1YslkbdfdU6y8eOEPem6KU4kS1l1Qxbuwlm1%2FmsFEvXxCTN6AxTZtsVztrx8AuDlX1XtLr9HNfF3VLb7EB3omoD53idWvqjZy3Q48a1E1BkFGVSo1joC1YslkbdfdU6y8eOEPem6KU4kS1l1QxbuwlmX%2FJxFEvXxCTN6AxTZtsVztrx8AuDlX1XtLr9HNfqLjLb7EB3omoD53idWvqjZy3Q48a1E1BkFGVSo1tyk1YslkbdfdU6y8eOEPem6KU4kS1l1QxbuwlmN%2FL%2FFEvXxCTN6AxTZtsVztrx8AuvfX1XlBXp9crqRktLZ2FL4z%2FDxCXqbJ3VLX11pUgfSwd1E1XmABPUWeaMLX11ZqYzQ84Fc%3D";
            CookieContainer cc = new CookieContainer();
            string refer = "http://login.taobao.com/member/taobaoke/login.htm/is_login=1";
            string result = HttpHelper1.SendDataByPost(posturl, poststr, refer, ref cc);
            string strurl = "https://www.alimama.com/membersvc/my.htm?domain=taobao&service=user_on_taobao&sign_account=a49f9f62502fc96517a626ea7e685eb4";
            string resultstr = HttpHelper1.SendDataByGET(strurl, ref cc);
            bool isLogin = Alimama.TestLogin(cc);
            return cc;
        }
        public static CookieContainer Login(string username,string password)
        {
            string posturl = "https://login.taobao.com/member/login.jhtml?redirectURL=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1";
            string poststr = "TPL_username="+username+"&TPL_password="+password+"&ncoSig=&ncoSessionid=&ncoToken=8bb84c14acdeac1ea8a25bd93ff9b3e8141bc70e&slideCodeShow=false&useMobile=false&lang=zh_CN&loginsite=0&newlogin=&TPL_redirect_url=http%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1&from=alimama&fc=default&style=mini&css_style=&keyLogin=false&qrLogin=true&newMini=false&newMini2=true&tid=&loginType=3&minititle=&minipara=&pstrong=&sign=&need_sign=&isIgnore=&full_redirect=true&sub_jump=&popid=&callback=&guf=&not_duplite_str=&need_user_id=&poy=&gvfdcname=&gvfdcre=&from_encoding=&sub=false&TPL_password_2=&loginASR=1&loginASRSuc=0&allp=&oslanguage=&sr=&osVer=&naviVer=&osACN=&osAV=&osPF=&miserHardInfo=&appkey=&bind_login=false&bind_token=&nickLoginLink=&mobileLoginLink=https%3A%2F%2Flogin.taobao.com%2Fmember%2Flogin.jhtml%3Fstyle%3Dmini%26newMini2%3Dtrue%26from%3Dalimama%26redirectURL%3Dhttp%3A%2F%2Flogin.taobao.com%2Fmember%2Ftaobaoke%2Flogin.htm%3Fis_login%3D1%26full_redirect%3Dtrue%26disableQuickLogin%3Dtrue%26useMobile%3Dtrue";
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
            string cookiestr = File.ReadAllText(str3 + "/config/alimamacookie.txt");
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
        public static bool IsCampaignExits(CampainsData campains,CookieContainer alimamacc,ref string keeperid)
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

        public static CampainsData GetCampainByTBItemID(string itemid,string tbtoken,string pv_id,CookieContainer alimamacc)
        {
            string url = "http://pub.alimama.com/pubauc/getCommonCampaignByItemId.json?itemId=" + itemid + "&t=" + HttpHelper1.GetTicks() + "&_tb_token_=" + tbtoken + "&pvid=" + pv_id;
            string resulthtml = HttpHelper1.SendDataByGET(url, ref alimamacc);
            CampainsData campains = Newtonsoft.Json.JsonConvert.DeserializeObject<CampainsData>(resulthtml);
            return campains;
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
            if (campaignstrs.StartsWith("null")) return false;//如果没有定向计划，退出
            
            string pv_id = "";
            HttpHelper1.GetStringInTwoKeyword(resulthtml, ref pv_id, "pvid\":\"", "\",\"docsfound", 0);
            string itemid = "";
            HttpHelper1.GetStringInTwoKeyword(resulthtml, ref itemid, "auctionId\":",",\"", 0);
            string keeperid = "";
            
            CookieCollection ccl = alimamacc.GetCookies(new Uri("http://alimama.com"));
            string tbtoken = ccl["_tb_token_"].Value;
            string refer = "http://pub.alimama.com/promo/search/index.htm?queryType=2&q=" + HttpUtility.UrlEncode(itemurl);
            CampainsData campainsdata = Alimama.GetCampainByTBItemID(itemid,tbtoken,pv_id, alimamacc);
            //if (IsCampaignExits(campainsdata, alimamacc,ref keeperid)) return false;//如果申请过定向计划，退出
            double maxCommission = 0;
            double maxManualCommission = 0;
            
            CampainsData.Datum maxCampaign = new CampainsData.Datum();
            CampainsData.Datum maxManualCampaign = new CampainsData.Datum();
            //挑选出自动审核计划中，佣金最高的一个
            foreach (CampainsData.Datum campain in campainsdata.data)
            {
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
            bool isSuc = ApplyCampaign(maxCampaign.CampaignID.ToString(), keeperid, pv_id, refer, alimamacc);//自动审核最高分成比例的计划一定要申请
            if (maxCommission < maxManualCommission)
            {
                ApplyCampaign(maxManualCampaign.CampaignID.ToString(), keeperid, pv_id, refer, alimamacc);//如果人工审核的计划分成比例要更高，则再申请人工审核计划
                if(maxManualCampaign.CampaignName.Contains("QQ"))
                    File.AppendAllText("taobaoke定向计划.txt", "分成比例：" + maxManualCampaign.AvgCommission + " 计划名称：" + maxManualCampaign.CampaignName + "\r\n");
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
            string poststr = "campId="+campaignid+"&keeperid="+ keeperid + "&applyreason=%e5%be%ae%e5%8d%9a%e5%a4%a7%e5%8f%b7%e5%af%bc%e8%b4%ad%ef%bc%8cQQ%ef%bc%9a1145837517&t=" + HttpHelper1.GetTicks()+"&_tb_token_="+ tbtoken + "&pvid="+pv_id;
            
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
