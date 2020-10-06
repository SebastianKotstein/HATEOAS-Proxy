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

namespace skotstein.net.http.reachabilityanalyzer.bulk.model
{
    public class AnalysisResult
    {
        private static int _itemCounter;

        public const string PARAMETER_COUNTER = "counter";
        public const string PARAMETER_API_KEY = "api.key";
        public const string PARAMETER_API_NAME = "api.name";
        public const string PARAMETER_API_ADDED = "api.added";
        public const string PARAMETER_API_VERSION_KEY = "api.version.key";
        public const string PARAMETER_API_VERSION_NAME = "api.version.name";
        public const string PARAMETER_API_VERSION_ADDED = "api.version.added";

        public const string PARAMETER_NUM_RESOURCES = "api.version.#resources";
        public const string PARAMETER_NUM_READ_ONLY_RESOURCES = "api.version.#readOnlyResources";
        public const string PARAMETER_NUM_POST = "api.version.#POST";
        public const string PARAMETER_NUM_PUT = "api.version.#PUT";
        public const string PARAMETER_NUM_DELETE = "api.version.#DELETE";
        public const string PARAMETER_NUM_PATCH = "api.version.#PATCH";
        public const string PARAMETER_NUM_ROOTS = "api.version.#roots";
        public const string PARAMETER_HEIGHT = "api.version.height";
        public const string PARAMETER_NUM_PATH_SEGMENTS = "api.version.#pathSegments";
        public const string PARAMETER_NUM_VARIABLE_PATH_SEGMENTS = "api.version.#variablePathSegments";
        public const string PARAMETER_NUM_PATH_PARAMETERS = "api.version.#pathParameters";
        public const string PARAMETER_NUM_DIFFERENT_PATH_PARAMETER_NAMES = "api.version.#differentPathParameterNames";
        public const string PARAMETER_NUM_UNIQUE_PATH_PARAMETER_NAMES = "api.version.#uniquePathParameterNames";
        public const string PARAMETER_SHARE_UNIQUE_PATH_PARAMETER_NAMES = "api.version.%uniquePathParameterNames";

        public const string PARAMETER_NUM_PATH_PARAMETERS_WITH_LINKS = "api.version.#pathParametersWithLinks";
        public const string PARAMETER_SHARE_PATH_PARAMETERS_WITH_LINKS = "api.version.%pathParametersWithLinks";
        public const string PARAMETER_AVG_LINKS_PER_PARAMETER = "api.version.AVG:linksPerPathParameter";
        public const string PARAMETER_NUM_LINKS = "api.version.#links";
        public const string PARAMETER_NUM_LINKS_WITH_EXACT_MATCH = "api.version.links.#exactMatch";
        public const string PARAMETER_NUM_LINKS_WITH_PROPERTY_NAME_CONTAINS_PATH_PARAMETER_NAME = "api.version.links.#propertyNameContainsPathParameterName";
        public const string PARAMETER_NUM_LINKS_WITH_PATH_PARAMETER_CONTAINS_PROPERTY_NAME = "api.version.links.#pathParameterNameContainsPropertyName";
        public const string PARAMETER_SHARE_LINKS_WITH_EXACT_MATCH = "api.version.links.%exactMatch";
        public const string PARAMETER_SHARE_LINKS_WITH_PROPERTY_NAME_CONTAINS_PATH_PARAMETER_NAME = "api.version.links.%propertyNameContainsPathParameterName";
        public const string PARAMETER_SHARE_LINKS_WITH_PATH_PARAMETER_CONTAINS_PROPERTY_NAME = "api.version.links.%pathParameterNameContainsPropertyName";
        public const string PARAMETER_NUM_REACHABLE_RESOURCES_WITH_EXACT_MATCH = "api.version.links.#ReachableResourcesWithExactMatch";
        public const string PARAMETER_SHARE_REACHABLE_RESOURCES_WITH_EXACT_MATCH = "api.version.links.%ReachableResourcesWithExactMatch";
        public const string PARAMETER_NUM_REACHABLE_RESOURCES = "api.version.links.#ReachableResources";
        public const string PARAMETER_SHARE_REACHABLE_RESOURCES = "api.version.links.%ReachableResources";

