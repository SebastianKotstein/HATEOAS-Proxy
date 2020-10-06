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
using skotstein.research.rest.apiguru.loader.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skotstein.net.http.reachabilityanalyzer.bulk.tools
{
    public class CustomizedArguments : Arguments
    { 

        public const string ARG_SOURCE_PATH = "source.path";

        //public const string ARG_SOURCE_API_KEY = "source.api.key";
        //public const string ARG_SOURCE_API_VALUE = "source.api.value";
        //public const string ARG_SOURCE_API_VERSION_KEY = "source.api.version.key";
        //public const string ARG_SOURCE_API_VERSION_VALUE = "source.api.version.value";

        public const string ARG_FILTER_MATCH_TYPE_EXACT = "filter.matchtype.exact";
        public const string ARG_FILTER_MATCH_TYPE_PROPERTY_CONTAINS_PARAMETER = "filter.matchtype.prop_contains_para";
        public const string ARG_FILTER_MATCH_TYPE_PARAMETER_CONTAINS_PROPERTY = "filter.matchtype.para_contains_prop";

        public const string ARG_PREFERRED_VERSION_ONLY = "source.api.version.preferred";

        public static CustomizedArguments Parse(string[] args)
        {
            return new CustomizedArguments(Arguments.Parse(args).ParsendArguments);
        }

        private CustomizedArguments(IDictionary<string, string> parsedArguments) : base(parsedArguments)
        {

        }

        public override string ToString()
        {
            string s = "";
            foreach (KeyValuePair<string, string> arg in ParsendArguments)
            {
                s += arg.Key + "=" + arg.Value + "\n";
            }
            return s;
        }
    }
}
