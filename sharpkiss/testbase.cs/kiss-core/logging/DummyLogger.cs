using Kiss.Utils;
using System;

namespace Kiss.Logging
{
    class DummyLogger : ILogger
    {
        #region ILogger Members

        public void Debug(string message)
        {
#if(DEBUG)
            TxtLogger.Append(message);
#endif
        }

        public void Debug(string message, Exception exception)
        {
#if(DEBUG)
            TxtLogger.Append(message);
            TxtLogger.DumpException(exception);
#endif
        }

        public void Debug(string format, params object[] args)
        {
#if(DEBUG)
            TxtLogger.Append(format, args);
#endif
        }

        public void DebugFormat(string format, params object[] args)
        {
#if(DEBUG)
            TxtLogger.Append(format, args);
#endif
        }

        public void DebugFormat(Exception exception, string format, params object[] args)
        {
#if(DEBUG)
            TxtLogger.Append(format, args);
            TxtLogger.DumpException(exception);
#endif
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
#if(DEBUG)
            TxtLogger.Append(format, args);
#endif
        }

        public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
#if(DEBUG)
            TxtLogger.Append(format, args);
            TxtLogger.DumpException(exception);
#endif
        }

        public bool IsDebugEnabled
        {
            get
            {
#if(DEBUG)
                return true;
#else
                return false;
#endif
            }
        }

        public void Info(string message)
        {
            TxtLogger.Append(message);
        }

        public void Info(string message, Exception exception)
        {
            TxtLogger.Append(message);
            TxtLogger.DumpException(exception);
        }

        public void Info(string format, params object[] args)
        {
            TxtLogger.Append(format, args);
        }

        public void InfoFormat(string format, params object[] args)
        {
            TxtLogger.Append(format, args);
        }

        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            TxtLogger.Append(format, args);
            TxtLogger.DumpException(exception);
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            TxtLogger.Append(format, args);
        }

        public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            TxtLogger.Append(format, args);
            TxtLogger.DumpException(exception);
        }

        public bool IsInfoEnabled
        {
            get { return true; }
        }

        public void Warn(string message)
        {
            TxtLogger.Append(message);
        }

        public void Warn(string message, Exception exception)
        {
            TxtLogger.Append(message);
            TxtLogger.DumpException(exception);
        }

        public void Warn(string format, params object[] args)
        {
            TxtLogger.Append(format, args);
        }

        public void WarnFormat(string format, params object[] args)
        {
            TxtLogger.Append(format, args);
        }

        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            TxtLogger.Append(format, args);
            TxtLogger.DumpException(exception);
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            TxtLogger.Append(format, args);
        }

        public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            TxtLogger.Append(format, args);
            TxtLogger.DumpException(exception);
        }

        public bool IsWarnEnabled
        {
            get { return true; }
        }

        public void Error(string message)
        {
            TxtLogger.Append(message);
        }

        public void Error(string message, Exception exception)
        {
            TxtLogger.Append(message);
            TxtLogger.DumpException(exception);
        }

        public void Error(string format, params object[] args)
        {
            TxtLogger.Append(format, args);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            TxtLogger.Append(format, args);
        }

        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            TxtLogger.Append(format, args);
            TxtLogger.DumpException(exception);
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            TxtLogger.Append(format, args);
        }

        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            TxtLogger.Append(format, args);
            TxtLogger.DumpException(exception);
        }

        public bool IsErrorEnabled
        {
            get { return true; }
        }

        public void Fatal(string message)
        {
            TxtLogger.Append(message);
        }

        public void Fatal(string message, Exception exception)
        {
            TxtLogger.Append(message);
            TxtLogger.DumpException(exception);
        }

        public void Fatal(string format, params object[] args)
        {
            TxtLogger.Append(format, args);
        }

        public void FatalFormat(string format, params object[] args)
        {
            TxtLogger.Append(format, args);
        }

        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            TxtLogger.Append(format, args);
            TxtLogger.DumpException(exception);
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            TxtLogger.Append(format, args);
        }

        public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            TxtLogger.Append(format, args);
            TxtLogger.DumpException(exception);
        }

        public bool IsFatalEnabled
        {
            get { return true; }
        }

        public ILogger CreateChildLogger(string loggerName)
        {
            return new DummyLogger();
        }

        #endregion
    }
}
