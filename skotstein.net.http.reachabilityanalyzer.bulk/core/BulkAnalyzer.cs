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

namespace skotstein.net.http.reachabilityanalyzer.bulk.core
{
    public class BulkAnalyzer
    {

        private ConsoleOut _out = new ConsoleOut();
        private string _projectFolderPath;

        private bool _isNameCaseInvariant = true;
        private bool _allowExactMatch = true;
        private bool _allowPropertyNameContainsParameterName = true;
        private bool _allowParameterNameContainsPropertyName = true;

        public ConsoleOut Out
        {
            get
            {
                return _out;
            }
        }

        public string ProjectFolderPath
        {
            get
            {
                return _projectFolderPath;
            }

            set
            {
                _projectFolderPath = value;
            }
        }

        public bool IsNameCaseInvariant
        {
            get
            {
                return _isNameCaseInvariant;
            }

            set
            {
                _isNameCaseInvariant = value;
            }
        }

        public bool AllowExactMatch
        {
            get
            {
                return _allowExactMatch;
            }

            set
            {
                _allowExactMatch = value;
            }
        }

        public bool AllowPropertyNameContainsParameterName
        {
            get
            {
                return _allowPropertyNameContainsParameterName;
            }

            set
            {
                _allowPropertyNameContainsParameterName = value;
            }
        }

        public bool AllowParameterNameContainsPropertyName
        {
            get
            {
                return _allowParameterNameContainsPropertyName;
            }

            set
            {
                _allowParameterNameContainsPropertyName = value;
            }
        }

