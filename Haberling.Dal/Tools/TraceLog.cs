using Sagede.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberling.Dal
{
    /// <summary>
    /// Stellt eine, für diese Assembly,
    /// exclusive Instanz des Loggers zur Verfügung.
    /// </summary>
    public static class TraceLog
    {
        private static ILogger _logger;
        private static readonly object LockObject = new object();

        /// <summary>
        /// Liefert die Instanz des Loggers.
        /// </summary>
        internal static ILogger Logger
        {
            get
            {
                lock (LockObject)
                {
                    return _logger ?? (_logger = LogManager.GetLogger("Weko", "Haberling.Importer"));
                }
            }
        }

        /// <summary>
        /// Loggt Debug-Informationen im TraceLog-Manager
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void LogVerbose(string message, params object[] args)
        {
            //Logger.LogVerboseFormat(message, args);
            Logger.Verbose(string.Format(message, args));
        }

        /// <summary>
        /// Loggt Exceptions im Tracelog-Manager
        /// </summary>
        /// <param name="ex"></param>
        public static void LogException(Exception ex)
        {
            Logger.Error(String.Format("{0},{1},{2}", ex.Message, Environment.NewLine, ex.StackTrace));
        }

        /// <summary>
        /// Loggt Exceptions im Tracelog-Manager
        /// </summary>
        /// <param name="message">Meldung</param>
        /// <param name="args">Arguemnte</param>
        public static void LogException(string message, params string[] args)
        {
            Logger.LogError(message, args);
        }

        /// <summary>
        /// Loggt die Dauer von Aufrufen
        /// </summary>
        public static void LogTime(string measurement, long start)
        {
            Logger.LogTime(measurement, start);
        }
    }
}
