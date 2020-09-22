using Apache.Ignite.Core.Cache.Event;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ApacheIgniteExample.Infrastructure.Listeners
{
    class ItemListener<TKey, TValue> : ICacheEntryEventListener<TKey, TValue>
    {
        public readonly SemaphoreSlim HasData = new SemaphoreSlim(0, 1);

        public readonly ConcurrentQueue<ICacheEntryEvent<TKey, TValue>> Events
            = new ConcurrentQueue<ICacheEntryEvent<TKey, TValue>>();

        public void OnEvent(IEnumerable<ICacheEntryEvent<TKey, TValue>> evts)
        {
            foreach (var entryEvent in evts)
                Events.Enqueue(entryEvent);

            HasData.Release();
        }
    }
}
