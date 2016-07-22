#region File Comment
//+-------------------------------------------------------------------+
//+ FileName: 	    TxtLogger.cs
//+ File Created:   20090729
//+-------------------------------------------------------------------+
//+ Purpose:        �ṩ��¼��־���ļ��Ĺ���
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 20090729        ZHLI Comment Created. 
//                  �Բ�ͬ���򼯵ĵ��ã���¼����ͬ���ļ�
//+-------------------------------------------------------------------+
//+ 20090811        zhli ���appname���ã����ڷ�����־�ļ��ڲ�ͬ���ļ���
//+-------------------------------------------------------------------+
//+ 20090827        zhli ����public static void Append ( string format, params object[ ] objs )����
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
    /// �ṩ��¼��־���ļ��Ĺ���
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
        /// ��Log�ļ���Ӽ�¼
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
        /// ��Log�ļ���Ӽ�¼
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
        /// ����ļ��Ƿ񳬹�10M
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