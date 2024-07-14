using System.Collections.Generic;

namespace Reaction
{
    public interface IRxnReadOnlySet<T> : IRxnReadOnlyCollection<T> { }

    public class RxnSet<T> : RxnCollection<HashSet<T>, T> { }
}