        public const string PARAMETER_BOOL_HAS_MULTIPLE_PATH_PARAMETER_PER_VARIABLE_PATH_SEGMENT = "api.version.?HasMultiplePathParameterPerVariablePathSegment";
        public const string PARAMETER_NUM_OPEN_API_PARSING_ERROR = "api.version.#OpenApiReaderParsingErrors";
        public const string PARAMETER_NUM_MISSING_MEDIA_TYPES = "api.version.#MissingMediaTypes";
        public const string PARAMETER_EXCEPTION = "api.version.exception";

        private double _reachabilityShare;

        public static int ItemCounter
        {
            get
            {
                return _itemCounter;
            }
        }

        public double ReachabilityShare
        {
            get
            {
                return _reachabilityShare;
            }
            set
            {
                _reachabilityShare = value;
            }
        }



        private IDictionary<string, string> _parameters = new Dictionary<string, string>();
        private IList<string> _order = new List<string>();

        public AnalysisResult()
        {
            Set(PARAMETER_COUNTER, "");
            Set(PARAMETER_API_KEY, "");
            Set(PARAMETER_API_NAME, "");
            Set(PARAMETER_API_ADDED, "");
            Set(PARAMETER_API_VERSION_KEY, "");
            Set(PARAMETER_API_VERSION_NAME, "");
            Set(PARAMETER_API_VERSION_ADDED, "");

            Set(PARAMETER_NUM_RESOURCES, "");
            Set(PARAMETER_NUM_READ_ONLY_RESOURCES, "");
            Set(PARAMETER_NUM_POST, "");
            Set(PARAMETER_NUM_PUT, "");
            Set(PARAMETER_NUM_DELETE, "");
            Set(PARAMETER_NUM_PATCH, "");
            Set(PARAMETER_NUM_ROOTS, "");
            Set(PARAMETER_HEIGHT, "");
            Set(PARAMETER_NUM_PATH_SEGMENTS, "");
            Set(PARAMETER_NUM_VARIABLE_PATH_SEGMENTS, "");
            Set(PARAMETER_NUM_PATH_PARAMETERS, "");
            Set(PARAMETER_NUM_DIFFERENT_PATH_PARAMETER_NAMES, "");
            Set(PARAMETER_NUM_UNIQUE_PATH_PARAMETER_NAMES, "");
            Set(PARAMETER_SHARE_UNIQUE_PATH_PARAMETER_NAMES, "");

            Set(PARAMETER_NUM_PATH_PARAMETERS_WITH_LINKS, "");
            Set(PARAMETER_SHARE_PATH_PARAMETERS_WITH_LINKS, "");
            Set(PARAMETER_AVG_LINKS_PER_PARAMETER, "");
            Set(PARAMETER_NUM_LINKS, "");
            Set(PARAMETER_NUM_LINKS_WITH_EXACT_MATCH, "");
            Set(PARAMETER_NUM_LINKS_WITH_PROPERTY_NAME_CONTAINS_PATH_PARAMETER_NAME, "");
            Set(PARAMETER_NUM_LINKS_WITH_PATH_PARAMETER_CONTAINS_PROPERTY_NAME, "");
            Set(PARAMETER_SHARE_LINKS_WITH_EXACT_MATCH, "");
            Set(PARAMETER_SHARE_LINKS_WITH_PROPERTY_NAME_CONTAINS_PATH_PARAMETER_NAME, "");
            Set(PARAMETER_SHARE_LINKS_WITH_PATH_PARAMETER_CONTAINS_PROPERTY_NAME, "");
            Set(PARAMETER_NUM_REACHABLE_RESOURCES_WITH_EXACT_MATCH, "");
            Set(PARAMETER_SHARE_REACHABLE_RESOURCES_WITH_EXACT_MATCH, "");
            Set(PARAMETER_NUM_REACHABLE_RESOURCES, "");
            Set(PARAMETER_SHARE_REACHABLE_RESOURCES, "");

            Set(PARAMETER_BOOL_HAS_MULTIPLE_PATH_PARAMETER_PER_VARIABLE_PATH_SEGMENT, "");
            Set(PARAMETER_NUM_OPEN_API_PARSING_ERROR,"");
            Set(PARAMETER_NUM_MISSING_MEDIA_TYPES, "");
            Set(PARAMETER_EXCEPTION,"");
        }

