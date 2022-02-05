using System;

namespace Reaction.Exceptions
{
    public class RxnTimeoutException : Exception
    {
        public RxnTimeoutException(float timeout) : base($"Rxn timed out after {timeout} seconds") { }
    }
}