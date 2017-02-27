using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weibo.Models
{
    public class ForwardResponseData
    {
        public int code { get; set; }
        public string msg { get; set; }
        public object data { get; set; }
    }
}
