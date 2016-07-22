using System;

namespace Kiss.Config
{
    /// <summary>
    /// 配置异常类
    /// </summary>
    [Serializable]
    public class ConfigException : KissException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ConfigException ( ) { }
        public ConfigException ( string message ) : base ( message ) { }
        public ConfigException ( string message, Exception inner ) : base ( message, inner ) { }
        protected ConfigException (
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context )
            : base ( info, context ) { }
    }
}
