using System.Collections.Generic;

namespace Reaction
{
    public class RxnSet<T> : RxnCollection<HashSet<T>, T>
    {
        protected override RxnOwnerValidator OwnerValidator { get; } = new RxnOwnerValidator();
    }
}