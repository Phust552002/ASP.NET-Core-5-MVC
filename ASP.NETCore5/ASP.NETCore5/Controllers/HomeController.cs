using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using ASP.NETCore5.Models;

namespace ASP.NETCore5.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
        public IActionResult SessionView()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SessionView(int a, int b, string c)
        {
            HttpContext.Session.SetInt32("a", a);
            HttpContext.Session.SetInt32("b", b);
            HttpContext.Session.SetInt32("ab", a + b);
            HttpContext.Session.SetString("c", c);

            TempData["a"] = a;
            TempData["b"] = b;
            TempData["ab"] = a + b;
            TempData["c"] = c;

            return RedirectToAction("ShowSessionView");
        }
        public IActionResult ShowSessionView()
        {
            ViewBag.a = HttpContext.Session.GetInt32("a");
            ViewBag.b = HttpContext.Session.GetInt32("b");
            ViewBag.an = HttpContext.Session.GetInt32("ab");
            ViewBag.c = HttpContext.Session.GetString("c");
            ViewBag.ta = TempData["a"];
            ViewBag.tb = TempData["b"];
            ViewBag.tab = TempData["ab"];
            ViewBag.tc = TempData["c"];

            return View();
        }

        public IActionResult ShowSessionView2()
        {
            ViewBag.a = HttpContext.Session.GetInt32("a");
            ViewBag.b = HttpContext.Session.GetInt32("b");
            ViewBag.an = HttpContext.Session.GetInt32("ab");
            ViewBag.c = HttpContext.Session.GetString("c");
            ViewBag.ta = TempData["a"];
            ViewBag.tb = TempData["b"];
            ViewBag.tab = TempData["ab"];
            ViewBag.tc = TempData["c"];

            return View();
        }

        }
    }
