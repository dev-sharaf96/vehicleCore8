using Tameenk.Redis;

namespace Tameenk.Services.IdentityApi.App_Start
{
    public static class CacheManagerConfig
    {
        public static void Initialize()
        {
            _ = RedisCacheManager.Instance;
        }
    }
}