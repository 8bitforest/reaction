using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Reaction
{
    public class RxnDictionary<TKey, TValue>
        : RxnCollection<Dictionary<TKey, RxnValue<TValue>>, KeyValuePair<TKey, RxnValue<TValue>>,
            RxnDictionary<TKey, TValue>.RxnDictionaryOwner>
    {
        protected override RxnDictionaryOwner Owner => new RxnDictionaryOwner(this);

        protected override RxnOwnerValidator OwnerValidator { get; } = new RxnOwnerValidator();

        public bool ContainsKey(TKey key) => Items.ContainsKey(key);
        public bool ContainsValue(TValue value) => Items.ContainsValue(new RxnValue<TValue>(value));
        public bool TryGetValue(TKey key, out RxnValue<TValue> value) => Items.TryGetValue(key, out value);
        public RxnValue<TValue> this[TKey key] => Items[key];
        public IEnumerable<TKey> Keys => Items.Keys;
        public IEnumerable<RxnValue<TValue>> RxnValues => Items.Values;
        public IEnumerable<TValue> Values => Items.Values.Select(v => v.Value);

        public int OnKeyAdded(TKey key, GameObject g, Action a)
            => OnKeyAdded(key, g, _ => a());

        public int OnKeyAdded(TKey key, GameObject g, Action<KeyValuePair<TKey, RxnValue<TValue>>> a)
        {
            return OnAddedWhen(g, kv => kv.Key.Equals(key), a);
        }

        public int RelayKeyAdded(TKey key, GameObject g, Action a)
            => RelayKeyAdded(key, g, _ => a());

        public int RelayKeyAdded(TKey key, GameObject g, Action<KeyValuePair<TKey, RxnValue<TValue>>> a)
        {
            var id = OnAddedWhen(g, kv => kv.Key.Equals(key), a);
            if (Items.ContainsKey(key))
                a(new KeyValuePair<TKey, RxnValue<TValue>>(key, Items[key]));
            return id;
        }

        public int OnAddedKey(GameObject g, Action<TKey> a)
            => OnAdded(g, kv => a(kv.Key));

        public int RelayAddedKey(GameObject g, Action<TKey> a)
            => RelayAdded(g, kv => a(kv.Key));

        public int OnAddedValue(GameObject g, Action<TValue> a)
            => OnAdded(g, kv => a(kv.Value));

        public int RelayAddedValue(GameObject g, Action<TValue> a)
            => RelayAdded(g, kv => a(kv.Value));


        public class RxnDictionaryOwner : RxnCollectionOwner<RxnDictionary<TKey, TValue>>
        {
            public RxnDictionaryOwner(RxnDictionary<TKey, TValue> collection) : base(collection) { }

            public TValue this[TKey key]
            {
                set
                {
                    if (!Collection.ContainsKey(key))
                    {
                        Add(key, value);
                    }
                    else
                    {
                        Collection.Items[key].AsOwner.Set(value);
                        InvokeHandlers(new KeyValuePair<TKey, RxnValue<TValue>>(key, Collection.Items[key]));
                    }
                }
            }

            public void Add(TKey key, TValue value)
            {
                Add(new KeyValuePair<TKey, RxnValue<TValue>>(key, new RxnValue<TValue>(value)));
            }

            public void Remove(TKey key)
            {
                Remove(new KeyValuePair<TKey, RxnValue<TValue>>(key, Collection[key]));
            }
        }
    }
}