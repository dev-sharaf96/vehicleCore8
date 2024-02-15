using Tameenk.Redis;

namespace Tameenk.Services.InquiryGateway
{
    public class CacheManagerConfig
    {
        public static void Initialize()
        {
            _ = RedisCacheManager.Instance;
        }
    }
}