        public static AnalysisResult Create(string apiKey, string apiName, string versionKey, string versionName, string apiAdded, string versionAdded, UriModel uriModel, int openApiReaderParsingErrors, int missingMediaCounter, bool hasMultiplePathParameterPerVariablePathSegment, Exception otherException)
        {
            AnalysisResult result = new AnalysisResult();

            if (apiKey.CompareTo("146") == 0)
            {
                int a = 45;
            }

            #region API META INFORMATION

            result.Set(PARAMETER_COUNTER, ItemCounter + "");
            _itemCounter++;
            result.Set(PARAMETER_API_KEY, apiKey);
            result.Set(PARAMETER_API_NAME, apiName);
            result.Set(PARAMETER_API_ADDED, apiAdded);
            result.Set(PARAMETER_API_VERSION_KEY, versionKey);
            result.Set(PARAMETER_API_VERSION_NAME, versionName);
            result.Set(PARAMETER_API_VERSION_ADDED, versionAdded);

            #endregion

            if (otherException == null)
            {
                #region API URI/RESOURCE STRUCTURE

                /******************************************** api.version.#resources *****************************************/
                int numberOfResources = uriModel.Root.QuerySubTree.HasOperations().Results.Count;
                result.Set(PARAMETER_NUM_RESOURCES, numberOfResources + "");

                /******************************************** api.version.#readOnlyResources *****************************************/
                int readOnlyResources = uriModel.Root.QuerySubTree.And(
                    QueryFilter.HasOperation(HttpMethod.GET),
                    QueryFilter.Not(QueryFilter.HasOperation(HttpMethod.POST)),
                    QueryFilter.Not(QueryFilter.HasOperation(HttpMethod.PUT)),
                    QueryFilter.Not(QueryFilter.HasOperation(HttpMethod.DELETE)),
                    QueryFilter.Not(QueryFilter.HasOperation(HttpMethod.PATCH))).Results.Count;
                result.Set(PARAMETER_NUM_READ_ONLY_RESOURCES, readOnlyResources + "");

                /******************************************** api.version.#POST *****************************************/
                result.Set(PARAMETER_NUM_POST, uriModel.Root.QuerySubTree.HasOperation(HttpMethod.POST).Results.Count + "");

                /******************************************** api.version.#PUT *****************************************/
                result.Set(PARAMETER_NUM_PUT, uriModel.Root.QuerySubTree.HasOperation(HttpMethod.PUT).Results.Count + "");

                /******************************************** api.version.#DELETE *****************************************/
                result.Set(PARAMETER_NUM_DELETE, uriModel.Root.QuerySubTree.HasOperation(HttpMethod.DELETE).Results.Count + "");

                /******************************************** api.version.#PATCH *****************************************/
                result.Set(PARAMETER_NUM_PATCH, uriModel.Root.QuerySubTree.HasOperation(HttpMethod.PATCH).Results.Count + "");

                /******************************************** api.version.#roots *****************************************/
                int roots = 0;
                if (uriModel.Root.HasOperations)
                {
                    roots = 1;
                }
                else
                {
                    roots = uriModel.Root.NextDescendantsWithOperations.Count;
                }
                result.Set(PARAMETER_NUM_ROOTS, roots + "");

                /******************************************** api.version.height *****************************************/
                //without root
                result.Set(PARAMETER_HEIGHT, (uriModel.Root.Height - 1) + "");

                /******************************************** api.version.#Segments *****************************************/
                //without root
                result.Set(PARAMETER_NUM_PATH_SEGMENTS, (uriModel.Root.QuerySubTree.Results.Count - 1) + "");

                /******************************************** api.version.#variableSegments *****************************************/
                result.Set(PARAMETER_NUM_VARIABLE_PATH_SEGMENTS, uriModel.Root.QuerySubTree.IsVariable().Results.Count + "");

                /******************************************** api.version.#pathParameters *****************************************/
                int numberOfPathParameters = 0;
                foreach (PathSegment pathSegment in uriModel.Root.QuerySubTree.IsVariable().Results)
                {
                    numberOfPathParameters += pathSegment.PathParameters.Count;
                }
                result.Set(PARAMETER_NUM_PATH_PARAMETERS, numberOfPathParameters + "");

                /******************************************** api.version.#differentPathParameterNames *****************************************/
                ISet<string> differentPathParameterNames = new HashSet<string>();
                foreach (PathSegment pathSegment in uriModel.Root.QuerySubTree.IsVariable().Results)
                {
                    foreach (PathParameter pathParameter in pathSegment.PathParameters)
                    {
                        if (!differentPathParameterNames.Contains(pathParameter.ParameterName.ToLower()))
                        {
                            differentPathParameterNames.Add(pathParameter.ParameterName.ToLower());
                        }
                    }
                }
                result.Set(PARAMETER_NUM_DIFFERENT_PATH_PARAMETER_NAMES, differentPathParameterNames.Count + "");

                /******************************************** api.version.#uniquePathParameters *****************************************/
                int uniquePathParameter = 0;
                foreach (PathSegment pathSegment in uriModel.Root.QuerySubTree.IsVariable().Results)
                {
                    foreach (PathParameter pathParameter in pathSegment.PathParameters)
                    {
                        int counter = 0;

                        foreach (PathSegment ps in uriModel.Root.QuerySubTree.IsVariable().Results)
                        {
                            foreach (PathParameter pp in ps.PathParameters)
                            {
                                if (pp.ParameterName.ToLower().CompareTo(pathParameter.ParameterName.ToLower()) == 0)
                                {
                                    counter++;
                                }
                            }
                        }
                        if (counter <= 1) //there could be one match, which is the path parameter name compared with itself
                        {
                            uniquePathParameter++;
                        }
                    }
                }
                result.Set(PARAMETER_NUM_UNIQUE_PATH_PARAMETER_NAMES, uniquePathParameter + "");

                /******************************************** api.version.%uniquePathParameters *****************************************/
                if (numberOfPathParameters > 0)
                {
                    result.Set(PARAMETER_SHARE_UNIQUE_PATH_PARAMETER_NAMES, ((double)uniquePathParameter / (double)numberOfPathParameters) + "");
                }
                else
                {
                    result.Set(PARAMETER_SHARE_UNIQUE_PATH_PARAMETER_NAMES, "-1");
                }
                #endregion

               
                if (!hasMultiplePathParameterPerVariablePathSegment)
                {
                    #region ANALYSIS RESULTS

                    /******************************************** api.version.#pathParameterWithLinks *************************************/
                    int numberOfPathParametersWithLinks = 0;
                    foreach (PathSegment pathSegment in uriModel.Root.QuerySubTree.IsVariable().Results)
                    {
                        foreach (PathParameter pathParameter in pathSegment.PathParameters)
                        {
                            if (((CustomizedPathParameter)pathParameter).Links.Count > 0)
                            {
                                numberOfPathParametersWithLinks++;
                            }
                        }
                    }
                    result.Set(PARAMETER_NUM_PATH_PARAMETERS_WITH_LINKS, numberOfPathParametersWithLinks + "");

                    /******************************************** api.version.%pathParameterWithLinks ****************************/
                    if (numberOfPathParameters > 0)
                    {
                        result.Set(PARAMETER_SHARE_PATH_PARAMETERS_WITH_LINKS, ((double)numberOfPathParametersWithLinks / (double)numberOfPathParameters) + "");
                    }
                    else
                    {
                        result.Set(PARAMETER_SHARE_PATH_PARAMETERS_WITH_LINKS, "-1");
                    }

                    /******************************************** api.version.AVG:linksPerPathParameter ****************************/
                    IList<PathParameter> pathParameterWithLinks = new List<PathParameter>();
                    IList<Link> allLinks = new List<Link>();

                    foreach (PathSegment pathSegment in uriModel.Root.QuerySubTree.IsVariable().Results)
                    {
                        foreach (PathParameter pathParameter in pathSegment.PathParameters)
                        {
                            if (((CustomizedPathParameter)pathParameter).Links.Count > 0)
                            {
                                pathParameterWithLinks.Add(pathParameter);
                                foreach (Link link in ((CustomizedPathParameter)pathParameter).Links)
                                {
                                    allLinks.Add(link);
                                }
                            }
                        }
                    }
                    if (pathParameterWithLinks.Count > 0)
                    {
                        result.Set(PARAMETER_AVG_LINKS_PER_PARAMETER, ((double)allLinks.Count / (double)pathParameterWithLinks.Count) + "");
                    }
                    else
                    {
                        result.Set(PARAMETER_AVG_LINKS_PER_PARAMETER, "-1");
                    }



                    /******************************************** api.version.#links ****************************/
                    result.Set(PARAMETER_NUM_LINKS, allLinks.Count + "");


                    //match type:
                    int exactMatchCounter = 0;
                    int propertyNameContainsPathParameterName = 0;
                    int pathParameterNameContainsPropertyName = 0;

                    foreach (Link link in allLinks)
                    {
                        switch (link.MatchType)
                        {
                            case LinkMatchType.exactMatch:
                                exactMatchCounter++;
                                break;
                            case LinkMatchType.propertyNameContainsPathParameterName:
                                propertyNameContainsPathParameterName++;
                                break;
                            case LinkMatchType.pathParameterNameContainsPropertyName:
                                pathParameterNameContainsPropertyName++;
                                break;
                        }
                    }

                    /******************************************** api.version.links.#exactMatch ****************************/
                    result.Set(PARAMETER_NUM_LINKS_WITH_EXACT_MATCH, exactMatchCounter + "");

                    /********************************************* api.version.links.#propertyNameContainsPathParameterName *********/
                    result.Set(PARAMETER_NUM_LINKS_WITH_PROPERTY_NAME_CONTAINS_PATH_PARAMETER_NAME, propertyNameContainsPathParameterName + "");

                    /********************************************* api.version.links.#pathParameterNameContainsPropertyName *********/
                    result.Set(PARAMETER_NUM_LINKS_WITH_PATH_PARAMETER_CONTAINS_PROPERTY_NAME, pathParameterNameContainsPropertyName + "");

                    if (allLinks.Count > 0)
                    {
                        /******************************************** api.version.links.%exactMatch ****************************/
                        result.Set(PARAMETER_SHARE_LINKS_WITH_EXACT_MATCH, ((double)exactMatchCounter / (double)allLinks.Count) + "");

                        /********************************************* api.version.links.%keyContainsPropertyKeyCounter *********/
                        result.Set(PARAMETER_SHARE_LINKS_WITH_PROPERTY_NAME_CONTAINS_PATH_PARAMETER_NAME, ((double)propertyNameContainsPathParameterName / (double)allLinks.Count) + "");

                        /********************************************* api.version.links.%propertyKeyContainsKeyCounter *********/
                        result.Set(PARAMETER_SHARE_LINKS_WITH_PATH_PARAMETER_CONTAINS_PROPERTY_NAME, ((double)pathParameterNameContainsPropertyName / (double)allLinks.Count) + "");
                    }
                    else
                    {
                        result.Set(PARAMETER_SHARE_LINKS_WITH_EXACT_MATCH, "-1");
                        result.Set(PARAMETER_SHARE_LINKS_WITH_PROPERTY_NAME_CONTAINS_PATH_PARAMETER_NAME, "-1");
                        result.Set(PARAMETER_SHARE_LINKS_WITH_PATH_PARAMETER_CONTAINS_PROPERTY_NAME, "-1");
                    }

                    /********************************************* api.version.links.#ReachableResourcesWithExactMatch*********/
                    if (apiKey.CompareTo("146") == 0)
                    {
                        int a = 45;
                    }

                    ISet<PathSegment> reachedPathSegments = new HashSet<PathSegment>();
                    IList<PathSegment> apiStartPathSegments = new List<PathSegment>();
                    if (uriModel.Root.HasOperations)
                    {
                        apiStartPathSegments.Add(uriModel.Root);
                    }
                    else
                    {
                        apiStartPathSegments = ((CustomizedPathSegment)uriModel.Root).NextDescendantsRepresentingOneResource;
                    }
                    foreach(PathSegment pathSegment in apiStartPathSegments)
                    {
                        FollowReachabilityAssociations(pathSegment, reachedPathSegments,"2", new List<HttpMethod>() { HttpMethod.GET}, new List<LinkMatchType> { LinkMatchType.exactMatch });
                    }
                    int numberOfReachedPathSegments = GetNumberOfPathSegmentsRepresentingResources(reachedPathSegments);
                    result.Set(PARAMETER_NUM_REACHABLE_RESOURCES_WITH_EXACT_MATCH, numberOfReachedPathSegments + "");

                    /********************************************* api.version.links.%ReachableResourcesWithExactMatch*********/
                    result.Set(PARAMETER_SHARE_REACHABLE_RESOURCES_WITH_EXACT_MATCH, ((double)numberOfReachedPathSegments / (double)numberOfResources) + "");

                    /********************************************* api.version.links.#ReachableResources*********/
                    reachedPathSegments.Clear();
                    foreach (PathSegment pathSegment in apiStartPathSegments)
                    {
                        FollowReachabilityAssociations(pathSegment, reachedPathSegments, "2", new List<HttpMethod>() { HttpMethod.GET }, new List<LinkMatchType>());
                    }
                    numberOfReachedPathSegments = GetNumberOfPathSegmentsRepresentingResources(reachedPathSegments);
                    result.Set(PARAMETER_NUM_REACHABLE_RESOURCES, numberOfReachedPathSegments + "");

                    /********************************************* api.version.links.%ReachableResourcesWithExactMatch*********/
                    result.ReachabilityShare = (double)numberOfReachedPathSegments / (double)numberOfResources;
                    result.Set(PARAMETER_SHARE_REACHABLE_RESOURCES, result.ReachabilityShare + "");


                    #endregion
                }

            }

            #region ERRORS AND WARNINGS

            /********************************************* api.version.links.?HasMultiplePathParameterPerVariablePathSegment *********/
            result.Set(PARAMETER_BOOL_HAS_MULTIPLE_PATH_PARAMETER_PER_VARIABLE_PATH_SEGMENT, hasMultiplePathParameterPerVariablePathSegment ? "TRUE" : "FALSE");

            /********************************************* api.version.links.#OpenApiReaderParsingErrors *********/
            result.Set(PARAMETER_NUM_OPEN_API_PARSING_ERROR, openApiReaderParsingErrors + "");

            /********************************************* api.version.links.#MissingMediaCounter *********/
            result.Set(PARAMETER_NUM_MISSING_MEDIA_TYPES, missingMediaCounter + "");

            /********************************************* api.version.exception *********/
            result.Set(PARAMETER_EXCEPTION, otherException != null?otherException.Message:"");

            #endregion

            return result;
        }

