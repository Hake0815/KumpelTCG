using System;
using System.Runtime.Serialization;

namespace gamecore.common
{
    [Serializable]
    public class IlleagalActionException : Exception
    {
        public IlleagalActionException() { }

        public IlleagalActionException(string message)
            : base(message) { }

        public IlleagalActionException(string message, Exception innerException)
            : base(message, innerException) { }

        protected IlleagalActionException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
