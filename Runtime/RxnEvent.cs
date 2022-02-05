using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reaction.Exceptions;
using UnityEngine;

namespace Reaction
{
    public class RxnEvent : RxnEventBase<RxnEvent.RxnEventOwner>
    {
        protected override RxnEventBaseOwner Owner => new RxnEventOwner(this);
        public int OnInvoked(GameObject g, Action a) => OnInvokedBase(g, a);

        public async Task Wait(float timeout = Rxn.Timeout)
            => await WaitBase(timeout);

        public class RxnEventOwner : RxnEventBaseOwner
        {
            public RxnEventOwner(RxnEventBase<RxnEventOwner> @base) : base(@base) { }
            public void Invoke() => InvokeBase();
            public void BindTo(GameObject g, RxnEvent other) => other.OnInvoked(g, Invoke);
        }
    }

    public class RxnEvent<T> : RxnEventBase<RxnEvent<T>.RxnEventOwner>
    {
        protected override RxnEventBaseOwner Owner => new RxnEventOwner(this);
        public int OnInvoked(GameObject g, Action<T> a) => OnInvokedBase(g, a);

        public async Task<T> Wait(float timeout = Rxn.Timeout)
            => (T) (await WaitBase(timeout))[0];

        public class RxnEventOwner : RxnEventBaseOwner
        {
            public RxnEventOwner(RxnEventBase<RxnEventOwner> @base) : base(@base) { }
            public void Invoke(T t) => InvokeBase(t);
            public void BindTo(GameObject g, RxnEvent<T> other) => other.OnInvoked(g, Invoke);
        }
    }

    public class RxnEvent<T, T2> : RxnEventBase<RxnEvent<T, T2>.RxnEventOwner>
    {
        protected override RxnEventBaseOwner Owner => new RxnEventOwner(this);
        public int OnInvoked(GameObject g, Action<T, T2> a) => OnInvokedBase(g, a);

        public async Task<(T, T2)> Wait(float timeout = Rxn.Timeout)
            => TupleExtensions.FromArray<T, T2>(await WaitBase(timeout));

        public class RxnEventOwner : RxnEventBaseOwner
        {
            public RxnEventOwner(RxnEventBase<RxnEventOwner> @base) : base(@base) { }
            public void Invoke(T t, T2 t2) => InvokeBase(t, t2);
            public void BindTo(GameObject g, RxnEvent<T, T2> other) => other.OnInvoked(g, Invoke);
        }
    }

    public class RxnEvent<T, T2, T3> : RxnEventBase<RxnEvent<T, T2, T3>.RxnEventOwner>
    {
        protected override RxnEventBaseOwner Owner => new RxnEventOwner(this);
        public int OnInvoked(GameObject g, Action<T, T2, T3> a) => OnInvokedBase(g, a);

        public async Task<(T, T2, T3)> Wait(float timeout = Rxn.Timeout)
            => TupleExtensions.FromArray<T, T2, T3>(await WaitBase(timeout));

        public class RxnEventOwner : RxnEventBaseOwner
        {
            public RxnEventOwner(RxnEventBase<RxnEventOwner> @base) : base(@base) { }
            public void Invoke(T t, T2 t2, T3 t3) => InvokeBase(t, t2, t3);
            public void BindTo(GameObject g, RxnEvent<T, T2, T3> other) => other.OnInvoked(g, Invoke);
        }
    }

    public class RxnEvent<T, T2, T3, T4> : RxnEventBase<RxnEvent<T, T2, T3, T4>.RxnEventOwner>
    {
        protected override RxnEventBaseOwner Owner => new RxnEventOwner(this);
        public int OnInvoked(GameObject g, Action<T, T2, T3, T4> a) => OnInvokedBase(g, a);

        public async Task<(T, T2, T3, T4)> Wait(float timeout = Rxn.Timeout)
            => TupleExtensions.FromArray<T, T2, T3, T4>(await WaitBase(timeout));

        public class RxnEventOwner : RxnEventBaseOwner
        {
            public RxnEventOwner(RxnEventBase<RxnEventOwner> @base) : base(@base) { }
            public void Invoke(T t, T2 t2, T3 t3, T4 t4) => InvokeBase(t, t2, t3, t4);
            public void BindTo(GameObject g, RxnEvent<T, T2, T3, T4> other) => other.OnInvoked(g, Invoke);
        }
    }

    public abstract class RxnEventBase<TOwner> where TOwner : RxnEventBase<TOwner>.RxnEventBaseOwner
    {
        public TOwner AsOwner
        {
            get
            {
                _ownerValidator.Validate();
                return Owner as TOwner;
            }
        }

        protected abstract RxnEventBaseOwner Owner { get; }
        
        private readonly RxnOwnerValidator _ownerValidator = new RxnOwnerValidator(3);
        private readonly Dictionary<int, Handler> _handlers = new Dictionary<int, Handler>();
        private readonly List<TaskCompletionSource<object[]>> _waitTasks = new List<TaskCompletionSource<object[]>>();

        protected int OnInvokedBase(GameObject g, Delegate a)
        {
            var handler = new Handler(g, a);
            _handlers.Add(handler.Id, handler);
            return handler.Id;
        }

        private int OnInvokedBase(GameObject g, Action<object[]> a)
        {
            var handler = new Handler(g, a);
            _handlers.Add(handler.Id, handler);
            return handler.Id;
        }

        protected async Task<object[]> WaitBase(float timeout)
        {
            // This does not re-use OnInvoked so that when we return, we return in a context where it is valid to call
            // Invoke.
            
            var task = new TaskCompletionSource<object[]>();
            _waitTasks.Add(task);

            if (timeout == 0)
                return await task.Task;
            
            if (await Task.WhenAny(task.Task, Task.Delay((int) (timeout * 1000))) == task.Task)
                return task.Task.Result;

            throw new RxnTimeoutException(timeout);
        }

        public void RemoveHandler(int id)
        {
            if (!_handlers.ContainsKey(id))
                return;

            _handlers.Remove(id);
        }

        public void RemoveHandlers(GameObject gameObject)
        {
            var toRemove =
                new HashSet<int>(_handlers.Where(kv => kv.Value.GameObject == gameObject).Select(kv => kv.Key));
            foreach (var id in toRemove)
                RemoveHandler(id);
        }

        public abstract class RxnEventBaseOwner
        {
            private readonly RxnEventBase<TOwner> _base;

            protected RxnEventBaseOwner(RxnEventBase<TOwner> @base)
            {
                _base = @base;
            }

            protected void InvokeBase(params object[] args)
            {
                // Make sure to call .ToList() to create a copy so that _handlers can be modified while invoking
                foreach (var handler in _base._handlers.Values.ToList())
                {
                    if ((object) handler.GameObject != null && handler.GameObject == null)
                        Debug.Log("RxnEvent found a destroyed GameObject! Removing...");
                    else
                        handler.Invoke(args);
                }

                // Very important to complete these AFTER running all the handlers. This ensures that any calls to Wait
                // will return outside of a handler context. This allows Invoke to be called right after Wait. We also
                // need to make sure to create a copy of the current wait tasks, so that Wait can be called after
                // another Wait returns
                var tasks = _base._waitTasks.ToList();
                _base._waitTasks.Clear();
                foreach (var task in tasks)
                    task.SetResult(args);
            }
        }
    }
}