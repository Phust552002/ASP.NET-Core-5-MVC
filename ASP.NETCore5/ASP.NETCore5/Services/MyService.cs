using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NETCore5.Services
{
    public class MyService: IMyService
    {
        public MyService() { }
        public int Calculate(int a, int b)
        {
            return a * b + 10 * b;
        }
    }
}
