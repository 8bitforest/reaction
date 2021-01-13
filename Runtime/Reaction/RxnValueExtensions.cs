using System;
using UnityEngine;

namespace Reaction
{
    public static class RxnValueExtensions
    {
        public static void OnDecreased<T>(this RxnValue<T> value, GameObject g, Action a) where T : IComparable
            => OnDecreased(value, g, _ => a());

        public static void OnDecreased<T>(this RxnValue<T> value, GameObject g, Action<T> a) where T : IComparable
        {
            value.OnChangedWhen(g, (newValue, oldValue) => newValue.CompareTo(oldValue) < 0, a);
        }

        public static void OnIncreased<T>(this RxnValue<T> value, GameObject g, Action a) where T : IComparable
            => OnIncreased(value, g, _ => a());

        public static void OnIncreased<T>(this RxnValue<T> value, GameObject g, Action<T> a) where T : IComparable
        {
            value.OnChangedWhen(g, (newValue, oldValue) => newValue.CompareTo(oldValue) > 0, a);
        }
    }
}