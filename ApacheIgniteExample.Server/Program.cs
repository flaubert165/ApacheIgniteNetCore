using Apache.Ignite.Core;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Apache.Ignite.Core.Events;
using System;

namespace ApacheIgniteExample.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Ignition.Start(new IgniteConfiguration
            {
                DiscoverySpi = new TcpDiscoverySpi
                {
                    IpFinder = new TcpDiscoveryStaticIpFinder
                    {
                        Endpoints = new[] { "127.0.0.1:47500..47599" }
                    },
                    SocketTimeout = TimeSpan.FromSeconds(0.3)
                },

                //CacheConfiguration = new Collection<CacheConfiguration> {
                //    new CacheConfiguration
                //    {
                //        Name = "orders",
                //        CacheStoreFactory = new OrderRepositoryFactory(),
                //        ReadThrough = true,
                //        WriteThrough = true
                //    }
                //},

                IncludedEventTypes = EventType.CacheAll,

                JvmOptions = new[] { "-Xms1024m", "-Xmx1024m" }
            });
        }
    }
}
