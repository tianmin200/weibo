using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weibo.Models
{
    public class DEPubTask
    {
        private string id;
        private DEWeiboAccount[] weiboAccounts;
        private string type;
        private string[] weiboAccountIds;
        private string interval;
        private string delay;

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public DEWeiboAccount[] WeiboAccounts
        {
            get
            {
                return weiboAccounts;
            }

            set
            {
                weiboAccounts = value;
            }
        }

        public string Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        public string[] WeiboAccountIds
        {
            get
            {
                return weiboAccountIds;
            }

            set
            {
                weiboAccountIds = value;
            }
        }

        public string Interval
        {
            get
            {
                return interval;
            }

            set
            {
                interval = value;
            }
        }

        public string Delay
        {
            get
            {
                return delay;
            }

            set
            {
                delay = value;
            }
        }
    }
}
