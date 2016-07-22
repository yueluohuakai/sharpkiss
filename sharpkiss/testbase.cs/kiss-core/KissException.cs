using System;

namespace Kiss
{
    /// <summary>
    /// 自定义异常的基类，框架中异常从此类继承
    /// </summary>
    [Serializable]
    public class KissException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public KissException ( ) { }
        public KissException ( string message ) : base ( message ) { }
        public KissException ( string message, Exception inner ) : base ( message, inner ) { }
        public KissException ( string format, params object[ ] objs ) : base ( string.Format ( format, objs ) ) { }
        protected KissException (
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context )
            : base ( info, context ) { }
    }
}
