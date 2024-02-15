using Tameenk.Redis;

namespace Tameenk.Services.Capcha.API
{
    public class CacheManagerConfig
    {
        public static void Initialize()
        {
            _ = RedisCacheManager.Instance;
        }
    }
}