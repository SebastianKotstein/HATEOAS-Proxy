// MIT License
//
// Copyright (c) 2020 Sebastian Kotstein
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skotstein.net.http.reachabilityanalyzer.tools
{
    public class Log
    {

        private static IList<LogWriter> _logWriters = new List<LogWriter>();

        private static int _infoCounter = 0;
        private static int _debugCounter = 0;
        private static int _warningCounter = 0;
        private static int _errorCounter = 0;

        public static IList<LogWriter> LogWriters
        {
            get
            {
                return _logWriters;
            }
        }

        public static int InfoCounter
        {
            get
            {
                return _infoCounter;
            }

        }

        public static int DebugCounter
        {
            get
            {
                return _debugCounter;
            }
        }

        public static int WarningCounter
        {
            get
            {
                return _warningCounter;
            }
        }

        public static int ErrorCounter
        {
            get
            {
                return _errorCounter;
            }
        }

        public static void ResetCounter()
        {
            Log._debugCounter = 0;
            Log._infoCounter = 0;
            Log._warningCounter = 0;
            Log._errorCounter = 0;
        }

        /// <summary>
        /// Adds a debug message to the <see cref="LogWriter"/>s having <see cref="LogLevel.debug"/> enabled.
        /// </summary>
        /// <param name="tag">tag</param>
        /// <param name="message">message</param>
        public static void Debug(string tag, string message)
        {
            _debugCounter++;
            foreach (LogWriter logWriter in _logWriters)
            {
                if (logWriter.LogLevel == LogLevel.debug)
                {
                    logWriter.Debug(tag, message);
                }
            }
        }

        /// <summary>
        /// Adds a info message to the <see cref="LogWriter"/>s having <see cref="LogLevel.debug"/> or <see cref="LogLevel.info"/> enabled.
        /// </summary>
        /// <param name="tag">tag</param>
        /// <param name="message">message</param>
        public static void Info(string tag, string message)
        {
            _infoCounter++;
            foreach (LogWriter logWriter in _logWriters)
            {
                if (logWriter.LogLevel == LogLevel.debug
                    || logWriter.LogLevel == LogLevel.info)
                {
                    logWriter.Info(tag, message);
                }
            }
        }

        /// <summary>
        /// Adds a warning to the <see cref="LogWriter"/>s having <see cref="LogLevel.debug"/>, <see cref="LogLevel.info"/> or <see cref="LogLevel.warning"/> enabled.
        /// </summary>
        /// <param name="tag">tag</param>
        /// <param name="message">warning</param>
        public static void Warning(string tag, string message)
        {
            _warningCounter++;
            foreach (LogWriter logWriter in _logWriters)
            {
                if (logWriter.LogLevel == LogLevel.debug
                    || logWriter.LogLevel == LogLevel.info
                    || logWriter.LogLevel == LogLevel.warning)
                {
                    logWriter.Warning(tag, message);
                }
            }
        }

        /// <summary>
        /// Adds a error message to the <see cref="LogWriter"/>s having <see cref="LogLevel.debug"/>, <see cref="LogLevel.info"/>, <see cref="LogLevel.warning"/> or <see cref="LogLevel.error"/> enabled.
        /// </summary>
        /// <param name="tag">tag</param>
        /// <param name="message">error</param>
        public static void Error(string tag, string message)
        {
            _errorCounter++;
            foreach (LogWriter logWriter in _logWriters)
            {
                if (logWriter.LogLevel == LogLevel.debug
                    || logWriter.LogLevel == LogLevel.error
                    || logWriter.LogLevel == LogLevel.warning
                    || logWriter.LogLevel == LogLevel.info)
                {
                    logWriter.Error(tag, message);
                }
            }
        }
    }
}
