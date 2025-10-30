using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETCore5.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ASP.NETCore5.Controllers
{
    public class ProductController : Controller
    {
        private IWebHostEnvironment _environment;
        public ProductController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
            public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create([FromForm] Product model)
        {
            if(ModelState.IsValid)
            {
                return View("Details", model);
            }
            return View();
        }
        public IActionResult CreateProduct()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] Product obj, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (files.Count > 0)
                {
                    var formFile = files[0];
                    var filePath = _environment.WebRootPath + "/img/" + formFile.FileName;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    ViewBag.ImageUrl = "/img/" + formFile.FileName;
                }

                return View("ImageDetail", obj);
            }

            return View();
        }

    }
}
