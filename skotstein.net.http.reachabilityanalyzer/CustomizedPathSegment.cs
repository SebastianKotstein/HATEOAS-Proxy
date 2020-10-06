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
    /// <summary>
    /// <see cref="CustomizedPathSegment"/> extends the original <see cref="PathSegment"/> type by adding methods for identifying <see cref="ReachabilityPath"/>s
    /// between this <see cref="PathSegment"/> and other <see cref="PathSegment"/>s. 
    /// </summary>
    public class CustomizedPathSegment : PathSegment
    {
        private IList<ReachabilityPath> _reachabilityPaths = new List<ReachabilityPath>();

        public IList<ReachabilityPath> ReachabilityPaths
        {
            get
            {
                return _reachabilityPaths;
            }
        }

        /// <summary>
        /// Returns the <see cref="ApiRoot"/> of this <see cref="PathSegment"/>
        /// </summary>
        /// <returns></returns>
        private PathSegment Root
        {
            get
            {
                PathSegment ps = this;
                while (true)
                {
                    if (ps.IsRoot)
                    {
                        return ps;
                    }
                    else
                    {
                        ps = (PathSegment)ps.Parent;
                    }
                }
            }
        }

        public CustomizedPathSegment(string value, string uriPath) : base(value, uriPath)
        {

        }


        public void IdentifyReachabilityPaths(ILinkAnalyzer linkAnalyzer)
        {
            //step 1: pre-allocate variable path segments
            PreAllocateAllPathSegmentsOnThePathFromThisToRoot();

            //step 2: identify one-to-pne reachability paths
            IList<ReachabilityPath> identifiedOneToOneReachabilityPaths = IdentifyOneToOneReachabilityPath();

            //step 3: identify one-to-many reachability paths
            IList<ReachabilityPath> identifiedOneToManyReachabilityPath = IdentifyOneToManyReachabilityPath(linkAnalyzer);

            //step 4: add all identified reachability paths
            foreach(ReachabilityPath reachabilityPath in identifiedOneToOneReachabilityPaths)
            {
                this.ReachabilityPaths.Add(reachabilityPath);
            }
            foreach (ReachabilityPath reachabilityPath in identifiedOneToManyReachabilityPath)
            {
                this.ReachabilityPaths.Add(reachabilityPath);
            }

            //step 5: reset pre-allocation
            this.Root.ResetAllocation();
        }

        /// <summary>
        /// Pre-allocates all (variable) <see cref="PathSegment"/>s on the path from this <see cref="PathSegment"/> to the the root
        /// </summary>
        private void PreAllocateAllPathSegmentsOnThePathFromThisToRoot()
        {
            PathSegment pathSegment = this;
            while (pathSegment != null)
            {
                pathSegment.IsAllocated = true;
                pathSegment = (PathSegment)pathSegment.Parent;
            }
        }

        #region Identification of One-to-One Reachability Paths
        /// <summary>
        /// Identifies and returns the list of one-to-one <see cref="ReachabilityPath"/>s 
        /// </summary>
        /// <returns></returns>
        private IList<ReachabilityPath> IdentifyOneToOneReachabilityPath()
        {
            IList<ReachabilityPath> reachabilityPaths = new List<ReachabilityPath>();

            //step 1: identify next ancestor
            PathSegment nextAncestor = this.NextAncestorWithOperations; //returns a path segment representing one resource after pre-allocation
            if(nextAncestor != null)
            {
                reachabilityPaths.Add(new ReachabilityPath(this, nextAncestor, ReachabilityPathType.oneToOne));
            }

            //step 2: identify next descendants
            IList<PathSegment> nextDescendants = this.NextDescendantsRepresentingOneResource;
            foreach(PathSegment nextDescendant in nextDescendants)
            {
                reachabilityPaths.Add(new ReachabilityPath(this, nextDescendant, ReachabilityPathType.oneToOne));
            }

            return reachabilityPaths;
        }

        /// <summary>
        /// Gets the list containing all next descendants of this <see cref="PathSegment"/> that support at least one operation AND represents a single resource (after pre-allocation).
        /// The returned list is empty, if no such next descendant exists.
        /// See <see cref="PathSegment.NextDescendantsWithOperations"/> for further details.
        /// </summary>
        public IList<PathSegment> NextDescendantsRepresentingOneResource
        {
            get
            {
                IList<PathSegment> results = new List<PathSegment>();
                foreach (PathSegment pathSegment in this.NextDescendantsWithOperations)
                {
                    if (pathSegment.ResourceEndpointType == ResourceEndpointType.one)
                    {
                        results.Add(pathSegment);
                    }
                }
                return results;
            }
        }

        #endregion
        #region Identification of One-to-Many Reachability Paths
        /// <summary>
        /// Identifies and returns the list of one-to-many <see cref="ReachabilityPath"/>s 
        /// </summary>
        /// <returns></returns>
        private IList<ReachabilityPath> IdentifyOneToManyReachabilityPath(ILinkAnalyzer linkAnalyzer)
        {
            IList<ReachabilityPath> reachabilityPaths = new List<ReachabilityPath>();

            //step 1: load API Root
            PathSegment apiRoot = this.Root;

            //step 2: load all variable path segments of the URI Model (note that pre-allocated variable path segments are ignored)
            IList<PathSegment> variablePathSegments = apiRoot.QuerySubTree.IsVariable().Results;

            //step 3: prepare and identify list of variable path segments that are on the path between this path segment and the next descendants 
            IList<PathSegment> variablePathSegmentsOnThePathBetweenThisPathSegmentAndNextDescendants = new List<PathSegment>();

            foreach (PathSegment variablePathSegment in variablePathSegments)
            {
                //calculate path between API Root and the respective variable path segment 
                //Note that for our case it does not matter, whether calculating the path between this path segment and the variable path segment
                //or the API Root and the variable path segment, since between API root and this path segment cannot be any variable path segment
                Stack<Node<string>> path = apiRoot.Path(variablePathSegment);

                //in the course of the next step, we will count the number of variable path segments being on the path between the API Root (or this path segment) and the respective variable path segment.
                int variablePathSegmentCounter = 0;
                while(path.Count > 0)
                {
                    PathSegment pathSegment = (PathSegment)path.Pop();
                    if (pathSegment.IsVariable)
                    {
                        //increase counter for each found variable path segment
                        variablePathSegmentCounter++;
                    }
                }

                //if there is exactely one variable path segment (namely the variable path segment 
                if(variablePathSegmentCounter == 1)
                {
                    variablePathSegmentsOnThePathBetweenThisPathSegmentAndNextDescendants.Add(variablePathSegment);
                }
            }

            //step 4: iterate over the list of identified variable path segments
            foreach(PathSegment variablePathSegment in variablePathSegmentsOnThePathBetweenThisPathSegmentAndNextDescendants)
            {
                IList<Link> links = new List<Link>();
                
                //iterate over all path parameters of the respective variable path segment
                foreach(PathParameter pathParameter in variablePathSegment.PathParameters)
                {
                    //analyze response payload schemas
                    IList<Link> linksToPathParameter = linkAnalyzer.AnalyzeResource(this, pathParameter);
                    foreach(Link linkToPathParameter in linksToPathParameter)
                    {
                        links.Add(linkToPathParameter);
                    }
                }

                //step 5: now we determine the target of the one-to-many reachability paths
                //if the respective variable path segment has operations (i.e. represents multiple resources)...
                if (variablePathSegment.HasOperations)
                {
                    //...then it is the target
                    ReachabilityPath reachabilityPath = new ReachabilityPath(this, variablePathSegment, ReachabilityPathType.oneToMany);
                    foreach(Link link in links)
                    {
                        reachabilityPath.Links.Add(link);
                        //activates the back reference (i.e. adds the link to the path parameter)
                        link.ActivateBackReferenceToPathParameter();
                    }
                    reachabilityPaths.Add(reachabilityPath);
                }
                //if the respective variable path segment HAS NOT any operation (i.e. DOES NOT represent multiple resources)
                else
                {
                    //...then whe have to identify the next descendants of this variable path segment and set them as targets
                    //Note that we want only next descendants that are NOT variable or whose path between the respective variable path segment and the next descendant
                    //is not blocked by a variable path segment, since on the path from the source to the target can only be one variable path segment
                    foreach(PathSegment nextDescendant in ((CustomizedPathSegment)variablePathSegment).NextDescendantsRepresentingResourcesButWithoutVariablePathSegments)
                    {
                        //...then the next descendant is one target
                        ReachabilityPath reachabilityPath = new ReachabilityPath(this, nextDescendant, ReachabilityPathType.oneToMany);
                        foreach (Link link in links)
                        {
                            reachabilityPath.Links.Add(link);
                            //activates the back reference (i.e. adds the link to the path parameter)
                            link.ActivateBackReferenceToPathParameter();

                        }
                        reachabilityPaths.Add(reachabilityPath);
                    }
                }
            }
            return reachabilityPaths;
        }

        /// <summary>
        /// Gets the list containing all next descendants of this <see cref="PathSegment"/>. The following rules must apply, such that a <see cref="PathSegment"/>
        /// is recognized as a next descendant by this method:
        /// - the next descendant must have at least one operation (i.e. represents a resource)
        /// - the next descendant cannot be variable
        /// - the path between the next descendant and this path segment cannot be blocked by a variable path segment
        /// This method is used within <see cref="IdentifyOneToManyReachabilityPath"/>.
        /// </summary>
        private IList<PathSegment> NextDescendantsRepresentingResourcesButWithoutVariablePathSegments
        {
            get
            {
                IList<PathSegment> nextDescendants = new List<PathSegment>();
                foreach (Node<string> c in this.Children)
                {
                    CustomizedPathSegment child = (CustomizedPathSegment)c;
                    if (!child.IsVariable)
                    {
                        if (child.HasOperations)
                        {
                            nextDescendants.Add(child);
                        }
                        else
                        {
                            IList<PathSegment> nextDescendantsOfChild = child.NextDescendantsRepresentingResourcesButWithoutVariablePathSegments;
                            foreach(PathSegment nextDescendantOfChild in nextDescendantsOfChild)
                            {
                                nextDescendants.Add(nextDescendantOfChild);
                            }
                        }
                    }
                }
                return nextDescendants;
            }
        }
        #endregion

    }
}
