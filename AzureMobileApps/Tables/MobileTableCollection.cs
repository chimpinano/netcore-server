using Microsoft.Azure.Mobile.Core.Server.Abstractions;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Mobile.Core.Server.Tables
{
    public class MobileTableCollection : IDictionary<string, ITable>
    {
        private IDictionary<string, ITable> _backingStore;

        public MobileTableCollection()
        {
            _backingStore = new Dictionary<string, ITable>();
        }

        public MobileTableCollection(ITableBuilder builder)
        {
            _backingStore = new Dictionary<string, ITable>();
            foreach (var table in builder.Tables)
            {
                _backingStore.Add(table.Name, table);
            }
        }

        public ITable this[string key]
        {
            get { return _backingStore[key]; }
            set { _backingStore[key] = value; }
        }

        public ICollection<string> Keys 
            => _backingStore.Keys;

        public ICollection<ITable> Values 
            => _backingStore.Values;

        public int Count 
            => _backingStore.Count;

        public bool IsReadOnly 
            => _backingStore.IsReadOnly;

        public void Add(string key, ITable value)
            => _backingStore.Add(key, value);

        public void Add(KeyValuePair<string, ITable> item)
            => _backingStore.Add(item.Key, item.Value);

        public void Clear()
           => _backingStore.Clear();

        public bool Contains(KeyValuePair<string, ITable> item)
            => _backingStore.Contains(item);

        public bool ContainsKey(string key) 
            => _backingStore.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, ITable>[] array, int arrayIndex)
            => _backingStore.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<string, ITable>> GetEnumerator()
            => _backingStore.GetEnumerator();

        public bool Remove(string key) 
            => _backingStore.Remove(key);

        public bool Remove(KeyValuePair<string, ITable> item)
            => _backingStore.Remove(item);

        public bool TryGetValue(string key, out ITable value)
            => _backingStore.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator()
            => _backingStore.GetEnumerator();
    }
}
