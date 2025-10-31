using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ASP.NETCore5.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { set; get; }
    }
}
