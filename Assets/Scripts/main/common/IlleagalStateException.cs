using System;
using System.Runtime.Serialization;

namespace gamecore.common
{
    [Serializable]
    public class IlleagalStateException : Exception
    {
        public IlleagalStateException() { }

        public IlleagalStateException(string message)
            : base(message) { }

        public IlleagalStateException(string message, Exception innerException)
            : base(message, innerException) { }

        protected IlleagalStateException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
