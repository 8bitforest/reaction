using System.Collections.Generic;

namespace Reaction
{
    public class RxnList<T> : RxnCollection<List<T>, T>
    {
        protected override RxnOwnerValidator OwnerValidator { get; } = new RxnOwnerValidator();
    }
}