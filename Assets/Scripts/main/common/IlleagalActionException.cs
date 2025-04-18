using System;

namespace gamecore.common
{
    public class IlleagalActionException : Exception
    {
        public IlleagalActionException(string message)
            : base(message) { }
    }
}
