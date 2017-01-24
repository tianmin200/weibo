using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQSpace
{
    public class QQHao
    {
        private string qqhaoma;
        private string password;
        private string g_tk;
        private string token;

        public string Qqhaoma
        {
            get
            {
                return qqhaoma;
            }

            set
            {
                qqhaoma = value;
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

        public string G_tk
        {
            get
            {
                return g_tk;
            }

            set
            {
                g_tk = value;
            }
        }

        public string Token
        {
            get
            {
                return token;
            }

            set
            {
                token = value;
            }
        }
    }
}
