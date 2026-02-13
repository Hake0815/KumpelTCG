using System;
using System.Runtime.Serialization;

namespace gamecore.common
{
    [Serializable]
    public class IllegalActionException : Exception
    {
        public IllegalActionException() { }

        public IllegalActionException(string message)
            : base(message) { }

        public IllegalActionException(string message, Exception innerException)
            : base(message, innerException) { }

        protected IllegalActionException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
