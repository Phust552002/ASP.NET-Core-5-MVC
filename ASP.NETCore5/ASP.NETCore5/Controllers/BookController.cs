using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETCore5.Services;
using ASP.NETCore5.Models;

namespace ASP.NETCore5.Controllers
{
    // http://<server>/book/index
    public class BookController : Controller
    {
        private IMyService _service;
        public BookController(IMyService service)
        {
            _service = service;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ShowBooks()
        {
            return View();
        }
        public IActionResult Demo()
        {
            return View("Index");
        }
        public IActionResult PartialDemo()
        {
            return View();
        }
        public IActionResult ShowView()
        {
            ViewBag.BookTitle = "Practical ASP.NET CORE 5";
            ViewBag.Total = 100;
            ViewData["BookTitle"] = "Essential ASP.NET CORE 5";
            ViewData["Total"] = 150;
            return View();
        }
        public IActionResult ShowDetail()
        {
            Book o = new Book();
            o.Id = 1;
            o.Title = "Essential ASP.NET CORE 5";
            o.Author = "MrPhuc";
            o.Email = "smaple@gmail.com";
            o.BookCategory = "programming";
            o.Description = " Heheheheheh";
            return View(o);
        }

        public IActionResult ViewComponentDemo()
        {
            return View();
        }
        public IActionResult ViewAll()
        {
            return View();
        }

        public IActionResult RoutingDemo()
        {
            return View();
        }
        public IActionResult BookDetail([FromRoute] int id)
        {
            Book o = new Book();
            o.Id = id;
            o.Title = string.Format("Book title {0}", id);
            o.Author = string.Format("Author {0}", id);
            o.Email = string.Format("dotnet{0}@email.com", id);
            o.BookCategory = "Programming";
            o.Description = string.Format("Description for book title {0}", id);

            return View("Detail", o);
        }

        // Action redirect sang trang chi tiết
        public IActionResult ShowBook(int id)
        {
            return RedirectToAction("BookDetail", new { id = id });
        }

        public IActionResult DIDemo([FromQuery] int a, [FromQuery] int b)
        {
            int result = this._service.Calculate(a, b);
            ViewBag.Result = string.Format("Input a={0}, b={1}. Result is {2}", a, b, result);

            return View();
        }

        public IActionResult ErrorHandling()
        {
            ViewBag.Result = string.Empty;
            return View();
        }

        [HttpPost]
        public IActionResult ErrorHandling(string val)
        {
            try
            {
                int total = Convert.ToInt32(val) * 10;
                ViewBag.Result = total.ToString();
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }

    }
}
