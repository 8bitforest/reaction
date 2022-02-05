namespace Reaction
{
    public static class TupleExtensions
    {
        public static (T, T2) FromArray<T, T2>(object[] array)
        {
            return ((T) array[0], (T2) array[1]);
        }

        public static (T, T2, T3) FromArray<T, T2, T3>(object[] array)
        {
            return ((T) array[0], (T2) array[1], (T3) array[2]);
        }

        public static (T, T2, T3, T4) FromArray<T, T2, T3, T4>(object[] array)
        {
            return ((T) array[0], (T2) array[1], (T3) array[2], (T4) array[3]);
        }
    }
}