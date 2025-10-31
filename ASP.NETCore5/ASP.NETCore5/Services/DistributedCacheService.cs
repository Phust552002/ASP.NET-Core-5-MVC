using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;

namespace ASP.NETCore5.Services
{
    public class DistributedCacheService : IDistributedCacheService
    {
        // Khai báo private field để lưu trữ đối tượng cache
        public IDistributedCache _cache;

        // Constructor để Dependency Injection (DI)
        public DistributedCacheService(IDistributedCache _cache)
        {
            this._cache = _cache;
        }

        // Phương thức generic để lấy dữ liệu từ cache
        // T là kiểu dữ liệu mong muốn, phải là kiểu reference (class)
        public T GetCache<T>(string key) where T : class
        {
            try
            {
                byte[] values = _cache.Get(key);
                if (values == null) return null;
                T result = JsonSerializer.Deserialize<T>(values);

                return result;
            }
            catch (Exception) { }
            return null;
        }
        public void SetCache<T>(string key, T values)
        {
            try
            {
                var bytes = JsonSerializer.SerializeToUtf8Bytes(values);

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                    SlidingExpiration = TimeSpan.FromSeconds(35) 
                };

                _cache.Set(key, bytes, cacheOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SetCache error: {ex.Message}");
            }
        }


    }
}