        /// <summary>
        /// Follows all <see cref="ReachabilityPath"/>s of the passed <see cref="PathSegment"/> and adds the passed <see cref="PathSegment"/> to the passed <see cref="ISet{T}"/> of already traversed <see cref="PathSegment"/>s.
        /// The method aborts immediately (i.e. nothing happens), if the passed <see cref="PathSegment"/> has already been traversed. Moreover, it is possible to filter <see cref="ReachabilityPath"/>s type of <see cref="ReachabilityPathType.oneToMany"/>.
        /// </summary>
        /// <param name="pathSegment"></param>
        /// <param name="alreadyReachedPathSegments"></param>
        /// <param name="statusCodeFilter"></param>
        /// <param name="operationTypeFilter"></param>
        /// <param name="linkMatchTypeFilter"></param>
        public static void FollowReachabilityAssociations(PathSegment pathSegment, ISet<PathSegment> alreadyReachedPathSegments, string statusCodeFilter, IList<HttpMethod> operationTypeFilter, IList<LinkMatchType> linkMatchTypeFilter)
        {
            if (!alreadyReachedPathSegments.Contains(pathSegment))
            {
                alreadyReachedPathSegments.Add(pathSegment);
            }
            else
            {
                //abort, if already traversed
                return;
            }

            foreach (ReachabilityPath reachabilityAssociation in ((CustomizedPathSegment)pathSegment).ReachabilityPaths)
            {
                if (reachabilityAssociation.Type == ReachabilityPathType.oneToMany)
                {
                    //one-to-many
                    bool atLeastOneLinkMatchesFilter = false;
                    foreach (Link link in reachabilityAssociation.Links)
                    {
                        if (String.IsNullOrWhiteSpace(statusCodeFilter) || link.StatusCode.StartsWith(statusCodeFilter))
                        {
                            if (operationTypeFilter.Count == 0 || operationTypeFilter.Contains(link.Operation.Method))
                            {
                                if (linkMatchTypeFilter.Count == 0 || linkMatchTypeFilter.Contains(link.MatchType))
                                {
                                    atLeastOneLinkMatchesFilter = true;
                                }
                            }
                        }
                    }
                    if (atLeastOneLinkMatchesFilter)
                    {
                        FollowReachabilityAssociations(reachabilityAssociation.Target, alreadyReachedPathSegments, statusCodeFilter, operationTypeFilter, linkMatchTypeFilter);
                    }
                }
                else
                {
                    //one-to-one
                    FollowReachabilityAssociations(reachabilityAssociation.Target, alreadyReachedPathSegments, statusCodeFilter, operationTypeFilter, linkMatchTypeFilter);
                }
            }
        }

        private static int GetNumberOfPathSegmentsRepresentingResources(ISet<PathSegment> pathSegments)
        {
            int counter = 0;
            foreach (PathSegment pathSegment in pathSegments)
            {
                if (pathSegment.HasOperations)
                {
                    counter++;
                }
            }
            return counter;
        }

        public void Set(string key, string value)
        {
            if(value == null)
            {
                value = "";
            }

            if (Has(key))
            {
                _parameters[key] = value;
            }
            else
            {
                _parameters.Add(key, value);
                _order.Add(key);
            }
        }

        public string Get(string key)
        {
            if (Has(key))
            {
                return _parameters[key];
            }
            else
            {
                return null;
            }
        }

        public bool Has(string key)
        {
            return _parameters.ContainsKey(key);
        }

        public string ToCsv
        {
            get
            {
                string row = "";
                foreach (string key in _order)
                {
                    row += _parameters[key] + ";";
                }
                row = row.TrimEnd(';');
                row += "\n";
                return row;
            }
        }

        public string CsvHeader
        {
            get
            {
                string header = "";
                foreach (string key in _order)
                {
                    header += key + ";";
                }
                header = header.TrimEnd(';');
                header += "\n";
                return header;
            }
        }


    }
}
