using System;
using UnityEngine;

namespace Reaction
{
    public class Handler
    {
        private static int _lastId;

        public int Id { get; }
        public GameObject GameObject { get; }
        public Delegate Delegate { get; }

        public Handler(GameObject gameObject, Delegate @delegate)
        {
            Id = _lastId++;
            GameObject = gameObject;
            Delegate = @delegate;
        }


        private bool Equals(Handler other)
        {
            return GameObject == other.GameObject && Delegate == other.Delegate;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Handler) obj);
        }

        public override int GetHashCode()
        {
            return ((GameObject != null ? GameObject.GetHashCode() : 0) * 397) ^ Delegate.GetHashCode();
        }
    }
}