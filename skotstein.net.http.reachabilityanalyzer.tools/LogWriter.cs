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
    public abstract class LogWriter
    {
        public const string INFO_TAG = "[INFO]";
        public const string WARNING_TAG = "[WARNING]";
        public const string ERROR_TAG = "[ERROR]";
        public const string DEBUG_TAG = "[DEBUG]";

        private LogLevel _logLever;

        public LogLevel LogLevel
        {
            get
            {
                return _logLever;
            }

            set
            {
                _logLever = value;
            }
        }

        protected string DateNow
        {
            get
            {
                return DateTime.Now.ToString(String.Format("yyyy-MM-dd.HH:mm:ss.FFF"));
            }
        }

        /// <summary>
        /// Adds a debug message
        /// </summary>
        /// <param name="tag">tag</param>
        /// <param name="message">message</param>
        public abstract void Debug(string tag, string message);

        /// <summary>
        /// Adds a info message
        /// </summary>
        /// <param name="tag">tag</param>
        /// <param name="message">message</param>
        public abstract void Info(string tag, string message);


        /// <summary>
        /// Adds a warning
        /// </summary>
        /// <param name="tag">tag</param>
        /// <param name="message">warning</param>
        public abstract void Warning(string tag, string message);

        /// <summary>
        /// Adds a error message
        /// </summary>
        /// <param name="tag">tag</param>
        /// <param name="message">error</param>
        public abstract void Error(string tag, string message);

    }

    public enum LogLevel
    {
        none, //no messages are logged
        error, //only errors are logged
        warning, //only errors and warnings are logged
        info, //only errors, warnings and info messages are logged
        debug //all messages are logged
    }
}
