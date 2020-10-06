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
using skotstein.net.http.reachabilityanalyzer.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skotstein.net.http.reachabilityanalyzer.proxy.tools
{
    public class CustomizedArguments : Arguments
    {
        public const string ARG_SOURCE = "doc_path"; //mandatory
        public const string ARG_PORT = "port"; //mandatory

        public const string ARG_LOG_LEVEL = "log_level";
        public const string OPT_LOG_LEVEL_ERROR = "error";
        public const string OPT_LOG_LEVEL_WARNING = "warning";
        public const string OPT_LOG_LEVEL_INFO = "info";
        public const string OPT_LOG_LEVEL_DEBUG = "debug";
        public const string OPT_LOG_LEVEL_NONE = "none"; //DEFAULT

        public const string ARG_LOG_OUT = "log_out";
        public const string OPT_LOG_OUT_CONSOLE = "console"; //DEFAULT
        public const string OPT_LOG_OUT_FILE = "file";

        public const string ARG_LOG_PATH = "log_path";

        public const string ARG_API_BASE_PATH = "api_base_path";

        public const string ARG_INJECTION_PREFIX = "inj_prefix";

        public const string ARG_ACCESS_TOKEN = "access_token";

        public const string ARG_EXTENDED_HYPERLINK = "hyperlink_with_debug_information";

        public const string ARG_PREFLIGHT_HEAD_COUNTER = "preflight_head_counter";

        public static CustomizedArguments Parse(string[] args)
        {
            return new CustomizedArguments(Arguments.Parse(args).ParsendArguments);
        }

        private CustomizedArguments(IDictionary<string, string> parsedArguments) : base(parsedArguments)
        {

        }

        /// <summary>
        /// Gets the specified <see cref="LogLevel"/>. If no <see cref="LogLevel"/> is specified, the default value <see cref="LogLevel.none"/> is returned.
        /// </summary>
        public LogLevel LogLevel
        {
            get
            {
                if (Has(ARG_LOG_LEVEL))
                {
                    if (Get(ARG_LOG_LEVEL).CompareTo(OPT_LOG_LEVEL_ERROR) == 0)
                    {
                        return LogLevel.error;
                    }
                    else if (Get(ARG_LOG_LEVEL).CompareTo(OPT_LOG_LEVEL_WARNING) == 0)
                    {
                        return LogLevel.warning;
                    }
                    else if (Get(ARG_LOG_LEVEL).CompareTo(OPT_LOG_LEVEL_INFO) == 0)
                    {
                        return LogLevel.info;
                    }
                    else if (Get(ARG_LOG_LEVEL).CompareTo(OPT_LOG_LEVEL_DEBUG) == 0)
                    {
                        return LogLevel.debug;
                    }
                    else
                    {
                        return LogLevel.none;
                    }
                }
                else
                {
                    return LogLevel.none;
                }
            }
        }

        /// <summary>
        /// Gets the path where the log file should be stored
        /// </summary>
        public string LogPath
        {
            get
            {
                if (Has(ARG_LOG_PATH))
                {
                    return Get(ARG_LOG_PATH);
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
