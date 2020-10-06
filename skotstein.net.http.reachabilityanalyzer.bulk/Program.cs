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
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using skotstein.net.http.reachabilityanalyzer.bulk.core;
using skotstein.net.http.reachabilityanalyzer.bulk.model;
using skotstein.net.http.reachabilityanalyzer.bulk.tools;
using skotstein.net.http.reachabilityanalyzer.tools;
using skotstein.net.http.urimodel;
using skotstein.research.rest.apiguru.loader.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skotstein.net.http.reachabilityanalyzer.bulk
{
    class Program
    { 

        public static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            BulkAnalyzer bulkAnalyzer = new BulkAnalyzer();
            CustomizedArguments arguments = CustomizedArguments.Parse(args);

            //step 1: load settings
            if (arguments.Has(CustomizedArguments.ARG_FILTER_MATCH_TYPE_EXACT) ||
                arguments.Has(CustomizedArguments.ARG_FILTER_MATCH_TYPE_PARAMETER_CONTAINS_PROPERTY) ||
                arguments.Has(CustomizedArguments.ARG_FILTER_MATCH_TYPE_PROPERTY_CONTAINS_PARAMETER))
            {
                bulkAnalyzer.IsNameCaseInvariant = true;
                bulkAnalyzer.AllowExactMatch = arguments.Has(CustomizedArguments.ARG_FILTER_MATCH_TYPE_EXACT);
                bulkAnalyzer.AllowParameterNameContainsPropertyName = arguments.Has(CustomizedArguments.ARG_FILTER_MATCH_TYPE_PARAMETER_CONTAINS_PROPERTY);
                bulkAnalyzer.AllowPropertyNameContainsParameterName = arguments.Has(CustomizedArguments.ARG_FILTER_MATCH_TYPE_PROPERTY_CONTAINS_PARAMETER);
            }
            else
            {
                bulkAnalyzer.IsNameCaseInvariant = true;
                bulkAnalyzer.AllowExactMatch = true;
                bulkAnalyzer.AllowParameterNameContainsPropertyName = true;
                bulkAnalyzer.AllowPropertyNameContainsParameterName = true;
            }
            bulkAnalyzer.ProjectFolderPath = arguments.Get(CustomizedArguments.ARG_SOURCE_PATH);
            bool preferredVersionsOnly = arguments.Has(CustomizedArguments.ARG_PREFERRED_VERSION_ONLY);

            //step 2: print settings
            Console.WriteLine("Settings: ");
            Console.WriteLine("Project folder: " + bulkAnalyzer.ProjectFolderPath);
            Console.WriteLine("Preferred API versions only: " + preferredVersionsOnly);
            Console.WriteLine("Case invariant: " + bulkAnalyzer.IsNameCaseInvariant);
            Console.WriteLine("Allow exact match: " + bulkAnalyzer.AllowExactMatch);
            Console.WriteLine("Allow Property name contains Path Parameter name: " + bulkAnalyzer.AllowPropertyNameContainsParameterName);
            Console.WriteLine("Allow Path Parameter name contains Property name: " + bulkAnalyzer.AllowParameterNameContainsPropertyName);

            //step 3: start analysis
            bulkAnalyzer.StartBulkAnalysis(Alias.ReadList(File.ReadAllText(arguments.Get(CustomizedArguments.ARG_SOURCE_PATH) + @"\alias.json")),preferredVersionsOnly);
            Console.ReadLine();

        }
    }
}
