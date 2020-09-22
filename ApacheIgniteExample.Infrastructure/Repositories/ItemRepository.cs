using Apache.Ignite.Core.Cache.Store;
using Apache.Ignite.Core.Common;
using ApacheIgniteExample.Infrastructure.Datacontext;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ApacheIgniteExample.Infrastructure.Repositories
{
    /*
create table clear.ItemIgnite
(
    Id int not null primary key
    ,Description varchar2(500) not null
)
     */
    public class ItemRepository : ICacheStore<int, string>
    {
        private static Dictionary<int, string> _itemCollection = new Dictionary<int, string>();

        private Dictionary<string, int> _counterDictionary = new Dictionary<string, int>();
        private ILogger<ItemRepository> _logger;

        private void WriteCounter([CallerMemberName]string key = null)
        {
            if (!_counterDictionary.ContainsKey(key))
                _counterDictionary.Add(key, 0);

            var counter = _counterDictionary[key];
            counter++;
            _counterDictionary[key] = counter;

            Console.WriteLine($"\t\t{DateTime.UtcNow.ToString("yyyy-MM-dd")} - COUNTER({key}) --> {counter}");
        }

        public ItemRepository()
        {

        }

        private OracleConnection GetNewConnection()
        {
            return new OracleConnection("User Id=clear;Password=clear;Data Source=localhost:1521/xe");
        }

        public void Delete(int key)
        {
            WriteCounter();
            //_itemCollection.Remove(key);
            GetNewConnection().Execute($"delete clear.ItemIgnite where Id = {key}");
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
            //if (_itemCollection.ContainsKey(key))
            //    return _itemCollection[key];

            //return string.Empty;
            return GetNewConnection().Query<string>($"select Description from clear.ItemIgnite where Id = {key}")
                .FirstOrDefault();
        }

        public IEnumerable<KeyValuePair<int, string>> LoadAll(IEnumerable<int> keys)
        {
            WriteCounter();
            //return _itemCollection;

            var returnCollection = new Dictionary<int, string>();

            var queryResult = GetNewConnection().Query<(int id, string description)>(
                $"select Id as Key, Description as Value from clear.ItemIgnite"
            );

            foreach (var (id, description) in queryResult)
                returnCollection.Add(id, description);

            return returnCollection;
        }

        public void LoadCache(Action<int, string> act, params object[] args)
        {
            WriteCounter();

            var queryResult = GetNewConnection().Query<(int id, string description)>(
                $"select Id, Description from clear.ItemIgnite"
            );

            foreach (var (id, description) in queryResult)
                act(id, description);
        }

        public void SessionEnd(bool commit)
        {
            //WriteCounter();
            return;
        }

        public void Write(int key, string val)
        {
            
            WriteCounter();
            //_itemCollection.Add(key, val);

            GetNewConnection().Execute($"insert into clear.ItemIgnite(Id, Description) values ({key}, '{val}')");
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
