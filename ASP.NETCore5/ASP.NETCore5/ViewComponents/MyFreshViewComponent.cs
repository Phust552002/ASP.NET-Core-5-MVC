using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NETCore5.ViewComponents
{
    [ViewComponent(Name ="MyFresh")]
    public class MyFreshViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke( int a, int b, string op)
        {
            int res = 0;
            if(op=="add")
            {
                res=a+b;
            }
            else
                if(op == "subtract")
            {
                res = a - b;
            }
            ViewBag.a = a;
            ViewBag.b = b;
            ViewBag.op = op;
            ViewBag.result = res;
            return View();
        }
    }
}