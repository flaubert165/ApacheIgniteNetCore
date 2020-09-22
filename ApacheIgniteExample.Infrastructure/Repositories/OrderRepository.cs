
using Apache.Ignite.Core.Cache.Store;
using Apache.Ignite.Core.Common;
using ApacheIgniteExample.Domain;
using ApacheIgniteExample.Infrastructure.Datacontext;
using System;
using System.Collections.Generic;
using Dapper;
using System.Linq;

namespace ApacheIgniteExample.Infrastructure.Repositories
{
    public class OrderRepository : ICacheStore<Guid, Order>
    {
        private static List<Order> _orders = new List<Order>();

        private IOracleDataContext _context;

        public OrderRepository(IOracleDataContext context)
        {
            _context = context;
        }

        public void LoadCache(Action<Guid, Order> act, params object[] args)
        {
            return;

            var query = "SELECT * FROM Orders";

            using (var ctx = _context.GetConnection())
            {
                ctx.Query<Order>(query).AsList().ForEach(item => {
                    act(item.Id, item);
                });
            }
        }

        public void Delete(Guid key)
        {
            _orders.Remove(_orders.FirstOrDefault(q => q.Id == key));
        }

        public void DeleteAll(IEnumerable<Guid> keys)
        {
            foreach (var key in keys)
                Delete(key);
        }

        public Order Load(Guid key)
        {
            return _orders.FirstOrDefault(q => q.Id == key);

            var query = "SELECT * FROM Orders WHERE Id = @id";

            using (var ctx = _context.GetConnection())
            {
                return ctx.QueryFirstOrDefault<Order>(query, new 
                {
                    id = key
                });
            }
        }

        public IEnumerable<KeyValuePair<Guid, Order>> LoadAll(IEnumerable<Guid> keys)
        {

            var query = "SELECT * FROM Orders WHERE Id IN (@)";

            throw new NotImplementedException();
        }

        public void Write(Guid key, Order val)
        {
            return; 
            var query = "INSERT INTO Orders (Id, UserId, Symbol, Quantity, Side, Price, Type, Status, TriggerPrice, ExpiresAt, CreatedAt)" +
                        "VALUES (@id, @userId, @symbol, @quantity, @side, @price, @type, @status, @triggerPrice, @expiresAt, @createdAt)";

            using (var ctx = _context.GetConnection())
            {
                    ctx.Execute(query, new
                    {
                        id = key,
                        userId = val.UserId,
                        symbol = val.Symbol,
                        quantity = val.Quantity,
                        side = val.Side,
                        price = val.Price,
                        type = val.Type,
                        status = val.Status,
                        triggerPrice = val.TriggerPrice,
                        expiresAt = val.ExpiresAt,
                        createAt = val.CreatedAt
                    });
            }
        }

        public void WriteAll(IEnumerable<KeyValuePair<Guid, Order>> entries)
        {
            return;

            var query = "INSERT INTO Orders (Id, UserId, Symbol, Quantity, Side, Price, Type, Status, TriggerPrice, ExpiresAt, CreatedAt )" +
                        "VALUES (@id, @userId, @symbol, @quantity, @side, @price, @type, @status, @triggerPrice, @expiresAt, @createdAt)";

            using (var ctx = _context.GetConnection())
            {
                foreach (var entry in entries)
                    ctx.Execute(query, new 
                    { 
                        id = entry.Key, 
                        userId = entry.Value.UserId,
                        symbol = entry.Value.Symbol,
                        quantity = entry.Value.Quantity,
                        side = entry.Value.Side,
                        price = entry.Value.Price,
                        type = entry.Value.Type,
                        status = entry.Value.Status,
                        triggerPrice = entry.Value.TriggerPrice,
                        expiresAt = entry.Value.ExpiresAt,
                        createAt = entry.Value.CreatedAt
                    });
            }
        }

        public void SessionEnd(bool commit)
        {
            throw new NotImplementedException();
        }
    }

    public class OrderRepositoryFactory : IFactory<ICacheStore>
    {
        private IOracleDataContext _context;

        public ICacheStore CreateInstance()
        {
            return new OrderRepository(_context);
        }
    }
}
