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
    public class ReachabilityPath
    {
        private PathSegment _source;
        private PathSegment _target;
        private ReachabilityPathType _type;

        private IList<Link> _links = new List<Link>();

        public PathSegment Source
        {
            get
            {
                return _source;
            }
        }

        public PathSegment Target
        {
            get
            {
                return _target;
            }
        }

        public ReachabilityPathType Type
        {
            get
            {
                return _type;
            }
        }

        public IList<Link> Links
        {
            get
            {
                return _links;
            }
        }

        public ReachabilityPath(PathSegment source, PathSegment target, ReachabilityPathType type)
        {
            _source = source;
            _target = target;
            _type = type;
        }

        public override string ToString()
        {
            string s = "---";
            switch (Type)
            {
                case ReachabilityPathType.oneToOne:
                    s += " one-to-one  ";
                    break;
                case ReachabilityPathType.oneToMany:
                    s += " one-to-many ";
                    break;
            }
            s += "---> " + Target.UriPath + " (" + Links.Count + ")";
            return s;
        }

    }

    public enum ReachabilityPathType
    {
        oneToOne, oneToMany
    }
}
