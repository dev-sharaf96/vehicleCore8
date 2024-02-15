using StackExchange.Redis;
using System;

namespace Tameenk.Redis
{
    public class RedisConnectorHelper
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection;

        static RedisConnectorHelper()
        {
            ConfigurationOptions options = new ConfigurationOptions
            {
                EndPoints = { { "10.101.15.18", 6379 } },
                AbortOnConnectFail = false,
                AsyncTimeout = 1000
            };

            RedisConnectorHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(options);
            });
        }

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}
