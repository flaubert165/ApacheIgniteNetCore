using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Apache.Ignite.Core.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ApacheIgniteExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IgniteInitialize();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void IgniteInitialize()
        {
            Ignition.StartClient(new Apache.Ignite.Core.Client.IgniteClientConfiguration("127.0.0.1:47500"));
            //Ignition.Start(new IgniteConfiguration
            //{
            //    DiscoverySpi = new TcpDiscoverySpi
            //    {
            //        IpFinder = new TcpDiscoveryStaticIpFinder
            //        {
            //            Endpoints = new[] { "127.0.0.1:47500" }
            //        },
            //        SocketTimeout = TimeSpan.FromSeconds(0.3),
            //    },

            //    //CacheConfiguration = new Collection<CacheConfiguration> {
            //    //    new CacheConfiguration
            //    //    {
            //    //        Name = "orders",
            //    //        CacheStoreFactory = new OrderRepositoryFactory(),
            //    //        ReadThrough = true,
            //    //        WriteThrough = true
            //    //    }
            //    //},

                

            //    IncludedEventTypes = EventType.CacheAll,

            //    JvmOptions = new[] { "-Xms1024m", "-Xmx1024m" }
            //});
        }
    }
}
