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
using Microsoft.OpenApi.Models;
using skotstein.net.http.urimodel;
using skotstein.net.http.urimodel.openapi;

namespace skotstein.net.http.reachabilityanalyzer
{
    public class DefaultLinkAnalyzer : ILinkAnalyzer
    {
        private bool _isNameCaseInvariant = true;
        private bool _allowExactMatch = true;
        private bool _allowPropertyNameContainsParameterName = true;
        private bool _allowParameterNameContainsPropertyName = true;

        private string _statusCodeFilter = "2";
        private IList<string> _mediaTypeFilter = new List<string>();
        private IList<HttpMethod> _operationTypeFilter = new List<HttpMethod>();

        private int _missingMediaCounter = 0;

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

        public string ResponseCodeFilter
        {
            get
            {
                return _statusCodeFilter; 
            }

            set
            {
                _statusCodeFilter = value;
            }
        }

        public IList<string> MediaTypeFilter
        {
            get
            {
                return _mediaTypeFilter;
            }
        }

        public IList<HttpMethod> OperationTypesFilter
        {
            get
            {
                return _operationTypeFilter;
            }
        }

        public int MissingMediaCounter
        {
            get
            {
                return _missingMediaCounter;
            }

            set
            {
                _missingMediaCounter = value;
            }
        }

        public IList<Link> AnalyzeResource(PathSegment pathSegment, PathParameter pathParameter)
        {
            IList<Link> links = new List<Link>();

            //iterate over all operations
            foreach(Operation operation in pathSegment.Operations)
            {
                IList<Link> linksOfIteration = AnalyzeOperation((OApiOperation)operation, pathParameter);
                foreach(Link link in linksOfIteration)
                {
                    links.Add(link);
                }
            }
            return links;
        }

        public IList<Link> AnalyzeOperation(OApiOperation operation, PathParameter pathParameter)
        {
            if(OperationTypesFilter.Count == 0 || OperationTypesFilter.Contains(operation.Method))
            {
                IList<Link> links = new List<Link>();

                //iterate over all response
                foreach(KeyValuePair<string,OpenApiResponse> openApiRespose in operation.OpenApiOperation.Responses)
                {
                    IList<Link> linksOfIteration = AnalyzeResponse(openApiRespose.Key, openApiRespose.Value, pathParameter,operation.Method);
                    foreach(Link link in linksOfIteration)
                    {
                        link.Operation = operation;
                        links.Add(link);
                    }
                }
                return links;
            }
            return new List<Link>();
        }

        public IList<Link> AnalyzeResponse(string statusCode, OpenApiResponse response, PathParameter pathParameter, HttpMethod method)
        {
            if(String.IsNullOrWhiteSpace(ResponseCodeFilter) || statusCode.StartsWith(ResponseCodeFilter))
            {
                if(method == HttpMethod.GET && response.Content.Count == 0)
                {
                    MissingMediaCounter++;
                }

                IList<Link> links = new List<Link>();

                //iterate over all media types
                foreach(KeyValuePair<string, OpenApiMediaType> mediaType in response.Content)
                {
                    IList<Link> linksOfIteration = AnalyzeMediaType(mediaType.Key, mediaType.Value, pathParameter);
                    foreach(Link link in linksOfIteration)
                    {
                        link.StatusCode = statusCode;
                        links.Add(link);
                    }
                }
                return links;
            }
            return new List<Link>();
        }


        public IList<Link> AnalyzeMediaType(string mediaTypeName, OpenApiMediaType mediaType, PathParameter pathParameter)
        {
            if (CheckMediaType(mediaTypeName))
            {
                OpenApiSchema schema = mediaType.Schema;
                IList<Link> links = AnalyzeSchema(schema, pathParameter);
                foreach(Link link in links)
                {
                    link.MediaType = mediaTypeName;
                }
                return links;

            }
            return new List<Link>();
        }


        public IList<Link> AnalyzeSchema(OpenApiSchema schema, PathParameter pathParameter)
        {
            IList<Link> links = new List<Link>();
            AnalyzeSchema(schema, false, "$", new HashSet<OpenApiSchema>(), links, pathParameter);
            return links;
        }

