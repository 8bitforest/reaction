using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

namespace Reaction
{
    public struct ValueChange<T>
    {
        public T New { get; }
        public T Old { get; }

        public ValueChange(T old, T @new)
        {
            Old = old;
            New = @new;
        }
    }

    public interface IRxnReadOnlyValue<T>
    {
        T Value { get; }
        int OnChanged(GameObject g, Action<ValueChange<T>> a);
        int OnChangedInit(GameObject g, Action<ValueChange<T>> a);
        int OnChangedWhen(GameObject g, Func<ValueChange<T>, bool> p, Action<ValueChange<T>> a);
        int OnChangedWhenInit(GameObject g, Func<ValueChange<T>, bool> p, Action<ValueChange<T>> a);
        int OnChangedTo(T v, GameObject g, Action<ValueChange<T>> a);
        int OnChangedToInit(T v, GameObject g, Action<ValueChange<T>> a);
        Task WaitUntil(T v, float timeout = Rxn.Timeout);
        void RemoveHandler(int id);
    }

    public class RxnValue<T> : IRxnReadOnlyValue<T>
    {
        public T Value { get; private set; }
        private readonly RxnEvent<ValueChange<T>> _handlers;

        public RxnValue() : this(default) { }

        public RxnValue(T value)
        {
            Value = value;
            _handlers = new RxnEvent<ValueChange<T>>();
        }

        public void Set(T newValue, bool forceUpdate = false, bool ignoreUpdate = false)
        {
            var changed = !Value?.Equals(newValue) ?? newValue != null;
            var oldValue = Value;
            Value = newValue;

            if (!ignoreUpdate && (forceUpdate || changed))
                _handlers.Invoke(new ValueChange<T>(oldValue, newValue));
        }

        // Implementing IRxnValue<T> methods
        public int OnChanged(GameObject g, Action<ValueChange<T>> a) => OnChanged(g, a, false);
        public int OnChangedInit(GameObject g, Action<ValueChange<T>> a) => OnChanged(g, a, true);

        private int OnChanged(GameObject g, Action<ValueChange<T>> a, bool init)
        {
            var id = _handlers.OnInvoked(g, a);
            if (init)
                a(new ValueChange<T>(Value, Value));
            return id;
        }

        // @formatter:off
        public int OnChangedWhen(GameObject g, Func<ValueChange<T>, bool> p, Action<ValueChange<T>> a) => OnChangedWhen(g, p, a, false);
        public int OnChangedWhenInit(GameObject g, Func<ValueChange<T>, bool> p, Action<ValueChange<T>> a) => OnChangedWhen(g, p, a, true);
        // @formatter:on

        private int OnChangedWhen(GameObject g, Func<ValueChange<T>, bool> p, Action<ValueChange<T>> a, bool init)
        {
            return OnChanged(g, (change) =>
            {
                if (p(change))
                    a(change);
            }, init);
        }

        public int OnChangedTo(T v, GameObject g, Action<ValueChange<T>> a) => OnChangedTo(v, g, a, false);
        public int OnChangedToInit(T v, GameObject g, Action<ValueChange<T>> a) => OnChangedTo(v, g, a, true);

        private int OnChangedTo(T v, GameObject g, Action<ValueChange<T>> a, bool init)
        {
            return OnChanged(g, (change) =>
            {
                if (change.New.Equals(v))
                    a(change);
            }, init);
        }

        public async Task WaitUntil(T v, float timeout = Rxn.Timeout)
        {
            var hasTimeout = timeout > 0;
            var timeoutWatch = Stopwatch.StartNew();

            while (!Value.Equals(v))
            {
                var msLeft = (long)(timeout * 1000) - timeoutWatch.ElapsedMilliseconds;
                if (hasTimeout && msLeft <= 0)
                    throw new RxnTimeoutException(timeout);

                try
                {
                    await _handlers.Wait(hasTimeout ? msLeft / 1000f : 0);
                }
                catch (RxnTimeoutException)
                {
                    throw new RxnTimeoutException(timeout);
                }
            }
        }

        public void RemoveHandler(int id)
        {
            _handlers.RemoveHandler(id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Value.Equals(((RxnValue<T>)obj).Value);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Value.GetHashCode();
        }

        public static implicit operator T(RxnValue<T> value)
        {
            return value.Value;
        }
    }
}