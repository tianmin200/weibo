using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weibo.Models
{
    public class DEColTask
    {
        private string id;
        private string name;
        private string pages;
        private string type;
        private string interval;
        private string[] weiboaccountids;
        private DEWeiboAccount[] weiboaccounts;
        private string savePath;

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

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Pages
        {
            get
            {
                return pages;
            }

            set
            {
                pages = value;
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

        public DEWeiboAccount[] Weiboaccounts
        {
            get
            {
                return weiboaccounts;
            }

            set
            {
                weiboaccounts = value;
            }
        }

        public string[] Weiboaccountids
        {
            get
            {
                return weiboaccountids;
            }

            set
            {
                weiboaccountids = value;
            }
        }

        public string SavePath
        {
            get
            {
                return savePath;
            }

            set
            {
                savePath = value;
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
    }
}
