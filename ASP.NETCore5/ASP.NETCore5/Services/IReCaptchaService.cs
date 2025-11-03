using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETCore5.Settings;

namespace ASP.NETCore5.Services
{
    public interface IReCaptchaService
    {
        ReCaptchaSettings Configs { get; }
        bool ValidateReCaptcha(string token);
    }
}
