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
using skotstein.net.http.urimodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skotstein.net.http.reachabilityanalyzer
{
    public class LinkAnalyzerFactory
    { 
        public static ILinkAnalyzer Create()
        {
            return Create(true, true, true, true);
        }

        public static ILinkAnalyzer Create(bool isNameCaseInvariant, bool allowExactMatch, bool allowPropertyNameContainsParameterName, bool allowParameterNameContainsPropertyName)
        {
            return Create(isNameCaseInvariant, allowExactMatch, allowPropertyNameContainsParameterName, allowParameterNameContainsPropertyName,"2",new List<string>(),new List<HttpMethod>());
        }

        public static ILinkAnalyzer Create(bool isNameCaseInvariant, bool allowExactMatch, bool allowPropertyNameContainsParameterName, bool allowParameterNameContainsPropertyName, string responseCodeFilter, IList<string> mediaTypeFilter, IList<HttpMethod> operationTypesFilter)
        {
            ILinkAnalyzer linkAnalyzer = new DefaultLinkAnalyzer();
            linkAnalyzer.IsNameCaseInvariant = isNameCaseInvariant;
            linkAnalyzer.AllowExactMatch = allowExactMatch;
            linkAnalyzer.AllowPropertyNameContainsParameterName = allowPropertyNameContainsParameterName;
            linkAnalyzer.AllowParameterNameContainsPropertyName = allowParameterNameContainsPropertyName;
            linkAnalyzer.ResponseCodeFilter = responseCodeFilter;
            foreach(string mediaType in mediaTypeFilter)
            {
                linkAnalyzer.MediaTypeFilter.Add(mediaType);
            }
            foreach(HttpMethod httpMethod in operationTypesFilter)
            {
                linkAnalyzer.OperationTypesFilter.Add(httpMethod);
            }
            return linkAnalyzer;
        }
    }
}