        /// <summary>
        /// Analyzes the passed <see cref="OpenApiSchema"/> for properties whose names match the passed <see cref="PathParameter"/> name. The results are stored as <see cref="Link"/> objects in the passed result list.
        /// Moreover, this method requires the current position in the schema, which is documented as XPath, a flag that indicates whether the result is within an array structure, and a set that contains all <see cref="OpenApiSchema"/>s that have already been analyzed in order to avoid loops.
        /// Note that this method is called recursively.
        /// </summary>
        /// <param name="pathParameterName"></param>
        /// <param name="schema"></param>
        /// <param name="isInArray"></param>
        /// <param name="xPath"></param>
        /// <param name="alreadyAnalyzedSchemas"></param>
        /// <param name="results"></param>
        private void AnalyzeSchema(OpenApiSchema schema, bool isInArray, string xPath, ISet<OpenApiSchema> alreadyAnalyzedSchemas, IList<Link> results, PathParameter pathParameter)
        {
            string pathParameterName = pathParameter.ParameterName;

            //Create a copy of the Set of already analyzed schemas. Thus, only current path within the structure is memorized:
            ISet<OpenApiSchema> alreadyAnalyzedSchemasCopy = new HashSet<OpenApiSchema>(alreadyAnalyzedSchemas);
            //ISet<OpenApiSchema> alreadyAnalyzedSchemasCopy = alreadyAnalyzedSchemas;

            if (alreadyAnalyzedSchemasCopy.Contains(schema) || schema == null)
            {
                return;
            }
            alreadyAnalyzedSchemasCopy.Add(schema);

            //analyze items
            if (schema.Items != null)
            {
                AnalyzeSchema(schema.Items, isInArray, xPath, alreadyAnalyzedSchemasCopy, results, pathParameter);
            }

            //analyze linked schemas type of 'AllOf'
            if (schema.AllOf != null)
            {
                foreach (OpenApiSchema linkedSchema in schema.AllOf)
                {
                    AnalyzeSchema(linkedSchema, isInArray, xPath, alreadyAnalyzedSchemasCopy, results, pathParameter);
                }
            }

            //analyze linked schemas type of 'AnyOf'
            if (schema.AnyOf != null)
            {
                foreach (OpenApiSchema linkedSchema in schema.AnyOf)
                {
                    AnalyzeSchema(linkedSchema, isInArray, xPath, alreadyAnalyzedSchemasCopy, results, pathParameter);
                }
            }

            //analyze linked schemas type of 'OneOf'
            if (schema.OneOf != null)
            {
                foreach (OpenApiSchema linkedSchema in schema.OneOf)
                {
                    AnalyzeSchema(linkedSchema, isInArray, xPath, alreadyAnalyzedSchemasCopy, results, pathParameter);
                }
            }

            //analyze properties of schema
            if (schema.Properties != null)
            {
                foreach (KeyValuePair<string, OpenApiSchema> prop in schema.Properties)
                {
                    string name = prop.Key;
                    OpenApiSchema property = prop.Value;

                    //filter type
                    if (String.IsNullOrWhiteSpace(property.Type) || CompareTo(property.Type, new string[] { "string", "number", "integer", "boolean" }))
                    {
                        string modifiedPathParameterName = pathParameterName;
                        if (IsNameCaseInvariant)
                        {
                            name = name.ToLower();
                            modifiedPathParameterName = pathParameterName.ToLower();
                        }

                        //check for exact match (if enabled)
                        if (AllowExactMatch && name.CompareTo(modifiedPathParameterName) == 0)
                        {
                            Link link = new Link();
                            link.MatchType = LinkMatchType.exactMatch;
                            link.XPath = xPath + "." + prop.Key;
                            link.ContainsMultipleValues = isInArray;
                            link.PathParameter = pathParameter;
                            results.Add(link);
                        }

                        //check whether property name contains parameter name (if enabled)
                        else if (AllowPropertyNameContainsParameterName && name.Contains(modifiedPathParameterName))
                        {
                            Link link = new Link();
                            link.MatchType = LinkMatchType.propertyNameContainsPathParameterName;
                            link.XPath = xPath + "." + prop.Key;
                            link.ContainsMultipleValues = isInArray;
                            link.PathParameter = pathParameter;
                            results.Add(link);
                        }

                        //check whether parameter name contains property name (if enabled)
                        else if (AllowParameterNameContainsPropertyName && modifiedPathParameterName.Contains(name))
                        {
                            Link link = new Link();
                            link.MatchType = LinkMatchType.pathParameterNameContainsPropertyName;
                            link.XPath = xPath + "." + prop.Key;
                            link.ContainsMultipleValues = isInArray;
                            link.PathParameter = pathParameter;
                            results.Add(link);
                        }
                    }
                    else if (property.Type.CompareTo("array") == 0)
                    {
                        AnalyzeSchema(property, true, xPath + "." + name + "[*]", alreadyAnalyzedSchemasCopy, results, pathParameter);
                    }
                    else //property.Type == "object"
                    {
                        AnalyzeSchema(property, isInArray, xPath + "." + name, alreadyAnalyzedSchemasCopy, results, pathParameter);
                    }
                }
            }
        }

        /// <summary>
        /// Method returns true, if the passed string s matches at least one string of the passed array 'values'.
        /// The method uses <see cref="string.CompareTo(string)"/> for comparison.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool CompareTo(string s, string[] values)
        {
            foreach (string v in values)
            {
                if (s.CompareTo(v) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return true if the passed media-type name matches the pre-configured media-type filter, else false.
        /// </summary>
        /// <param name="mediaTypeName"></param>
        /// <returns></returns>
        private bool CheckMediaType(string mediaTypeName)
        {
            if (MediaTypeFilter.Count == 0)
            {
                return true;
            }
            else
            {
                foreach (string mediaType in MediaTypeFilter)
                {
                    if (mediaTypeName.Contains(mediaType))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
