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
    /// A <see cref="Link"/> connects a <see cref="PathParameter"/> with a response payload that is the outcome of a particular <see cref="Operation"/>.
    /// More precisely, it contains 
    /// It contains all required information for extracting one or multiple identity based values out of the particular response payload.
    /// This includes the 
    /// </summary>
    public class Link
    {
        private PathParameter _pathParameter;
        private Operation _operation;
        private string _mediaType;
        private string _statusCode;
        private string _xPath;
        private bool _containsMultipleValues;
        private LinkMatchType _matchType;

        /// <summary>
        /// Gets or sets the path parameter that can be substituted with identity based values queried from the response payload.
        /// </summary>
        public PathParameter PathParameter
        {
            get
            {
                return _pathParameter;
            }

            set
            {
                _pathParameter = value;
            }
        }

        /// <summary>
        /// Gets or sets the operation that must be exectuted in order to receive the response payload that contains 
        /// feasible identity based values for substituting the specified <see cref="PathParameter"/>.
        /// </summary>
        public Operation Operation
        {
            get
            {
                return _operation;
            }

            set
            {
                _operation = value;
            }
        }

        /// <summary>
        /// Gets or sets the media type that must be requested in order to receive the response payload that contains feasible
        /// identity based values for substituting the specified <see cref="PathParameter"/>.
        /// </summary>
        public string MediaType
        {
            get
            {
                return _mediaType;
            }

            set
            {
                _mediaType = value;
            }
        }

        /// <summary>
        /// Gets or sets the status code that must be returned when executing the specified <see cref="Operation"/> 
        /// such that the response payload contains feasible identity based values for subsituting the specified <see cref="PathParameter"/>.
        /// </summary>
        public string StatusCode
        {
            get
            {
                return _statusCode;
            }

            set
            {
                _statusCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the XPath or JSONPath for querying the response payload for identity based values 
        /// that are feasible for the substitution of the specified <see cref="PathParameter"/>.
        /// </summary>
        public string XPath
        {
            get
            {
                return _xPath;
            }

            set
            {
                _xPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the flag that indicates whether one (false) or multiple (true) values can be expected when querying the response payload.
        /// </summary>
        public bool ContainsMultipleValues
        {
            get
            {
                return _containsMultipleValues;
            }

            set
            {
                _containsMultipleValues = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="LinkMatchType"/> of this <see cref="Link"/>.
        /// </summary>
        public LinkMatchType MatchType
        {
            get
            {
                return _matchType;
            }

            set
            {
                _matchType = value;
            }
        }

        /// <summary>
        /// This method adds this <see cref="Link"/> to the list of <see cref="CustomizedPathParameter.Links"/>.
        /// </summary>
        public void ActivateBackReferenceToPathParameter()
        {
            ((CustomizedPathParameter)PathParameter).Links.Add(this);
        }
    }

    public enum LinkMatchType
    {
        exactMatch, pathParameterNameContainsPropertyName, propertyNameContainsPathParameterName
    }

}
