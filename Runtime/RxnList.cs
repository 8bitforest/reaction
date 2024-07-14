using System.Collections.Generic;

namespace Reaction
{
    public interface IRxnReadOnlyList<T> : IRxnReadOnlyCollection<T> { }

    public class RxnList<T> : RxnCollection<List<T>, T> { }
}