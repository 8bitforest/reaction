using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Reaction
{
    public class RxnOwnerValidator
    {
        private readonly string _ownerFile;

        static RxnOwnerValidator()
        {
#if DEBUG
            Debug.Log("DEBUG flag is set, RxnOwnerValidator will validate Rxn class owner calls!");
#endif
        }

        public RxnOwnerValidator(int ownerStackPos = 2)
        {
#if DEBUG
            _ownerFile = new StackTrace(true).GetFrame(ownerStackPos).GetFileName();
#endif
        }

        public void Validate(int callerStackPos = 2)
        {
#if DEBUG
            var callerFile = new StackTrace(true).GetFrame(callerStackPos).GetFileName();
            if (callerFile != _ownerFile)
                throw new Exception($"{callerFile} is trying to modify {_ownerFile}!");
#endif
        }
    }
}