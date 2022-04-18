using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Platform.Tenant.Management.Tests.L0.MockData
{
	internal class MockCacheProvider : ICacheProvider
    {
        private Guid _key;
        private object _value;
        public event EventHandler WrittenTo;
        public event EventHandler ReadFrom;
        public MockCacheProvider() : this(Guid.Empty)
        {
        }

        public MockCacheProvider(Guid key) : this(key, null)
        {
        }

        public MockCacheProvider(Guid key, object value)
        {
            this._key = key;
            this._value = value;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(string key)
        {
            throw new NotImplementedException();
        }

        public object Delete(string key)
        {
            throw new NotImplementedException();
        }

        public object Get(string key)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<object, Task<T>> factory, CancellationToken cancellationToken)
        {
            if (this._key.ToString() == key)
                return Task.FromResult<T>((T)this._value);
            return factory(key);
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<object, Task<T>> factory, ICacheEntryOptions policy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Initialise()
        {
            throw new NotImplementedException();
        }

        public void Post(string key, object value)
        {
            throw new NotImplementedException();
        }

        public void Post(string key, object value, ICacheEntryOptions policy)
        {
            throw new NotImplementedException();
        }

        public void Put(string key, object value)
        {
            throw new NotImplementedException();
        }

        public void Put(string key, object value, ICacheEntryOptions policy)
        {
            throw new NotImplementedException();
        }

        public bool TryGet(string key, out object item)
        {
            throw new NotImplementedException();
        }

        public bool TryGet<T>(string key, out T item)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, T> TypeOf<T>()
        {
            throw new NotImplementedException();
        }
    }
}
