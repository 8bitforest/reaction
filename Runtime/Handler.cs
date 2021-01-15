using System;
using UnityEngine;

namespace Reaction
{
    public class Handler
    {
        private static int _lastId;

        public int Id { get; }
        public GameObject GameObject { get; }

        private readonly Delegate _delegate;
        private readonly Action<object[]> _action;

        public Handler(GameObject gameObject, Delegate @delegate)
        {
            Id = _lastId++;
            GameObject = gameObject;
            _delegate = @delegate;
        }

        public Handler(GameObject gameObject, Action<object[]> action)
        {
            Id = _lastId++;
            GameObject = gameObject;
            _action = action;
        }

        public void Invoke(object[] args)
        {
            _delegate?.DynamicInvoke(args);
            _action?.Invoke(args);
        }

        private bool Equals(Handler other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Handler) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}