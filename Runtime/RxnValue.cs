using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Reaction.Exceptions;
using UnityEngine;

namespace Reaction
{
    public class RxnValue<T>
    {
        public RxnValueOwner AsOwner
        {
            get
            {
                _ownerValidator.Validate();
                return _owner;
            }
        }

        public T Value { get; private set; }

        private readonly RxnValueOwner _owner;
        private readonly RxnOwnerValidator _ownerValidator = new RxnOwnerValidator();
        private readonly RxnEvent<T> _handlers = new RxnEvent<T>();

        public RxnValue()
        {
            _owner = new RxnValueOwner(this);
            Value = default;
        }

        public RxnValue(T value)
        {
            _owner = new RxnValueOwner(this);
            Value = value;
        }

        public int OnChanged(GameObject g, Action a)
            => OnChanged(g, _ => a());

        public int OnChanged(GameObject g, Action<T> a)
            => _handlers.OnInvoked(g, a);

        public int RelayChanged(GameObject g, Action a)
            => RelayChanged(g, _ => a());

        public int RelayChanged(GameObject g, Action<T> a)
        {
            var id = OnChanged(g, a);
            a(Value);
            return id;
        }

        public int OnChangedWhen(GameObject g, Func<T, T, bool> predicate, Action a)
            => OnChangedWhen(g, predicate, _ => a());

        public int OnChangedWhen(GameObject g, Func<T, T, bool> predicate, Action<T> a)
        {
            var prevValue = Value;
            return OnChanged(g, newValue =>
            {
                if (predicate(newValue, prevValue))
                    a(newValue);
                prevValue = Value;
            });
        }

        public int RelayChangedWhen(GameObject g, Func<T, T, bool> predicate, Action a)
            => RelayChangedWhen(g, predicate, _ => a());

        public int RelayChangedWhen(GameObject g, Func<T, T, bool> predicate, Action<T> a)
        {
            var prevValue = Value;
            return RelayChanged(g, newValue =>
            {
                if (predicate(newValue, prevValue))
                    a(newValue);
                prevValue = Value;
            });
        }

        public int OnChangedTo(T v, GameObject g, Action a)
            => OnChangedTo(v, g, _ => a());

        public int OnChangedTo(T v, GameObject g, Action<T> a)
        {
            return OnChangedWhen(g, (newValue, _) => newValue.Equals(v), a);
        }

        public int RelayChangedTo(T v, GameObject g, Action a)
            => RelayChangedTo(v, g, _ => a());

        public int RelayChangedTo(T v, GameObject g, Action<T> a)
        {
            var id = OnChangedTo(v, g, a);
            if (Value.Equals(v))
                a(Value);
            return id;
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

        protected bool Equals(RxnValue<T> other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((RxnValue<T>) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static implicit operator T(RxnValue<T> value)
        {
            return value.Value;
        }

        public class RxnValueOwner
        {
            private readonly RxnValue<T> _value;

            public RxnValueOwner(RxnValue<T> value)
            {
                _value = value;
            }

            public void Set(T newValue, bool forceUpdate = false, bool ignoreUpdate = false)
            {
                var changed = !_value.Value?.Equals(newValue) ?? newValue != null;
                _value.Value = newValue;

                if (!ignoreUpdate && (forceUpdate || changed))
                    _value._handlers.AsOwner.Invoke(_value.Value);
            }

            public int BindTo(GameObject g, RxnValue<T> otherValue)
            {
                return otherValue.RelayChanged(g, v => Set(v));
            }

            public int BindToWhen(GameObject g, RxnValue<T> otherValue, Func<T, T, bool> predicate)
            {
                return otherValue.RelayChangedWhen(g, predicate, v => Set(v));
            }
        }
    }
}