using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Reaction
{
    public interface IRxnReadOnlyEvent
    {
        int OnInvoked(GameObject g, Action a);
        Task Wait(float timeout = Rxn.Timeout);
        void RemoveHandler(int id);
        void RemoveHandlers(GameObject gameObject);
    }

    public interface IRxnReadOnlyEvent<T>
    {
        int OnInvoked(GameObject g, Action<T> a);
        Task<T> Wait(float timeout = Rxn.Timeout);
        void RemoveHandler(int id);
        void RemoveHandlers(GameObject gameObject);
    }

    public interface IRxnReadOnlyEvent<T, T2>
    {
        int OnInvoked(GameObject g, Action<T, T2> a);
        Task<(T, T2)> Wait(float timeout = Rxn.Timeout);
        void RemoveHandler(int id);
        void RemoveHandlers(GameObject gameObject);
    }

    public interface IRxnReadOnlyEvent<T, T2, T3>
    {
        int OnInvoked(GameObject g, Action<T, T2, T3> a);
        Task<(T, T2, T3)> Wait(float timeout = Rxn.Timeout);
        void RemoveHandler(int id);
        void RemoveHandlers(GameObject gameObject);
    }

    public interface IRxnReadOnlyEvent<T, T2, T3, T4>
    {
        int OnInvoked(GameObject g, Action<T, T2, T3, T4> a);
        Task<(T, T2, T3, T4)> Wait(float timeout = Rxn.Timeout);
        void RemoveHandler(int id);
        void RemoveHandlers(GameObject gameObject);
    }

    public class RxnEvent : RxnEventBase, IRxnReadOnlyEvent
    {
        public void Invoke() => InvokeBase();
        public int OnInvoked(GameObject g, Action a) => OnInvokedBase(g, a);
        public async Task Wait(float timeout = Rxn.Timeout) => await WaitBase(timeout);
    }

    public class RxnEvent<T> : RxnEventBase, IRxnReadOnlyEvent<T>
    {
        public void Invoke(T t) => InvokeBase(t);
        public int OnInvoked(GameObject g, Action<T> a) => OnInvokedBase(g, a);
        public async Task<T> Wait(float timeout = Rxn.Timeout) => (T)(await WaitBase(timeout))[0];
    }

    public class RxnEvent<T, T2> : RxnEventBase, IRxnReadOnlyEvent<T, T2>
    {
        public void Invoke(T t, T2 t2) => InvokeBase(t, t2);
        public int OnInvoked(GameObject g, Action<T, T2> a) => OnInvokedBase(g, a);

        public async Task<(T, T2)> Wait(float timeout = Rxn.Timeout) =>
            TupleExtensions.FromArray<T, T2>(await WaitBase(timeout));
    }

    public class RxnEvent<T, T2, T3> : RxnEventBase, IRxnReadOnlyEvent<T, T2, T3>
    {
        public void Invoke(T t, T2 t2, T3 t3) => InvokeBase(t, t2, t3);
        public int OnInvoked(GameObject g, Action<T, T2, T3> a) => OnInvokedBase(g, a);

        public async Task<(T, T2, T3)> Wait(float timeout = Rxn.Timeout) =>
            TupleExtensions.FromArray<T, T2, T3>(await WaitBase(timeout));
    }

    public class RxnEvent<T, T2, T3, T4> : RxnEventBase, IRxnReadOnlyEvent<T, T2, T3, T4>
    {
        public void Invoke(T t, T2 t2, T3 t3, T4 t4) => InvokeBase(t, t2, t3, t4);
        public int OnInvoked(GameObject g, Action<T, T2, T3, T4> a) => OnInvokedBase(g, a);

        public async Task<(T, T2, T3, T4)> Wait(float timeout = Rxn.Timeout) =>
            TupleExtensions.FromArray<T, T2, T3, T4>(await WaitBase(timeout));
    }

    public class RxnEventBase
    {
        private readonly Dictionary<int, Handler> _handlers = new();
        private readonly List<TaskCompletionSource<object[]>> _waitTasks = new();

        protected RxnEventBase() { }

        protected void InvokeBase(params object[] args)
        {
            // Make sure to call .ToList() to create a copy so that _handlers can be modified while invoking
            foreach (var handler in _handlers.Values.ToList())
            {
                if ((object)handler.GameObject != null && handler.GameObject == null)
                    Debug.Log("RxnEvent found a destroyed GameObject! Removing...");
                else
                    handler.Invoke(args);
            }

            // Very important to complete these AFTER running all the handlers. This ensures that any calls to Wait
            // will return outside of a handler context. This allows Invoke to be called right after Wait. We also
            // need to make sure to create a copy of the current wait tasks, so that Wait can be called after
            // another Wait returns
            var tasks = _waitTasks.ToList();
            _waitTasks.Clear();
            foreach (var task in tasks)
                task.SetResult(args);
        }

        protected int OnInvokedBase(GameObject g, Delegate a)
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

            if (await Task.WhenAny(task.Task, Task.Delay((int)(timeout * 1000))) == task.Task)
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
    }
}