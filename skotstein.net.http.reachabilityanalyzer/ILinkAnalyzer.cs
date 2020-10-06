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
using skotstein.net.http.urimodel;
using skotstein.net.http.urimodel.openapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skotstein.net.http.reachabilityanalyzer
{
    public interface ILinkAnalyzer
    {
        /// <summary>
        /// Gets or sets the flag indicating whether the string comparison should be case invariant
        /// </summary>
        bool IsNameCaseInvariant { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating whether exact matches of a property name and a parameter name are allowed 
        /// </summary>
        bool AllowExactMatch { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating whether the parameter name can be a sub string of the property name
        /// </summary>
        bool AllowPropertyNameContainsParameterName { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating whether the property name can be a sub string of the parameter name
        /// </summary>
        bool AllowParameterNameContainsPropertyName { get; set; }

        /// <summary>
        /// Gets or sets the response code filter.
        /// If this value is not empty or null, only <see cref="OpenApiSchema"/>s associated with a response code starting with this value are analyzed.
        /// E.g. a value of '2' means that only possitive response code schemas are analyzed (such as '200' - OK)
        /// </summary>
        string ResponseCodeFilter { get; set; }

        /// <summary>
        /// Gets or sets the media type filter.
        /// If this list is empty, response payloads of all media types are analyzed.
        /// Otherwise only media types whose name contains one of the strings in the list are filtered.
        /// </summary>
        IList<string> MediaTypeFilter { get; }

        /// <summary>
        /// Gets or sets the <see cref="OperationType"/> filter.
        /// If this list is empty, all the response payloads of all <see cref="Operation"/>s are analyzed.
        /// Otherwise only <see cref="Operation"/>s of the contained <see cref="HttpMethod"/> are analyzed.
        /// </summary>
        IList<HttpMethod> OperationTypesFilter { get; }

        int MissingMediaCounter { get; set; }

        IList<Link> AnalyzeResource(PathSegment pathSegment, PathParameter pathParameter);

        IList<Link> AnalyzeOperation(OApiOperation operation, PathParameter pathParameter);

        IList<Link> AnalyzeResponse(string statusCode, OpenApiResponse response, PathParameter pathParameter, HttpMethod method);

        IList<Link> AnalyzeMediaType(string mediaTypeName, OpenApiMediaType mediaType, PathParameter pathParameter);

        IList<Link> AnalyzeSchema(OpenApiSchema schema, PathParameter pathParameter);
    }
}
