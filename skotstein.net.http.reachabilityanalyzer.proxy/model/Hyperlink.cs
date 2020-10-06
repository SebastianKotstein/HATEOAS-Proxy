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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skotstein.net.http.reachabilityanalyzer.proxy.model
{
    /// <summary>
    /// This class represents a sub resource containing a hyperlink to another resource. An instance of this class can be serialized and deserialized into/from JSON.
    /// </summary>
    public class Hyperlink
    {
        private string _rel;
        private string _href;
        private int? _debug_statusCode;
        private string _debug_msg;

        [JsonProperty("rel")]
        public string Rel
        {
            get
            {
                return _rel;
            }

            set
            {
                _rel = value;
            }
        }

        [JsonProperty("href")]
        public string Href
        {
            get
            {
                return _href;
            }

            set
            {
                _href = value;
            }
        }

        [JsonProperty("debug_statusCode", NullValueHandling = NullValueHandling.Ignore)]
        public int? Debug_statusCode
        {
            get
            {
                return _debug_statusCode;
            }

            set
            {
                _debug_statusCode = value;
            }
        }

        [JsonProperty("debug_msg", NullValueHandling =NullValueHandling.Ignore)]
        public string Debug_msg
        {
            get
            {
                return _debug_msg;
            }

            set
            {
                _debug_msg = value;
            }
        }
    }
}
