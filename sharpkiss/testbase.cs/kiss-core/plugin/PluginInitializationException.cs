using System;

namespace Kiss.Plugin
{
    [Serializable]
    public class PluginInitException : KissException
    {
        public PluginInitException(string message, Exception[] innerExceptions)
            : base(message, innerExceptions[0])
        {
            this.innerExceptions = innerExceptions;
        }

        private Exception[] innerExceptions;

        public Exception[] InnerExceptions
        {
            get { return innerExceptions; }
            set { innerExceptions = value; }
        }
    }
}
