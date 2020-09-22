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
using ApacheIgniteExample.Infrastructure.Ignite;
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
                return IgniteManager.Ignite;
            });
            services.AddSingleton<ICache<int, string>>(q => {
                return IgniteManager.ItemCache;
            });

            services.AddTransient<ICacheStore<Guid, Order>, OrderRepository>();
            services.AddScoped<ICacheStore<int, string>, ItemRepository>();
        }
    }
}
