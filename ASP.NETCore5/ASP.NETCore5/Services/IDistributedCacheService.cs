using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NETCore5.Services
{
    interface IDistributedCacheService
    {
        T GetCache<T>(string key) where T : class;
        void SetCache<T>(string key, T values);
    }
}
