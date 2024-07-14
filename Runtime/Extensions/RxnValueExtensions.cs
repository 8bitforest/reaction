using System;
using UnityEngine;

namespace Reaction
{
    public static class RxnValueExtensions
    {
        public static void OnDecreased<T>(this IRxnReadOnlyValue<T> value, GameObject g, Action<ValueChange<T>> a)
            where T : IComparable
        {
            value.OnChangedWhen(g, c => c.New.CompareTo(c.Old) < 0, a);
        }

        public static void OnIncreased<T>(this IRxnReadOnlyValue<T> value, GameObject g, Action<ValueChange<T>> a)
            where T : IComparable
        {
            value.OnChangedWhen(g, c => c.New.CompareTo(c.Old) > 0, a);
        }
    }
}