        public void StartBulkAnalysis(IList<Alias> apis, bool preferredOnly)
        {
            bool csvHeaderAdded = false;
            string startDate = DateTime.Now.ToString(String.Format("yyyy-MM-dd_HH.mm.ss.FFF"));

            Out.PrintHeaderBulkAnalysisStart(startDate);

            foreach (Alias api in apis)
            {
                //load API info
                ApiGuruApi apiInfo = GetApiInfo(api.Key, ProjectFolderPath);
                if(apiInfo == null)
                {
                    continue;
                }

                //load all versions
                IList<Alias> versions = GetVersionAlias(api.Key, ProjectFolderPath);
                if(versions == null)
                {
                    continue;
                }

                foreach(Alias version in versions)
                {
                    if(!preferredOnly || (preferredOnly && apiInfo.Preferred.CompareTo(version.Value) == 0))
                    {
                        //load version Info
                        ApiGuruApiVersion versionInfo = apiInfo.Versions.Versions[version.Value];

                        //determine documentation path
                        string rawContent = LoadOpenApiFile(api.Key, version.Key, ProjectFolderPath);

                        //start analysis
                        AnalysisResult analysisResult = AnalyzeSingleApi(rawContent, api.Key, api.Value, version.Key, version.Value, apiInfo.Added, versionInfo.Added, LinkAnalyzerFactory.Create(IsNameCaseInvariant,AllowExactMatch,AllowPropertyNameContainsParameterName,AllowParameterNameContainsPropertyName));
                        Out.PrintResult(analysisResult);

                        //append results to file
                        string csvContent = "";
                        if (!csvHeaderAdded)
                        {
                            csvContent += analysisResult.CsvHeader;
                            csvHeaderAdded = true;
                        }
                        csvContent += analysisResult.ToCsv;
                        File.AppendAllText(ProjectFolderPath + @"\" + startDate + ".csv", csvContent);
                    }
                }

            }
            Out.PrintHeaderBulkAnalysisFinish(DateTime.Now.ToString(String.Format("yyyy-MM-dd_HH.mm.ss.FFF")));
        }

        public AnalysisResult AnalyzeSingleApi(string rawContent, string apiKey, string apiName, string versionKey, string versionName, string apiAdded, string versionAdded, ILinkAnalyzer linkAnalyzer)
        { 

            OpenApiDiagnostic openApiDiagnostic = new OpenApiDiagnostic();
            OpenApiDocument openApiDocument = null;
            try
            { 
                //step 1: convert OpenAPI documentation into object model
                openApiDocument = new OpenApiStringReader().Read(rawContent, out openApiDiagnostic);
            }
            catch (Exception e)
            {
                // possibly IO Exception or Exception due to mal-formed OpenAPI documentation
                return AnalysisResult.Create(apiKey, apiName, versionKey, versionName, apiAdded, versionAdded, null, 0, 0, false, e);
            }
            
            //step 2: write diagnostic to log
            int openApiReaderErrors = 0;
            foreach (OpenApiError openApiError in openApiDiagnostic.Errors)
            {
                openApiReaderErrors++;
                //TODO: write to log
            }

            //step 3: create URI Model
            UriModel uriModel = null;
            try
            {
                uriModel = CustomizedUriModelFactory.Instance.Create(openApiDocument);
            }
            catch(Exception e)
            {
                return AnalysisResult.Create(apiKey, apiName, versionKey, versionName, apiAdded, versionAdded, null, openApiReaderErrors, 0, false, e);
            }

            //step 4: check whether URI Model contains variable path segments that have more than one path parameter
            bool hasUriModelVariablePathSegmentsWithMoreThanOnePathParameter = HasUriModelVariablePathSegmentsWithMoreThanOnePathParameter(uriModel);
            if (hasUriModelVariablePathSegmentsWithMoreThanOnePathParameter)
            {
                return AnalysisResult.Create(apiKey, apiName, versionKey, versionName, apiAdded, versionAdded, uriModel, openApiReaderErrors, 0, true, null);
            }

            //step 5: identify reachability paths
            int missingMediaCounter = 0;
            try
            {
                IList<PathSegment> pathSegmentsRepresentingResources = uriModel.Root.QuerySubTree.HasOperations().Results;
                for (int i = 0; i < pathSegmentsRepresentingResources.Count; i++)
                {
                    ((CustomizedPathSegment)pathSegmentsRepresentingResources[i]).IdentifyReachabilityPaths(linkAnalyzer);
                    Out.UpdateStatusBar(((double)i / (double)pathSegmentsRepresentingResources.Count)*100);
                }

                missingMediaCounter = linkAnalyzer.MissingMediaCounter;

            }
            catch(Exception e)
            {
                //possibly Exception due to mal-formed OpenAPI documentation
                return AnalysisResult.Create(apiKey, apiName, versionKey, versionName, apiAdded, versionAdded, uriModel, openApiReaderErrors, 0, hasUriModelVariablePathSegmentsWithMoreThanOnePathParameter, e);
            }

            return AnalysisResult.Create(apiKey, apiName, versionKey, versionName, apiAdded, versionAdded, uriModel, openApiReaderErrors, missingMediaCounter, hasUriModelVariablePathSegmentsWithMoreThanOnePathParameter, null);
        }

        /// <summary>
        /// Returns true, if the passed <see cref="UriModel"/> contains variable path segments that have more than one path parameter.
        /// </summary>
        /// <param name="uriModel"></param>
        /// <returns></returns>
        private bool HasUriModelVariablePathSegmentsWithMoreThanOnePathParameter(UriModel uriModel)
        {
            IList<PathSegment> variablePathSegments = uriModel.Root.QuerySubTree.IsVariable().Results;
            foreach(PathSegment variablePathSegment in variablePathSegments)
            {
                if(variablePathSegment.PathParameters.Count > 1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the list of <see cref="Alias"/> stored in the API folder.
        /// </summary>
        private IList<Alias> GetVersionAlias(string apiKey, string projectFolderPath)
        {
            try
            {
                return Alias.ReadList(File.ReadAllText(projectFolderPath + @"\" + apiKey + @"\alias.json"));
            }
            catch (Exception e)
            {
                Out.PrintLine("Cannot open " + projectFolderPath + @"\" + apiKey + @"\alias.json, Exception: " + e.Message, ConsoleColor.Red);
                return null;
            }
        }

        private ApiGuruApi GetApiInfo(string apiKey, string projectFolderPath)
        {
            try
            {
                return ApiGuruApi.LoadFromDisk(projectFolderPath + @"\" + apiKey + @"\info.json");
            }
            catch (Exception e)
            {
                Out.PrintLine("Cannot open " + projectFolderPath + @"\" + apiKey + @"\info.json, Exception: " + e.Message, ConsoleColor.Red);
                return null;
            }
        }

        /// <summary>
        /// Loads the OpenAPI file (.json is preferred)
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="versionKey"></param>
        /// <returns></returns>
        private string LoadOpenApiFile(string apiKey, string versionKey, string projectFolderPath)
        {
            if (File.Exists(projectFolderPath + @"\" + apiKey + @"\" + versionKey + @"\swagger.json"))
            {
                return File.ReadAllText(projectFolderPath + @"\" + apiKey + @"\" + versionKey + @"\swagger.json");
            }
            else if (File.Exists(projectFolderPath + @"\" + apiKey + @"\" + versionKey + @"\swagger.yaml"))
            {
                return File.ReadAllText(projectFolderPath + @"\" + apiKey + @"\" + versionKey + @"\swagger.yaml");
            }
            else
            {
                Out.PrintLine("Cannot open OpenAPI Documentation in " + projectFolderPath + @"\" + apiKey + @"\" + versionKey, ConsoleColor.Red);
                return null;
            }
        }
    }
}
