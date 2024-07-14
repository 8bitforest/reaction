using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reaction
{
    public interface IRxnReadOnlyCollection<TItem> : IReadOnlyCollection<TItem>
    {
        bool Contains(TItem item);
        int OnChanged(GameObject g, Action a);
        int OnChangedInit(GameObject g, Action a);
        int OnAdded(GameObject g, Action<TItem> a);
        int OnAddedInit(GameObject g, Action<TItem> a);
        int OnAddedWhen(GameObject g, Func<TItem, bool> p, Action<TItem> a);
        int OnAddedWhenInit(GameObject g, Func<TItem, bool> p, Action<TItem> a);
        int OnItemAdded(TItem item, GameObject g, Action<TItem> a);
        int OnItemAddedInit(TItem item, GameObject g, Action<TItem> a);
        int OnRemoved(GameObject g, Action<TItem> a);
        int OnRemovedWhen(GameObject g, Func<TItem, bool> p, Action<TItem> a);
        int OnItemRemoved(TItem item, GameObject g, Action<TItem> a);
        void RemoveHandler(int id);
    }

    public abstract class RxnCollection<TCollection, TItem> : ICollection<TItem>, IRxnReadOnlyCollection<TItem>
        where TCollection : ICollection<TItem>, new()
    {
        public int Count => Items.Count;
        public bool IsReadOnly => false;

        protected TCollection Items { get; } = new();
        private readonly RxnEvent _changeHandlers = new();
        private readonly RxnEvent<TItem> _addHandlers = new();
        private readonly RxnEvent<TItem> _removeHandlers = new();

        public IEnumerator<TItem> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public bool Contains(TItem item) => Items.Contains(item);
        public void CopyTo(TItem[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

        public int OnChanged(GameObject g, Action a) => OnChanged(g, a, false);
        public int OnChangedInit(GameObject g, Action a) => OnChanged(g, a, true);

        protected int OnChanged(GameObject g, Action a, bool init)
        {
            var id = _changeHandlers.OnInvoked(g, a);
            if (init) a();
            return id;
        }

        public int OnAdded(GameObject g, Action<TItem> a) => OnAdded(g, a, false);
        public int OnAddedInit(GameObject g, Action<TItem> a) => OnAdded(g, a, true);

        protected int OnAdded(GameObject g, Action<TItem> a, bool init)
        {
            var id = _addHandlers.OnInvoked(g, a);
            if (init)
            {
                foreach (var item in Items)
                    a(item);
            }

            return id;
        }
        
        // @formatter:off
        public int OnAddedWhen(GameObject g, Func<TItem, bool> p, Action<TItem> a) => OnAddedWhen(g, p, a, false);
        public int OnAddedWhenInit(GameObject g, Func<TItem, bool> p, Action<TItem> a) => OnAddedWhen(g, p, a, true);
        // @formatter:on

        protected int OnAddedWhen(GameObject g, Func<TItem, bool> p, Action<TItem> a, bool init)
        {
            return OnAdded(g, item =>
            {
                if (p(item))
                    a(item);
            }, init);
        }

        public int OnItemAdded(TItem item, GameObject g, Action<TItem> a) => OnItemAdded(item, g, a, false);
        public int OnItemAddedInit(TItem item, GameObject g, Action<TItem> a) => OnItemAdded(item, g, a, true);

        protected int OnItemAdded(TItem item, GameObject g, Action<TItem> a, bool init)
        {
            return OnAdded(g, i =>
            {
                if (i.Equals(item))
                    a(i);
            }, init);
        }

        public int OnRemoved(GameObject g, Action<TItem> a)
        {
            return _removeHandlers.OnInvoked(g, a);
        }

        public int OnRemovedWhen(GameObject g, Func<TItem, bool> p, Action<TItem> a)
        {
            return OnRemoved(g, i =>
            {
                if (p(i)) a(i);
            });
        }

        public int OnItemRemoved(TItem item, GameObject g, Action<TItem> a)
        {
            return OnRemoved(g, i =>
            {
                if (i.Equals(item)) a(i);
            });
        }

        public void RemoveHandler(int id)
        {
            _addHandlers.RemoveHandler(id);
            _removeHandlers.RemoveHandler(id);
            _changeHandlers.RemoveHandler(id);
        }

        public void Add(TItem item)
        {
            Items.Add(item);
            _changeHandlers.Invoke();
            _addHandlers.Invoke(item);
        }

        public bool Remove(TItem item)
        {
            if (!RemoveInternal(item))
                return false;

            _changeHandlers.Invoke();
            return true;
        }

        public void Clear()
        {
            if (Items.Count == 0)
                return;

            foreach (var item in Items)
                _removeHandlers.Invoke(item);

            Items.Clear();
            _changeHandlers.Invoke();
        }

        private bool RemoveInternal(TItem item)
        {
            if (!Items.Remove(item))
                return false;

            _removeHandlers.Invoke(item);
            return true;
        }
    }
}