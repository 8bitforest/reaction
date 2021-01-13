using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Reaction
{
    public abstract class RxnCollection<TCollection, TItem>
        : RxnCollection<TCollection, TItem, RxnCollection<TCollection, TItem>.RxnCollectionDefaultOwner>
        where TCollection : ICollection<TItem>, new()
    {
        protected override RxnCollectionDefaultOwner Owner => new RxnCollectionDefaultOwner(this);

        public class RxnCollectionDefaultOwner : RxnCollectionOwner<RxnCollection<TCollection, TItem>>
        {
            public RxnCollectionDefaultOwner(RxnCollection<TCollection, TItem> collection) : base(collection) { }
        }
    }

    public abstract class RxnCollection<TCollection, TItem, TOwner> : IEnumerable<TItem>
        where TOwner : IRxnCollectionOwner
        where TCollection : ICollection<TItem>, new()
    {
        public TOwner AsOwner
        {
            get
            {
                OwnerValidator.Validate();
                return Owner;
            }
        }

        public int Count => Items.Count;

        protected TCollection Items { get; }
        protected abstract TOwner Owner { get; }
        protected abstract RxnOwnerValidator OwnerValidator { get; }

        private readonly RxnEvent<TItem> _handlers = new RxnEvent<TItem>();
        private readonly RxnEvent<TItem> _addHandlers = new RxnEvent<TItem>();
        private readonly RxnEvent<TItem> _removeHandlers = new RxnEvent<TItem>();

        protected RxnCollection()
        {
            Items = new TCollection();
        }

        public bool Contains(TItem item)
        {
            return Items.Contains(item);
        }

        public int OnChanged(GameObject g, Action a)
            => OnChanged(g, _ => a());

        public int OnChanged(GameObject g, Action<TItem> a)
        {
            return _handlers.OnInvoked(g, a);
        }

        public int RelayChanged(GameObject g, Action a)
            => RelayChanged(g, _ => a());

        public int RelayChanged(GameObject g, Action<TItem> a)
        {
            var id = OnChanged(g, a);
            foreach (var item in Items)
                a(item);
            return id;
        }

        public int OnChangedWhen(GameObject g, Func<TItem, bool> predicate, Action a)
            => OnChangedWhen(g, predicate, _ => a());

        public int OnChangedWhen(GameObject g, Func<TItem, bool> predicate, Action<TItem> a)
        {
            return OnChanged(g, item =>
            {
                if (predicate(item))
                    a(item);
            });
        }

        public int RelayChangedWhen(GameObject g, Func<TItem, bool> predicate, Action a)
            => RelayChangedWhen(g, predicate, _ => a());

        public int RelayChangedWhen(GameObject g, Func<TItem, bool> predicate, Action<TItem> a)
        {
            var id = OnChangedWhen(g, predicate, a);
            var existingItem = Items.FirstOrDefault(predicate);
            if (existingItem != null)
                a(existingItem);
            return id;
        }

        public int OnAdded(GameObject g, Action a)
            => OnAdded(g, _ => a());

        public int OnAdded(GameObject g, Action<TItem> a)
        {
            return _addHandlers.OnInvoked(g, a);
        }

        public int RelayAdded(GameObject g, Action a)
            => RelayAdded(g, _ => a());

        public int RelayAdded(GameObject g, Action<TItem> a)
        {
            var id = OnAdded(g, a);
            foreach (var item in Items)
                a(item);
            return id;
        }

        public int OnAddedWhen(GameObject g, Func<TItem, bool> predicate, Action a)
            => OnAddedWhen(g, predicate, _ => a());

        public int OnAddedWhen(GameObject g, Func<TItem, bool> predicate, Action<TItem> a)
        {
            return OnAdded(g, item =>
            {
                if (predicate(item))
                    a(item);
            });
        }

        public int RelayAddedWhen(GameObject g, Func<TItem, bool> predicate, Action a)
            => RelayAddedWhen(g, predicate, _ => a());

        public int RelayAddedWhen(GameObject g, Func<TItem, bool> predicate, Action<TItem> a)
        {
            var id = OnAddedWhen(g, predicate, a);
            var existingItem = Items.FirstOrDefault(predicate);
            if (existingItem != null)
                a(existingItem);
            return id;
        }

        public int OnItemAdded(TItem item, GameObject g, Action a)
            => OnItemAdded(item, g, _ => a());

        public int OnItemAdded(TItem item, GameObject g, Action<TItem> a)
        {
            return OnAddedWhen(g, removedItem => item.Equals(removedItem), a);
        }

        public int RelayItemAdded(TItem item, GameObject g, Action a)
            => RelayItemAdded(item, g, _ => a());

        public int RelayItemAdded(TItem item, GameObject g, Action<TItem> a)
        {
            return RelayAddedWhen(g, removedItem => item.Equals(removedItem), a);
        }

        public int OnRemoved(GameObject g, Action a)
            => OnRemoved(g, _ => a());

        public int OnRemoved(GameObject g, Action<TItem> a)
        {
            return _removeHandlers.OnInvoked(g, a);
        }

        public int OnRemovedWhen(GameObject g, Func<TItem, bool> predicate, Action a)
            => OnRemovedWhen(g, predicate, _ => a());

        public int OnRemovedWhen(GameObject g, Func<TItem, bool> predicate, Action<TItem> a)
        {
            return OnRemoved(g, item =>
            {
                if (predicate(item))
                    a(item);
            });
        }

        public int OnItemRemoved(TItem item, GameObject g, Action a)
            => OnItemRemoved(item, g, _ => a());

        public int OnItemRemoved(TItem item, GameObject g, Action<TItem> a)
        {
            return OnRemovedWhen(g, removedItem => item.Equals(removedItem), a);
        }

        public void RemoveHandler(int id)
        {
            _addHandlers.RemoveHandler(id);
            _removeHandlers.RemoveHandler(id);
            _handlers.RemoveHandler(id);
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract class RxnCollectionOwner<TRxnCollection> : IRxnCollectionOwner
            where TRxnCollection : RxnCollection<TCollection, TItem, TOwner>
        {
            protected readonly TRxnCollection Collection;

            protected RxnCollectionOwner(TRxnCollection collection)
            {
                Collection = collection;
            }

            protected void InvokeHandlers(TItem item)
            {
                Collection._handlers.AsOwner.Invoke(item);
            }

            public void Add(TItem item)
            {
                Collection.Items.Add(item);
                Collection._handlers.AsOwner.Invoke(item);
                Collection._addHandlers.AsOwner.Invoke(item);
            }

            public void Remove(TItem item)
            {
                Collection.Items.Remove(item);
                Collection._handlers.AsOwner.Invoke(item);
                Collection._removeHandlers.AsOwner.Invoke(item);
            }

            public void Clear()
            {
                foreach (var item in Collection.Items.ToArray())
                    Remove(item);
            }
        }
    }

    public interface IRxnCollectionOwner { }
}