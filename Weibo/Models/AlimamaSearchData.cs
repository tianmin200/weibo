﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Weibo.Models
{

    public partial class AlimamaSearchData
    {
        public class Head2
        {
            public string version { get; set; }
            public string status { get; set; }
            public int pageSize { get; set; }
            public int pageNo { get; set; }
            public object searchUrl { get; set; }
            public string pvid { get; set; }
            public int docsfound { get; set; }
            public object errmsg { get; set; }
            public object fromcache { get; set; }
            public int processtime { get; set; }
            public int ha3time { get; set; }
            public int docsreturn { get; set; }
            public object responseTxt { get; set; }
        }
    }

    public partial class AlimamaSearchData
    {
        public class Condition2
        {
            public object userType { get; set; }
            public int queryType { get; set; }
            public object sortType { get; set; }
            public object loc { get; set; }
            public object includeDxjh { get; set; }
            public object auctionTag { get; set; }
            public object freeShipment { get; set; }
            public object startTkRate { get; set; }
            public object endTkRate { get; set; }
            public object startTkTotalSales { get; set; }
            public object startPrice { get; set; }
            public object endPrice { get; set; }
            public object startRatesum { get; set; }
            public object endRatesum { get; set; }
            public object startQuantity { get; set; }
            public object startBiz30day { get; set; }
            public object startPayUv30 { get; set; }
            public object hPayRate30 { get; set; }
            public object hGoodRate { get; set; }
            public object jhs { get; set; }
            public object startDsr { get; set; }
            public object lRfdRate { get; set; }
            public object startSpay30 { get; set; }
            public object hSellerGoodrat { get; set; }
            public object hSpayRate30 { get; set; }
            public object hasUmpBonus { get; set; }
            public object isBizActivity { get; set; }
            public object subOeRule { get; set; }
            public object startRlRate { get; set; }
            public object shopTag { get; set; }
            public object npxType { get; set; }
            public object picQuality { get; set; }
            public object selectedNavigator { get; set; }
            public object typeTagName { get; set; }
        }
    }

    public partial class AlimamaSearchData
    {
        public class Paginator2
        {
            public int length { get; set; }
            public int offset { get; set; }
            public int page { get; set; }
            public int beginIndex { get; set; }
            public int endIndex { get; set; }
            public int items { get; set; }
            public int lastPage { get; set; }
            public int itemsPerPage { get; set; }
            public int previousPage { get; set; }
            public int nextPage { get; set; }
            public int pages { get; set; }
            public int firstPage { get; set; }
            public int[] slider { get; set; }
        }
    }

   

    public partial class AlimamaSearchData
    {
        public class PageList2
        {
            public string tkSpecialCampaignIdRateMap { get; set; }
            public int eventCreatorId { get; set; }
            public int rootCatId { get; set; }
            public int leafCatId { get; set; }
            public object debugInfo { get; set; }
            public int rootCatScore { get; set; }
            public long sellerId { get; set; }
            public int userType { get; set; }
            public string shopTitle { get; set; }
            public string pictUrl { get; set; }
            public string title { get; set; }
            public long auctionId { get; set; }
            public object couponActivityId { get; set; }
            public double reservePrice { get; set; }
            public int dayLeft { get; set; }
            public object tk3rdRate { get; set; }
            public double zkPrice { get; set; }
            public double tkRate { get; set; }
            public double tkCommFee { get; set; }
            public string nick { get; set; }
            public int biz30day { get; set; }
            public double rlRate { get; set; }
            public int totalNum { get; set; }
            public double totalFee { get; set; }
            public string sameItemPid { get; set; }
            public int hasRecommended { get; set; }
            public int hasSame { get; set; }
            public int couponTotalCount { get; set; }
            public int couponLeftCount { get; set; }
            public string auctionUrl { get; set; }
            public string couponLink { get; set; }
            public string couponLinkTaoToken { get; set; }
            public int includeDxjh { get; set; }
            public string auctionTag { get; set; }
            public int couponAmount { get; set; }
            public object eventRate { get; set; }
            public object couponShortLink { get; set; }
            public string couponInfo { get; set; }
            public int couponStartFee { get; set; }
            public string couponEffectiveStartTime { get; set; }
            public string couponEffectiveEndTime { get; set; }
            public object hasUmpBonus { get; set; }
            public object isBizActivity { get; set; }
            public object umpBonus { get; set; }
            public object rootCategoryName { get; set; }
            public object couponOriLink { get; set; }
            public object userTypeName { get; set; }
        }
    }

    public partial class AlimamaSearchData
    {
        public class Data2
        {
            public Head2 head { get; set; }
            public Condition2 condition { get; set; }
            public Paginator2 paginator { get; set; }
            public PageList2[] pageList { get; set; }
            public object navigator { get; set; }
            public object extraInfo { get; set; }
        }
    }

    public partial class AlimamaSearchData
    {
        public class Info2
        {
            public object message { get; set; }
            public string pvid { get; set; }
            public bool ok { get; set; }
        }
    }

    public partial class AlimamaSearchData
    {
        public Data2 data { get; set; }
        public Info2 info { get; set; }
        public bool ok { get; set; }
        public object invalidKey { get; set; }
    }

}
