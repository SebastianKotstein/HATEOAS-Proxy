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
    public class Arguments
    {

        /// <summary>
        /// Parses the passed input arguments and returns an Argument object containing the parsed arguments in a key-value format.
        /// Note that every key of the input arguments must be unique (otherwise an <see cref="Exception"/> is thrown).
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Arguments Parse(string[] args)
        {
            IDictionary<string, string> parsedArgs = new Dictionary<string, string>();
            foreach (string arg in args)
            {
                if (arg.StartsWith("#")) //ignore comment
                {
                    continue;
                }

                string[] split = arg.Split('=');

                if (parsedArgs.ContainsKey(split[0]))
                {
                    throw new Exception("Argument exception: The key '" + split[0] + "' is not unique");
                }
                else
                {
                    if (split.Length > 1)
                    {
                        parsedArgs.Add(split[0], split[1]);
                    }
                    else
                    {
                        parsedArgs.Add(split[0], "");
                    }
                }
            }
            return new Arguments(parsedArgs);
        }

        private IDictionary<string, string> _parsendArguments;

        public IDictionary<string, string> ParsendArguments
        {
            get
            {
                return _parsendArguments;
            }
        }

        protected Arguments(IDictionary<string, string> parsedArguments)
        {
            _parsendArguments = parsedArguments;
        }

        /// <summary>
        /// Returns true if the specified key exists, else false.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Has(string key)
        {
            return _parsendArguments.ContainsKey(key);
        }

        /// <summary>
        /// Returns the value belonging to the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            return _parsendArguments[key];
        }


    }
}
