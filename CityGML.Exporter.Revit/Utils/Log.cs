using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.IO;
using System.Linq;

namespace IndoorGML.Exporter.Revit.Utils
{
    public static class Log
    {

        private static log4net.ILog log;

        public static void Setup(string logFile, string logName)
        {

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            if (log == null)
            {
                PatternLayout patternLayout = new PatternLayout();
                patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
                patternLayout.ActivateOptions();

                RollingFileAppender roller = new RollingFileAppender();
                roller.AppendToFile = false;
                roller.File = logFile;
                roller.Layout = patternLayout;
                roller.MaxSizeRollBackups = 5;
                roller.MaximumFileSize = "1GB";
                roller.RollingStyle = RollingFileAppender.RollingMode.Size;
                roller.StaticLogFileName = true;
                roller.Encoding = System.Text.Encoding.UTF8;
                //roller.ActivateOptions();
                hierarchy.Root.AddAppender(roller);

                MemoryAppender memory = new MemoryAppender();
                memory.ActivateOptions();
                hierarchy.Root.AddAppender(memory);

                hierarchy.Root.Level = Level.Info;
                hierarchy.Configured = true;
                //roller.ActivateOptions();
                
                log = log4net.LogManager.GetLogger(logName);
            }
            else
            {
                var appender = hierarchy.GetAppenders().Where(x => x is RollingFileAppender).FirstOrDefault();
                if(appender != null )
                {
                    var fileLogAppender = appender as RollingFileAppender;
                    if (fileLogAppender.File != logFile)
                    {
                        fileLogAppender.File = logFile;
                        fileLogAppender.Encoding = System.Text.Encoding.UTF8;
                        fileLogAppender.ActivateOptions();
                        log = log4net.LogManager.GetLogger(logName);
                    }
                }
            }
        }

        public static void Info(string msg)
        {
            try
            {
                if (log == null)
                {
                    Setup(Path.GetTempFileName(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace);
                }
                else
                {
                    //log?.Info(msg);
                }
            }
            catch { }
        }

        public static void Error(string msg, Exception ex)
        {
            if (log == null)
            {
                Setup(Path.GetTempFileName(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace);
            }
            log.Error(msg, ex);
        }

        public static void Warning(string msg, Exception ex)
        {
            if (log == null)
            {
                Setup(Path.GetTempFileName(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace);
            }
            log.Warn(msg, ex);
        }

        internal static void Flush()
        {
            if(log != null)
            {
                
                Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();                
                foreach(var appender in hierarchy.GetAppenders())
                {
                    if(appender is RollingFileAppender file)
                    {
                        file.Writer.Flush();
                    }
                }
            }
        }
    }
}
