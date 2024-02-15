using Tameenk.Redis;

namespace Tameenk.Services.Checkout.Api
{
    public class CacheManagerConfig
    {
        public static void Initialize()
        {
            _ = RedisCacheManager.Instance;
        }
    }
}