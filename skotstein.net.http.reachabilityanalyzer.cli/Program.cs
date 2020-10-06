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

namespace skotstein.net.http.reachabilityanalyzer.cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //step 0: load arguments
            CustomizedArguments arguments = CustomizedArguments.Parse(args);
            if (!arguments.Has(CustomizedArguments.ARG_SOURCE))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Path to OpenAPI documentation is missing. Use the parameter '" + CustomizedArguments.ARG_SOURCE + "' to specify the path to the JSON or YAML file");
            }

            //step 1a: load open API documentation and create URI Model
            Console.WriteLine("Load OpenAPI documentation from: '"+arguments.Get(CustomizedArguments.ARG_SOURCE)+"'");
            UriModel uriModel = CustomizedUriModelFactory.Instance.Create(arguments.Get(CustomizedArguments.ARG_SOURCE));
            Console.WriteLine(uriModel);
            Console.WriteLine("");

            //step 1b: calculate metrics
            int roots = 0;
            if (uriModel.Root.HasOperations)
            {
                roots = 1;
            }
            else
            {
                roots = uriModel.Root.NextDescendantsWithOperations.Count;
            }
            Console.WriteLine("Number of roots: " +roots);
            Console.WriteLine("Height (without root): " + (uriModel.Root.Height - 1));
            Console.WriteLine("Number of Path Segments (without root): " + (uriModel.Root.QuerySubTree.Results.Count - 1));
            Console.WriteLine("Number of Var. Path Segments: " + uriModel.Root.QuerySubTree.IsVariable().Results.Count);
            Console.WriteLine("Number of Resources: " + uriModel.Root.QuerySubTree.HasOperations().Results.Count);
            Console.WriteLine();

            //step 2: identity reachability paths for all path segments representing resources
            Console.Write("Identify reachability paths");
            IList<PathSegment> pathSegmentsRepresentingResources = uriModel.Root.QuerySubTree.HasOperations().Results;
            foreach(PathSegment pathSegmentRepresentingResources in pathSegmentsRepresentingResources)
            {
                ((CustomizedPathSegment)pathSegmentRepresentingResources).IdentifyReachabilityPaths(LinkAnalyzerFactory.Create());
            }
            Console.WriteLine(" - completed");

            //step 3: print identified reachability paths:
            foreach (PathSegment pathSegmentRepresentingResources in pathSegmentsRepresentingResources)
            {
                Console.WriteLine(pathSegmentRepresentingResources.UriPath+":");
                foreach(ReachabilityPath reachabilityPath in ((CustomizedPathSegment)pathSegmentRepresentingResources).ReachabilityPaths)
                {
                    Console.WriteLine(reachabilityPath);
                }
                Console.WriteLine("");
            }

            //step 4: calculate reachability paths
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
            /********************************************* api.version.links.#ReachableResources*********/
            foreach (PathSegment pathSegment in apiStartPathSegments)
            {
                FollowReachabilityPaths(pathSegment, reachedPathSegments, "2", new List<HttpMethod>() { HttpMethod.GET }, new List<LinkMatchType>());
            }
            int numberOfReachedPathSegments = GetNumberOfPathSegmentsRepresentingResources(reachedPathSegments);
            Console.WriteLine("Number of reached resources: " + numberOfReachedPathSegments+" of "+ uriModel.Root.QuerySubTree.HasOperations().Results.Count);

            /********************************************* api.version.links.%ReachableResourcesWithExactMatch*********/
            Console.WriteLine("Share of reached resources: "+(((double)numberOfReachedPathSegments / (double)uriModel.Root.QuerySubTree.HasOperations().Results.Count) *100)+" %");
            Console.WriteLine();
            foreach(PathSegment resourcePathSegment in uriModel.Root.QuerySubTree.HasOperations().Results)
            {
                if (reachedPathSegments.Contains(resourcePathSegment))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.WriteLine(resourcePathSegment.UriPath);
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
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
        public static void FollowReachabilityPaths(PathSegment pathSegment, ISet<PathSegment> alreadyReachedPathSegments, string statusCodeFilter, IList<HttpMethod> operationTypeFilter, IList<LinkMatchType> linkMatchTypeFilter)
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

            foreach (ReachabilityPath reachabilityPath in ((CustomizedPathSegment)pathSegment).ReachabilityPaths)
            {
                if (reachabilityPath.Type == ReachabilityPathType.oneToMany)
                {
                    //one-to-many
                    bool atLeastOneLinkMatchesFilter = false;
                    foreach (Link link in reachabilityPath.Links)
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
                        FollowReachabilityPaths(reachabilityPath.Target, alreadyReachedPathSegments, statusCodeFilter, operationTypeFilter, linkMatchTypeFilter);
                    }
                }
                else
                {
                    //one-to-one
                    FollowReachabilityPaths(reachabilityPath.Target, alreadyReachedPathSegments, statusCodeFilter, operationTypeFilter, linkMatchTypeFilter);
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
    }
}
