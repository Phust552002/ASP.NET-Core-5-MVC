using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NETCore5.Models
{
    public class CaptchaResponse
    {
        public bool success { set; get; }
        public DateTime challenge_ts { set; get; }
        public string hostname { get; set; }
    }
}
