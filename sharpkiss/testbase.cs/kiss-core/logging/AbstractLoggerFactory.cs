using System;
using System.IO;
using Kiss.Utils;

namespace Kiss.Logging
{
    /// <summary>
    /// 抽象日志factory
    /// </summary>
    [Serializable]
    public abstract class AbstractLoggerFactory : MarshalByRefObject, ILoggerFactory
    {
        public virtual ILogger Create(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return Create(type.FullName);
        }

        public abstract ILogger Create(String name);

        /// <summary>
        /// Gets the configuration file.
        /// </summary>
        /// <param name="fileName">i.e. log4net.config</param>
        /// <returns></returns>
        protected static FileInfo GetConfigFile(string fileName)
        {
            FileInfo result;

            if (Path.IsPathRooted(fileName))
            {
                result = new FileInfo(fileName);
            }
            else
            {
                result = new FileInfo(ServerUtil.MapPath(fileName));
            }

            return result;
        }
    }
}
