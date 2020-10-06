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
using Newtonsoft.Json.Linq;
using skotstein.net.http.jsonkit;
using skotstein.net.http.reachabilityanalyzer.proxy.model;
using skotstein.net.http.reachabilityanalyzer.tools;
using skotstein.net.http.urimodel;
using SKotstein.Net.Http.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skotstein.net.http.reachabilityanalyzer.proxy.server
{

    public class CustomizedProxyHandler : ProxyHandler
    {
        public const string LINKS_ARRAY = "links";
        public const string SELF_REL = "self";
        public const string ANCESTOR_REL = "up";
        public const string DESCENDANT_REL = "down";
        public const string ITEM_REL = "item";

        private string _propertyNamePrefix ="";
        private bool _hyperlinkWithDebugInformation = false;

        public CustomizedProxyHandler(string remoteApiBasePath, UriModel model, string propertyNamePrefix, bool hyperlinkWithDebugInformation, int headRetryCounter)
        {
            _remoteApiBasePath = remoteApiBasePath;
            _uriModel = model;
            _propertyNamePrefix = propertyNamePrefix;
            _hyperlinkWithDebugInformation = hyperlinkWithDebugInformation;
            HeadRetryCounter = headRetryCounter;
        }

        public override void ProcessGet(HttpContext context)
        {
            Forward(context);
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300) //only positive results are processed
            {
                string payload = InjectHyperlinks(context.Response.Payload.ReadAll(), context.Request.Path, context.Request.Method, context.Response.StatusCode);
                context.Response.Payload.ClearAll();
                context.Response.Payload.Write(payload);
            }
        }

        public override void ProcessDelete(HttpContext context)
        {
            Forward(context);
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300) //only positive results are processed
            {
                string payload = InjectHyperlinks(context.Response.Payload.ReadAll(), context.Request.Path, context.Request.Method, context.Response.StatusCode);
                context.Response.Payload.ClearAll();
                context.Response.Payload.Write(payload);
            }
        }

        public override void ProcessPatch(HttpContext context)
        {
            Forward(context);
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300) //only positive results are processed
            {
                string payload = InjectHyperlinks(context.Response.Payload.ReadAll(), context.Request.Path, context.Request.Method, context.Response.StatusCode);
                context.Response.Payload.ClearAll();
                context.Response.Payload.Write(payload);
            }
        }

        public override void ProcessPost(HttpContext context)
        {
            Forward(context);
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300) //only positive results are processed
            {
                string payload = InjectHyperlinks(context.Response.Payload.ReadAll(), context.Request.Path, context.Request.Method, context.Response.StatusCode);
                context.Response.Payload.ClearAll();
                context.Response.Payload.Write(payload);
            }
        }

        public override void ProcessPut(HttpContext context)
        {
            Forward(context);
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300) //only positive results are processed
            {
                string payload = InjectHyperlinks(context.Response.Payload.ReadAll(), context.Request.Path, context.Request.Method, context.Response.StatusCode);
                context.Response.Payload.ClearAll();
                context.Response.Payload.Write(payload);
            }
        }

        private string InjectHyperlinks(string content, string path, SKotstein.Net.Http.Context.HttpMethod method, int statusCode)
        {
            //step 1: determine the path segments that represents the queried/manipulated resource
            IList<KeyValuePair<PathSegment, string>> pathSegmentsRepresentingStableUriPath = _uriModel.GetPathSegmentsByStableUriPath(path);
            
            //check whether stable URI path could be mapped
            if(pathSegmentsRepresentingStableUriPath.Count == 0)
            {
                Log.Warning(TAG, "The stable URI path '" + path + "' cannot be mapped to path segments in the URI Model.");
                return content;
            }

            //extract last path segment that represents the queried or manipulated resource
            PathSegment pathSegmentRepresentingQueriedOrManipulatedResource = pathSegmentsRepresentingStableUriPath[pathSegmentsRepresentingStableUriPath.Count - 1].Key;

            //check whether the mapped last path segment represents a resource endpoint
            if (!pathSegmentRepresentingQueriedOrManipulatedResource.HasOperations)
            {
                Log.Warning(TAG, "The stable URI path '" + path + "' is mapped to the path segment with the full URI path '" + pathSegmentRepresentingQueriedOrManipulatedResource.UriPath + "'. However, according to the URI Model, this path segment does not support any operation (i.e. does not represent a resource endpoint).");
                return content;
            }
            
            //print mapping (debug only)
            foreach(KeyValuePair<PathSegment,string> pathSegment in pathSegmentsRepresentingStableUriPath)
            {
                Log.Debug(TAG, "'"+pathSegment.Value + "' is mapped to '" + pathSegment.Key.Value+"'");
            }

            //step 2: check whether content is empty
            if (String.IsNullOrWhiteSpace(content))
            {
                Log.Warning(TAG, "The response payload is empty");
                return content;
            }

            //step 3: parse JSON
            JToken rawData = JToken.Parse(content);
            JObject data = null;
            string xPathPrefix = "$";
            if(rawData is JObject)
            {
                data = (JObject)rawData;
            }
            else if(rawData is JArray)
            {
                data = new JObject();
                data.Add("data", rawData);
                xPathPrefix = "$.data[*]";
            }

            //JObject data = JObject.Parse(content);

            //step 4: add hyperlink to self
            AddHyperlinkToSelf(data,path);

            //step 5: add one-to-one reachability associations
            AddOneToOneReachabilityAssociations(data, pathSegmentsRepresentingStableUriPath);

            //step 6: add one-to-many reachability associations
            AddOneToManyReachabilityAssociations(data, pathSegmentsRepresentingStableUriPath, statusCode, method, xPathPrefix);

            //step 7: return modified value back
            return JsonSerializer.SerializeJson(data);
        }

        private void AddObjectToLinksArray(JObject data, object content)
        {
            JToken token = data[_propertyNamePrefix + LINKS_ARRAY];
            JArray links;
            if(token == null)
            {
                links = new JArray();
                data.Add(_propertyNamePrefix + LINKS_ARRAY, links);
            }
            else
            {
                links = (JArray)data[_propertyNamePrefix + LINKS_ARRAY];
            }
            links.Add(JToken.FromObject(content));
        }

        private void AddObjectToLinksArray(JContainer data, object content)
        {
            JToken token = data[_propertyNamePrefix + LINKS_ARRAY];
            JArray links;
            if (token == null)
            {
                links = new JArray();
                data.Add(new JProperty(_propertyNamePrefix + LINKS_ARRAY, links));
            }
            else
            {
                links = (JArray)data[_propertyNamePrefix + LINKS_ARRAY];
            }
            links.Add(JToken.FromObject(content));
        }


        private void AddHyperlinkToSelf(JObject data, string path)
        {
            Hyperlink self =  new Hyperlink()
            {
                Href = path,
                Rel = SELF_REL
            };
            AddObjectToLinksArray(data, self);
        }

        private void AddOneToOneReachabilityAssociations(JObject data, IList<KeyValuePair<PathSegment,string>> pathSegmentsRepresentingStableUriPath)
        {
            //extract path segment representing the queried or manipulated resource (a check whether it exists has been already done before)
            PathSegment pathSegmentRepresentingQueriedOrManipulatedResource = pathSegmentsRepresentingStableUriPath[pathSegmentsRepresentingStableUriPath.Count - 1].Key;

            foreach (ReachabilityPath reachabilityAssociation in ((CustomizedPathSegment)pathSegmentRepresentingQueriedOrManipulatedResource).ReachabilityPaths)
            {
                //filter for one-to-one reachability associations
                if(reachabilityAssociation.Type == ReachabilityPathType.oneToOne)
                {
                    //create stable URI path pointing on target
                    IList<KeyValuePair<PathSegment, string>> pathSegmentsToTarget = _uriModel.GetPathSegmentsByStablePathSegments(reachabilityAssociation.Target, ConvertListToDictionary(pathSegmentsRepresentingStableUriPath));
                    string pathToTarget = UriModel.BuildFullPath(pathSegmentsToTarget);

                    //send pre-flight request:
                    PreflightResult preflightResult = SendPreflight(pathToTarget);
                    if(preflightResult.StatusCode < 200 || preflightResult.StatusCode >= 300)
                    {
                        if (_hyperlinkWithDebugInformation)
                        {
                            Hyperlink hyperlink = new Hyperlink()
                            {
                                Href = pathToTarget,
                                Rel = pathSegmentRepresentingQueriedOrManipulatedResource.IsAncestorOf(reachabilityAssociation.Target) ? DESCENDANT_REL : ANCESTOR_REL,
                                Debug_statusCode = preflightResult.StatusCode,
                                Debug_msg = preflightResult.Message
                            };
                            this.AddObjectToLinksArray(data, hyperlink);
                        }
                    }
                    else
                    {
                        Hyperlink hyperlink = new Hyperlink()
                        {
                            Href = pathToTarget,
                            Rel = pathSegmentRepresentingQueriedOrManipulatedResource.IsAncestorOf(reachabilityAssociation.Target) ? DESCENDANT_REL : ANCESTOR_REL
                        };
                        this.AddObjectToLinksArray(data, hyperlink);
                    }
                }
            }
        }

        public void AddOneToManyReachabilityAssociations(JObject data, IList<KeyValuePair<PathSegment, string>> pathSegmentsRepresentingStableUriPath, int statusCode, SKotstein.Net.Http.Context.HttpMethod method, string xPathPrefix)
        {
            //extract path segment representing the queried or manipulated resource (a check whether it exists has been already done before)
            PathSegment pathSegmentRepresentingQueriedOrManipulatedResource = pathSegmentsRepresentingStableUriPath[pathSegmentsRepresentingStableUriPath.Count - 1].Key;

            foreach (ReachabilityPath reachabilityAssociation in ((CustomizedPathSegment)pathSegmentRepresentingQueriedOrManipulatedResource).ReachabilityPaths)
            {
                //filter for one-to-one reachability associations
                if (reachabilityAssociation.Type == ReachabilityPathType.oneToMany)
                {
                    foreach (Link link in reachabilityAssociation.Links)
                    {
                        //check whether status code matches documented status code in link
                        if (link.StatusCode.CompareTo(statusCode + "") == 0)
                        {
                            //check whether method matches documented method in link
                            switch (method)
                            {
                                case SKotstein.Net.Http.Context.HttpMethod.GET:
                                    if (link.Operation.Method != urimodel.HttpMethod.GET)
                                    {
                                        continue;
                                    }
                                    break;
                                case SKotstein.Net.Http.Context.HttpMethod.PATCH:
                                    if (link.Operation.Method != urimodel.HttpMethod.PATCH)
                                    {
                                        continue;
                                    }
                                    break;
                                case SKotstein.Net.Http.Context.HttpMethod.POST:
                                    if (link.Operation.Method != urimodel.HttpMethod.POST)
                                    {
                                        continue;
                                    }
                                    break;
                                case SKotstein.Net.Http.Context.HttpMethod.PUT:
                                    if (link.Operation.Method != urimodel.HttpMethod.PUT)
                                    {
                                        continue;
                                    }
                                    break;
                                case SKotstein.Net.Http.Context.HttpMethod.DELETE:
                                    if (link.Operation.Method != urimodel.HttpMethod.DELETE)
                                    {
                                        continue;
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            continue;
                        }

                        //load values
                        string xPath = link.XPath.Replace("$", xPathPrefix);
                        IEnumerable<JToken> values = data.SelectTokens(xPath);

                        //iterate over values
                        foreach (JToken value in values)
                        {
                            IDictionary<PathSegment, string> pathSegmentValues = ConvertListToDictionary(pathSegmentsRepresentingStableUriPath);

                            //subsitutute path parameter
                            IDictionary<PathParameter, string> pathParameterMapping = new Dictionary<PathParameter, string>();
                            pathParameterMapping.Add(link.PathParameter, (string)value);
                            string substitutedVariablePathSegment = link.PathParameter.PathSegment.BuildStablePathSegment(pathParameterMapping);

                            //add subsituted variable path segment
                            pathSegmentValues.Add(link.PathParameter.PathSegment, substitutedVariablePathSegment);

                            //create stable URI path pointing on target
                            IList<KeyValuePair<PathSegment, string>> pathSegmentsToTarget = _uriModel.GetPathSegmentsByStablePathSegments(reachabilityAssociation.Target, pathSegmentValues);
                            string pathToTarget = UriModel.BuildFullPath(pathSegmentsToTarget);

                            //send pre-flight request:
                            PreflightResult preflightResult = SendPreflight(pathToTarget);
                            if (preflightResult.StatusCode < 200 || preflightResult.StatusCode >= 300)
                            {
                                if (_hyperlinkWithDebugInformation)
                                {
                                    Hyperlink hyperlink = new Hyperlink()
                                    {
                                        Href = pathToTarget,
                                        Rel = ITEM_REL,
                                        Debug_statusCode = preflightResult.StatusCode,
                                        Debug_msg = preflightResult.Message
                                    };
                                    

                                    this.AddObjectToLinksArray(value.Parent.Parent, hyperlink);
                                }
                            }
                            else
                            {
                                Hyperlink hyperlink = new Hyperlink()
                                {
                                    Href = pathToTarget,
                                    Rel = ITEM_REL
                                };
                                this.AddObjectToLinksArray(value.Parent.Parent, hyperlink);
                            }

                        }
                    }

                }
            }
        }


        /// <summary>
        /// Converts a list of <see cref="KeyValuePair{TKey, TValue}"/> mapping a <see cref="PathSegment"/> to its stable path segment value into an 
        /// <see cref="IDictionary{TKey, TValue}"/> as it is needed by see <see cref="UriModel.GetPathSegmentsByStablePathSegments(PathSegment, IDictionary{PathSegment, string})"/>;
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private IDictionary<PathSegment,string> ConvertListToDictionary(IList<KeyValuePair<PathSegment, string>> list)
        {
            IDictionary<PathSegment, string> output = new Dictionary<PathSegment, string>();
            foreach(KeyValuePair<PathSegment, string> pathSegment in list)
            {
                output.Add(pathSegment.Key, pathSegment.Value);
            }
            return output;
        }
    }
}
