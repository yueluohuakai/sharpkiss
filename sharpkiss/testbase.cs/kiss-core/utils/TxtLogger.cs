#region File Comment
//+-------------------------------------------------------------------+
//+ FileName: 	    TxtLogger.cs
//+ File Created:   20090729
//+-------------------------------------------------------------------+
//+ Purpose:        提供记录日志到文件的功能
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 20090729        ZHLI Comment Created. 
//                  对不同程序集的调用，记录到不同的文件
//+-------------------------------------------------------------------+
//+ 20090811        zhli 添加appname配置，用于分离日志文件在不同的文件夹
//+-------------------------------------------------------------------+
//+ 20090827        zhli 增加public static void Append ( string format, params object[ ] objs )方法
//+-------------------------------------------------------------------+
//+ 20090914        zhli fix a bug
//+-------------------------------------------------------------------+
#endregion

using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Kiss.Utils
{
    /// <summary>
    /// 提供记录日志到文件的功能
    /// </summary>
    public static class TxtLogger
    {
        #region Fields

        private const int FILESIZELIMIT = 10485760; // The log file can not exceed 10MB.

        private static string LogDirectory
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logsfile");
            }
        }

        private static readonly object _syncLock = new object();

        #endregion

        #region Ctor

        static TxtLogger()
        {
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 向Log文件添加记录
        /// </summary>
        public static void Append(object obj)
        {
            if (obj == null) return;

            string filename = Path.Combine(LogDirectory,
                Assembly.GetCallingAssembly().ManifestModule.Name + ".log");

            AppendStringToTextFile(filename,
                obj.ToString());
        }

        /// <summary>
        /// 向Log文件添加记录
        /// </summary>
        public static void Append(string format, object[] objs)
        {
            string filename = Path.Combine(LogDirectory,
                Assembly.GetCallingAssembly().ManifestModule.Name + ".log");

            AppendStringToTextFile(filename,
                string.Format(format, objs));
        }

        /// <summary>
        /// Dump Exception
        /// </summary>
        /// <param name="theException"></param>
        public static void DumpException(Exception theException)
        {
            string filename = Path.Combine(LogDirectory,
                Assembly.GetCallingAssembly().ManifestModule.Name + ".log");

            StringBuilder txt = new StringBuilder();
            txt.AppendLine("Exception Dumping Started");
            txt.AppendLine(ExceptionUtil.WriteException(theException));
            txt.AppendLine("Exception Dumping Finished");

            AppendStringToTextFile(filename, txt.ToString());
        }

        #endregion

        #region Helper

        /// <summary>
        /// 检查文件是否超过10M
        /// </summary>
        private static void CheckFile(string filename)
        {
            if (!File.Exists(filename))
                return;

            FileInfo fi = new FileInfo(filename);

            if (fi.Length > FILESIZELIMIT)
            {
                string strBakFileName = filename + ".bak";
                if (File.Exists(strBakFileName))
                    File.Delete(strBakFileName);

                File.Move(filename, strBakFileName);
            }
        }

        private static void AppendStringToTextFile(string filename, string strContent)
        {
            lock (_syncLock)
            {
                try
                {
                    CheckFile(filename);

                    string strTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'' HH:mm:ss:fffffff ");
                    strContent = strTime + strContent;
                    using (StreamWriter writer = new StreamWriter(filename, true))
                    {
                        writer.WriteLine(strContent);
                    }
                }
                catch
                {
                }
            }
        }
        #endregion
    }
}