using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ASP.NETCore5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ASP.NETCore5.Controllers
{
    [Authorize(Roles = "Manager")]

    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
