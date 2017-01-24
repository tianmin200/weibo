using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Weibo.Models
{
    public class DEWeiboAccount
    {
        private string userid;
        private string username;
        private string password;
        private string nickname;
        private CookieContainer cookie;
        private bool islogin;
        private string st;
        private string siteid;
        private string adzoneid;

        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
            }
        }

        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
            }
        }

        public string Nickname
        {
            get
            {
                return nickname;
            }

            set
            {
                nickname = value;
            }
        }

        public CookieContainer Cookie
        {
            get
            {
                return cookie;
            }

            set
            {
                cookie = value;
            }
        }

        public bool Islogin
        {
            get
            {
                return islogin;
            }

            set
            {
                islogin = value;
            }
        }

        public string St
        {
            get
            {
                return st;
            }

            set
            {
                st = value;
            }
        }

        public string Userid
        {
            get
            {
                return userid;
            }

            set
            {
                userid = value;
            }
        }

        public string Siteid
        {
            get
            {
                return siteid;
            }

            set
            {
                siteid = value;
            }
        }

        public string Adzoneid
        {
            get
            {
                return adzoneid;
            }

            set
            {
                adzoneid = value;
            }
        }
    }
}
