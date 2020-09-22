using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query.Continuous;
using Apache.Ignite.Core.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Apache.Ignite.Core.Events;
using ApacheIgniteExample.Infrastructure.Listeners;
using ApacheIgniteExample.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApacheIgniteExample.Infrastructure.Ignite
{
    public class IgniteManager
    {
        private static IIgnite _ignite;

        private static ItemListener<int, string> _itemListener = new ItemListener<int, string>();
        private static IContinuousQueryHandle _itemQueryHandler;
        private static ContinuousQuery<int, string> _continuousQuery;

        public static IIgnite Ignite
        {
            get
            {
                if (_ignite == null)
                {
                    var workDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "igniteStorage");
                    if (!Directory.Exists(workDirectory))
                        Directory.CreateDirectory(workDirectory);

                    _ignite = Ignition.Start(new IgniteConfiguration
                    {

                        DiscoverySpi = new TcpDiscoverySpi
                        {
                            IpFinder = new TcpDiscoveryStaticIpFinder
                            {
                                Endpoints = new[] { "127.0.0.1:47500..47509" }
                            },
                            SocketTimeout = TimeSpan.FromSeconds(15),
                        },

                        CacheConfiguration = new Collection<CacheConfiguration> {
                            new CacheConfiguration
                            {
                                Name = "ItemIgnite",
                                CacheStoreFactory = new ItemRepositoryFactory(),
                                ReadThrough = true,
                                WriteThrough = true,
                                WriteBehindEnabled = true,
                                WriteBehindFlushFrequency = TimeSpan.FromMinutes(1)
                            }
                        },

                        IsActiveOnStart = true,
                        WorkDirectory = workDirectory,
                        DataStorageConfiguration = new DataStorageConfiguration
                        {
                            DataRegionConfigurations = new Collection<DataRegionConfiguration>
                            {
                                new DataRegionConfiguration()
                                {
                                    PersistenceEnabled = true,
                                    Name = "DefaultDataRegion"
                                }
                            },
                            WalMode = WalMode.Fsync,
                            WalFlushFrequency = TimeSpan.FromMinutes(1)
                        },

                        IncludedEventTypes = EventType.CacheAll,

                        JvmOptions = new[] { "-Xms1024m", "-Xmx1024m" }
                    });

                    _ignite.SetActive(true);
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

        private static async IAsyncEnumerable<ICacheEntry<int, string>> GetItemContinuousQuery()
        {
            var queryListener = new ItemListener<int, string>();
            var continuousQuery = new ContinuousQuery<int, string>(queryListener);

            using (ItemCache.QueryContinuous(continuousQuery))
            {
                while (true)
                {
                    while (queryListener.Events.TryDequeue(out var entryEvent))
                        yield return entryEvent;

                    await queryListener.HasData.WaitAsync();
                }
            }
        }

        public static void Initialize()
        {
            _ = Ignite;
            _ = ItemCache;

            _ = Task.Run(async () => {
                while (true)
                {
                    await foreach (var entry in GetItemContinuousQuery())
                    {
                        Console.WriteLine($"\n\n{new string('=', 50)} {entry.Value}\n\n");
                        break;
                    } 
                }
            });
            
        }
    }
}
