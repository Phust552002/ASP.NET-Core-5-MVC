using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETCore5.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NETCore5.ViewComponents
{
    [ViewComponent(Name ="MySimple")]
    public class MySimpleViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke( int total, string title)
        {
            var items = GetItems(total);
            ViewBag.CompTitle = title;
            return View(items);
        }
        private List<Book> GetItems(int total)
        {
            var list = new List<Book>();
            for (int i =0; i< total; i++)
            {
                Book o = new Book();
                o.Author = string.Format("Autjor {0}", i + 1);
                o.Title = string.Format("Title {0}", Guid.NewGuid().ToString());
                list.Add(o);
            }
            return list;
        }
    }
}