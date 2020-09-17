using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Store;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Apache.Ignite.Core.Events;
using Apache.Ignite.Core.Log;
using ApacheIgniteExample.Domain;
using ApacheIgniteExample.Infrastructure.Datacontext;
using ApacheIgniteExample.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;

namespace ApacheIgniteExample.IoC
{
    public static class IoCManager
    {
        public static void Inject(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IOracleDataContext, OracleDataContext>();

            services.AddSingleton<IIgnite>(q => {
                return Ignition.Start(new IgniteConfiguration
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
                        Name = "items",
                        CacheStoreFactory = new ItemRepositoryFactory(),
                        ReadThrough = true,
                        WriteThrough = true
                    }
                },

                    IncludedEventTypes = EventType.CacheAll,

                    JvmOptions = new[] { "-Xms1024m", "-Xmx1024m" }
                });
            });
            services.AddSingleton<ICache<int, string>>(q => {
                var ignite = q.GetService<IIgnite>();

                return ignite.GetOrCreateCache<int, string>("item");
            });

            //services.AddSingleton(Ignition.Start(new IgniteConfiguration
            //{
            //    DiscoverySpi = new TcpDiscoverySpi
            //    {
            //        IpFinder = new TcpDiscoveryStaticIpFinder
            //        {
            //            Endpoints = new[] { "127.0.0.1:47500..47509" }
            //        },
            //        SocketTimeout = TimeSpan.FromSeconds(0.3)
            //    },

            //    CacheConfiguration = new Collection<CacheConfiguration> {
            //        new CacheConfiguration
            //        {
            //            Name = "orders",
            //            CacheStoreFactory = new OrderRepositoryFactory(),
            //            ReadThrough = true,
            //            WriteThrough = true,
            //            KeepBinaryInStore = false,
            //            AtomicityMode = CacheAtomicityMode.Atomic
            //        }
            //    },

            //    IncludedEventTypes = EventType.CacheAll,

            //    JvmOptions = new[] { "-Xms1024m", "-Xmx1024m" }
            //}));

            services.AddTransient<ICacheStore<Guid, Order>, OrderRepository>();
            services.AddScoped<ICacheStore<int, string>, ItemRepository>();
        }
    }
}
