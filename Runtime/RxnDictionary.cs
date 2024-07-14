using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reaction
{
    public struct DictionaryValueChange<TKey, TValue>
    {
        public TKey Key { get; }
        public TValue NewValue { get; }
        public TValue OldValue { get; }

        public DictionaryValueChange(TKey key, TValue oldValue, TValue newValue)
        {
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public interface IRxnReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        int OnChanged(GameObject g, Action a);
        int OnChangedInit(GameObject g, Action a);
        int OnKeyChanged(GameObject g, Action<DictionaryValueChange<TKey, TValue>> a);
        int OnKeyChangedInit(GameObject g, Action<DictionaryValueChange<TKey, TValue>> a);
        int OnKeyChanged(TKey key, GameObject g, Action<DictionaryValueChange<TKey, TValue>> a);
        int OnKeyChangedInit(TKey key, GameObject g, Action<DictionaryValueChange<TKey, TValue>> a);
        int OnKeyAdded(GameObject g, Action<KeyValuePair<TKey, TValue>> a);
        int OnKeyAddedInit(GameObject g, Action<KeyValuePair<TKey, TValue>> a);
        int OnKeyAdded(TKey key, GameObject g, Action<KeyValuePair<TKey, TValue>> a);
        int OnKeyAddedInit(TKey key, GameObject g, Action<KeyValuePair<TKey, TValue>> a);
        int OnKeyRemoved(GameObject g, Action<KeyValuePair<TKey, TValue>> a);
        int OnKeyRemoved(TKey key, GameObject g, Action<KeyValuePair<TKey, TValue>> a);
        void RemoveHandler(int id);
    }

    public class RxnDictionary<TKey, TValue> : IRxnReadOnlyDictionary<TKey, TValue>, IDictionary<TKey, TValue>
    {
        public int Count => _items.Count;
        public bool IsReadOnly => false;
        public IEnumerable<TKey> Keys => _items.Keys;
        public IEnumerable<TValue> Values => _items.Values;
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _items.Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => _items.Values;

        private readonly Dictionary<TKey, TValue> _items = new();

        private readonly RxnEvent _changeHandlers = new();
        private readonly RxnEvent<DictionaryValueChange<TKey, TValue>> _keyChangeHandlers = new();
        private readonly RxnEvent<KeyValuePair<TKey, TValue>> _addHandlers = new();
        private readonly RxnEvent<KeyValuePair<TKey, TValue>> _removeHandlers = new();

        // @formatter:off
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public bool ContainsKey(TKey key) => _items.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => _items.TryGetValue(key, out value);
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_items).Contains(item);
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)_items).CopyTo(array, arrayIndex);
        // @formatter:on

        public TValue this[TKey key]
        {
            get => _items[key];
            set
            {
                if (!_items.TryGetValue(key, out var oldValue))
                {
                    Add(key, value);
                }
                else
                {
                    _items[key] = value;
                    _keyChangeHandlers.Invoke(new DictionaryValueChange<TKey, TValue>(key, oldValue, value));
                }

                _changeHandlers.Invoke();
            }
        }

        public void Add(TKey key, TValue value)
        {
            _items.Add(key, value);
            _addHandlers.Invoke(new KeyValuePair<TKey, TValue>(key, value));
            _changeHandlers.Invoke();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_items).Add(item);
            _addHandlers.Invoke(item);
            _changeHandlers.Invoke();
        }

        public bool Remove(TKey key)
        {
            _items.TryGetValue(key, out var oldValue);
            if (_items.Remove(key))
            {
                _removeHandlers.Invoke(new KeyValuePair<TKey, TValue>(key, oldValue));
                _changeHandlers.Invoke();
                return true;
            }

            return false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (((ICollection<KeyValuePair<TKey, TValue>>)_items).Remove(item))
            {
                _removeHandlers.Invoke(item);
                _changeHandlers.Invoke();
                return true;
            }

            return false;
        }

        public void Clear()
        {
            if (_items.Count == 0)
                return;

            foreach (var kv in _items)
                _removeHandlers.Invoke(kv);

            _items.Clear();
            _changeHandlers.Invoke();
        }

        public int OnChanged(GameObject g, Action a) => OnChanged(g, a, false);
        public int OnChangedInit(GameObject g, Action a) => OnChanged(g, a, true);

        private int OnChanged(GameObject g, Action a, bool init)
        {
            var id = _changeHandlers.OnInvoked(g, a);
            if (init) a();
            return id;
        }
        
        // @formatter:off
        public int OnKeyChanged(GameObject g, Action<DictionaryValueChange<TKey, TValue>> a) => OnKeyChanged(g, a, false);
        public int OnKeyChangedInit(GameObject g, Action<DictionaryValueChange<TKey, TValue>> a) => OnKeyChanged(g, a, true);
        // @formatter:on

        private int OnKeyChanged(GameObject g, Action<DictionaryValueChange<TKey, TValue>> a, bool init)
        {
            var id = _keyChangeHandlers.OnInvoked(g, a);
            if (init)
            {
                foreach (var kv in _items)
                    a(new DictionaryValueChange<TKey, TValue>(kv.Key, kv.Value, kv.Value));
            }

            return id;
        }
        
        // @formatter:off
        public int OnKeyChanged(TKey key, GameObject g, Action<DictionaryValueChange<TKey, TValue>> a) => OnKeyChanged(key, g, a, false);
        public int OnKeyChangedInit(TKey key, GameObject g, Action<DictionaryValueChange<TKey, TValue>> a) => OnKeyChanged(key, g, a, true);
        // @formatter:on

        private int OnKeyChanged(TKey key, GameObject g, Action<DictionaryValueChange<TKey, TValue>> a, bool init)
        {
            return OnKeyChanged(g, change =>
            {
                if (change.Key.Equals(key))
                    a(change);
            }, init);
        }

        public int OnKeyAdded(GameObject g, Action<KeyValuePair<TKey, TValue>> a) => OnKeyAdded(g, a, false);
        public int OnKeyAddedInit(GameObject g, Action<KeyValuePair<TKey, TValue>> a) => OnKeyAdded(g, a, true);

        private int OnKeyAdded(GameObject g, Action<KeyValuePair<TKey, TValue>> a, bool init)
        {
            var id = _addHandlers.OnInvoked(g, a);
            if (init)
            {
                foreach (var kv in _items)
                    a(kv);
            }

            return id;
        }
        
        // @formatter:off
        public int OnKeyAdded(TKey key, GameObject g, Action<KeyValuePair<TKey, TValue>> a) => OnKeyAdded(key, g, a, false);
        public int OnKeyAddedInit(TKey key, GameObject g, Action<KeyValuePair<TKey, TValue>> a) => OnKeyAdded(key, g, a, true);
        // @formatter:on

        private int OnKeyAdded(TKey key, GameObject g, Action<KeyValuePair<TKey, TValue>> a, bool init)
        {
            return OnKeyAdded(g, kv =>
            {
                if (kv.Key.Equals(key))
                    a(kv);
            }, init);
        }

        public int OnKeyRemoved(GameObject g, Action<KeyValuePair<TKey, TValue>> a)
        {
            return _removeHandlers.OnInvoked(g, a);
        }

        public int OnKeyRemoved(TKey key, GameObject g, Action<KeyValuePair<TKey, TValue>> a)
        {
            return OnKeyRemoved(g, kv =>
            {
                if (kv.Key.Equals(key))
                    a(kv);
            });
        }

        public void RemoveHandler(int id)
        {
            _addHandlers.RemoveHandler(id);
            _removeHandlers.RemoveHandler(id);
            _changeHandlers.RemoveHandler(id);
            _keyChangeHandlers.RemoveHandler(id);
        }
    }
}