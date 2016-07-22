using System;
using Kiss.Logging;

namespace Kiss
{
    /// <summary>
    /// Use the LogManager's <see cref="GetLogger(string)"/> or <see cref="GetLogger(System.Type)"/> 
    /// methods to obtain <see cref="ILogger"/> instances for logging.
    /// </summary>
    /// <example>
    /// The example below shows the typical use of LogManager to obtain a reference to a logger
    /// and log an exception:
    /// <code>
    /// 
    /// ILog log = LogManager.GetLogger(this.GetType());
    /// ...
    /// try 
    /// { 
    ///   /* .... */ 
    /// }
    /// catch(Exception ex)
    /// {
    ///   log.ErrorFormat("Hi {0}", ex, "dude");
    /// }
    /// 
    /// </code>
    /// </example>
    /// <seealso cref="ILogger"/>
    /// <seealso cref="ILoggerFactory"/>
    public static class LogManager
    {
        /// <summary>
        /// Gets the logger by calling <see cref="ILoggerFactory.Create(Type)"/>
        /// </summary>
        /// <returns>the logger instance obtained from the current <see cref="ILoggerFactory"/></returns>
        public static ILogger GetLogger<T>()
        {
            ILoggerFactory f = Factory;
            if (f != null)
                return f.Create(typeof(T));

            return new DummyLogger();
        }

        /// <summary>
        /// Gets the logger by calling <see cref="ILoggerFactory.Create(Type)"/>        
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>the logger instance obtained from the current <see cref="ILoggerFactory"/></returns>
        public static ILogger GetLogger(Type type)
        {
            ILoggerFactory f = Factory;
            if (f != null)
                return f.Create(type);

            return new DummyLogger();
        }

        /// <summary>
        /// Gets the logger by calling <see cref="ILoggerFactory.Create(string)"/>        
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>the logger instance obtained from the current <see cref="ILoggerFactory"/></returns>
        public static ILogger GetLogger(string name)
        {
            ILoggerFactory f = Factory;
            if (f != null)
                return f.Create(name);
            return new DummyLogger();
        }

        /// <summary>
        /// get logging factory
        /// </summary>
        static ILoggerFactory Factory
        {
            get
            {
                return ServiceLocator.Instance.SafeResolve<ILoggerFactory>();
            }
        }
    }
}
