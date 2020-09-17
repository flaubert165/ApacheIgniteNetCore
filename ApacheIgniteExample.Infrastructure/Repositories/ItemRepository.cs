using Apache.Ignite.Core.Cache.Store;
using Apache.Ignite.Core.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace ApacheIgniteExample.Infrastructure.Repositories
{
    public class ItemRepository : ICacheStore<int, string>
    {
        private static Dictionary<int, string> _itemCollection = new Dictionary<int, string>();

        private Dictionary<string, int> _counterDictionary = new Dictionary<string, int>();
        private ILogger<ItemRepository> _logger;


        private void WriteCounter([CallerMemberName]string key = null)
        {
            if (!_counterDictionary.ContainsKey(key))
                _counterDictionary.Add(key, 0);

            var counter = _counterDictionary[key]++;
            _counterDictionary[key] = counter;

            Console.WriteLine($"\t\t{DateTime.UtcNow.ToString("yyyy-MM-dd")} - COUNTER({key}) --> {counter}");
        }

        public ItemRepository()
        {

        }

        public void Delete(int key)
        {
            WriteCounter();
            _itemCollection.Remove(key);
        }

        public void DeleteAll(IEnumerable<int> keys)
        {
            WriteCounter();
            foreach (var key in keys)
            {
                Delete(key);
            }
        }

        public string Load(int key)
        {
            WriteCounter();
            if (_itemCollection.ContainsKey(key))
                return _itemCollection[key];

            return string.Empty;
        }

        public IEnumerable<KeyValuePair<int, string>> LoadAll(IEnumerable<int> keys)
        {
            WriteCounter();
            return _itemCollection;
        }

        public void LoadCache(Action<int, string> act, params object[] args)
        {
            WriteCounter();
            return;
        }

        public void SessionEnd(bool commit)
        {
            WriteCounter();
            return;
        }

        public void Write(int key, string val)
        {
            WriteCounter();
            _itemCollection.Add(key, val);
        }

        public void WriteAll(IEnumerable<KeyValuePair<int, string>> entries)
        {
            WriteCounter();
            foreach (var entry in entries)
            {
                Write(entry.Key, entry.Value);
            }
        }

    }

    public class ItemRepositoryFactory : IFactory<ICacheStore>
    {
        public ItemRepositoryFactory()
        {
        }

        public ICacheStore CreateInstance()
        {
            return new ItemRepository();
        }
    }
}
