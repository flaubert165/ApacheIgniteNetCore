using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Apache.Ignite.Core.Events;
using ApacheIgniteExample.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ApacheIgniteExample.Infrastructure.Ignite
{
    public class IgniteManager
    {
        private static IIgnite _ignite;

        public static IIgnite Ignite
        {
            get
            {
                if (_ignite == null)
                {
                    _ignite = Ignition.Start(new IgniteConfiguration
                    {
                        DiscoverySpi = new TcpDiscoverySpi
                        {
                            IpFinder = new TcpDiscoveryStaticIpFinder
                            {
                                Endpoints = new[] { "127.0.0.1:47500..47509" }
                            },
                            SocketTimeout = TimeSpan.FromSeconds(0.3),
                        },

                        CacheConfiguration = new Collection<CacheConfiguration> {
                            new CacheConfiguration
                            {
                                Name = "ItemIgnite",
                                CacheStoreFactory = new ItemRepositoryFactory(),
                                ReadThrough = true,
                                WriteThrough = true
                            }
                        },

                        IncludedEventTypes = EventType.CacheAll,

                        JvmOptions = new[] { "-Xms1024m", "-Xmx1024m" }
                    });
                }

                return _ignite;
            }
        }
        public static ICache<int, string> ItemCache
        {
            get
            {
                var cache = Ignite.GetOrCreateCache<int, string>("ItemIgnite");

                if (!cache.Any())
                    cache.LoadCache(null);

                return cache;
            }
        }

        public static void Initialize()
        {
            _ = Ignite;
            _ = ItemCache;
        }
    }
}
