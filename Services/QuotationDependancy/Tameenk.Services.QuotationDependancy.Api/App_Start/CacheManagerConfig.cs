using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Redis;

namespace Tameenk.Services.QuotationDependancy.Api
{
    public static class CacheManagerConfig
    {
        public static void Initialize()
        {
            _ = RedisCacheManager.Instance;
        }
    }
}