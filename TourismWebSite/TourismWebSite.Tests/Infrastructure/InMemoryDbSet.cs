// create file: TourismWebSite.Tests/Infrastructure/InMemoryDbSet.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace TourismWebSite.Tests.Infrastructure
{
    public class InMemoryDbSet<T> : DbSet<T>, IQueryable, IEnumerable<T> where T : class
    {
        private readonly List<T> _data;
        private readonly IQueryable _query;

        public InMemoryDbSet() : this(Enumerable.Empty<T>()) { }
        public InMemoryDbSet(IEnumerable<T> seed)
        {
            _data = new List<T>(seed);
            _query = _data.AsQueryable();
        }

        public override T Add(T item) { _data.Add(item); return item; }
        public override IEnumerable<T> AddRange(IEnumerable<T> items) { foreach (var i in items) Add(i); return items; }
        public override T Remove(T item) { _data.Remove(item); return item; }
        public override IEnumerable<T> RemoveRange(IEnumerable<T> items) { foreach (var i in items) Remove(i); return items; }
        public override T Attach(T item) { _data.Add(item); return item; }
        public override T Create() => Activator.CreateInstance<T>();
        public override ObservableCollection<T> Local => new ObservableCollection<T>(_data);

        public override T Find(params object[] keyValues)
        {
            if (keyValues == null || keyValues.Length != 1) return null;
            var key = keyValues[0];
            var type = typeof(T);
            var keyNames = new[] { "Id", type.Name + "Id", "BookingId", "TourId" };
            foreach (var name in keyNames)
            {
                var prop = type.GetProperty(name);
                if (prop == null) continue;
                var match = _data.FirstOrDefault(e => prop.GetValue(e)?.Equals(key) == true);
                if (match != null) return match;
            }
            return null;
        }

        Type IQueryable.ElementType => _query.ElementType;
        Expression IQueryable.Expression => _query.Expression;
        IQueryProvider IQueryable.Provider => _query.Provider;
        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
        public IEnumerator<T> GetEnumerator() => _data.GetEnumerator();
    }
}
