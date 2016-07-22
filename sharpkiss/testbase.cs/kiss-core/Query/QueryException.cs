using System;

namespace Kiss.Query
{
    /// <summary>
    /// 查询的异常
    /// </summary>
    [Serializable]
    public class QueryException : KissException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public QueryException() { }
        public QueryException(string message) : base(message) { }
        public QueryException(string message, Exception inner) : base(message, inner) { }
        public QueryException(string format, params object[] objs) : base(format, objs) { }
        protected QueryException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
