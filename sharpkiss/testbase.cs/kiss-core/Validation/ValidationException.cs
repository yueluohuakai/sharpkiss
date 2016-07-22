using System;

namespace Kiss.Validation
{
    public class ValidationException : KissException
    {
        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception inner) : base(message, inner) { }
        public ValidationException(string format, params object[] objs) : base(string.Format(format, objs)) { }
        protected ValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
