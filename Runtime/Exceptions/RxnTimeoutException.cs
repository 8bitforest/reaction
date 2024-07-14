using System;

namespace Reaction
{
    public class RxnTimeoutException : Exception
    {
        public RxnTimeoutException(float timeout) : base($"Rxn timed out after {timeout} seconds") { }
    }
}