using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace FarmHouseRedone
{
    public static class Logger
    {
        public static IMonitor monitor;
        public static bool debugMode;

        /// <summary>Logs a message to the console and error log.</summary>
        public static void Log(string log, LogLevel level = LogLevel.Debug)
        {
            if (level == LogLevel.Debug && !debugMode)
                return;
            if (monitor == null)
                return;
            monitor.Log(log, level);
        }
    }
}
