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
using skotstein.net.http.reachabilityanalyzer.tools;
using SKotstein.Net.Http.Attributes;
using SKotstein.Net.Http.Context;
using SKotstein.Net.Http.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skotstein.net.http.reachabilityanalyzer.proxy.server
{
    /// <summary>
    /// <see cref="HttpController"/> that implements endpoints for processing GET, POST, PUT, and DELETE requests for all URIs.
    /// </summary>
    public class ProxyController : HttpController
    {
        public const string TAG = "CONTROLLER";
        private bool _addStackTraceToErrorLog = true;

        private IProxyHandler _proxyHandler;

        public ProxyController(IProxyHandler proxyHandler)
        {
            _proxyHandler = proxyHandler;
        }

        /// <summary>
        /// Endpoint for answering incoming GET requests
        /// </summary>
        /// <param name="context"></param>
        [Path("/*", HttpMethod.GET)]
        public void Get(HttpContext context)
        {
            try
            {
                _proxyHandler.ProcessGet(context);
            }
            catch (Exception e)
            {
                LogError(e, _addStackTraceToErrorLog);
                throw e;
            }
        }

        /// <summary>
        /// Endpoint for answering incoming POST requests
        /// </summary>
        [Path("/*", HttpMethod.POST)]
        public void Post(HttpContext context)
        {
            try
            {
                _proxyHandler.ProcessPost(context);
            }
            catch (Exception e)
            {
                LogError(e, _addStackTraceToErrorLog);
                throw e;
            }
        }

        /// <summary>
        /// Endpoint for answering incoming PUT requests
        /// </summary>
        [Path("/*", HttpMethod.PUT)]
        public void Put(HttpContext context)
        {
            try
            {
                _proxyHandler.ProcessPut(context);
            }
            catch (Exception e)
            {
                LogError(e, _addStackTraceToErrorLog);
                throw e;
            }
        }

        /// <summary>
        /// Endpoint for answering incoming DELETE requests
        /// </summary>
        [Path("/*", HttpMethod.DELETE)]
        public void Delete(HttpContext context)
        {
            try
            {
                _proxyHandler.ProcessDelete(context);
            }
            catch (Exception e)
            {
                LogError(e, _addStackTraceToErrorLog);
                throw e;
            }
        }

        /// <summary>
        /// Endpoint for answering incoming PATCH requests
        /// </summary>
        [Path("/*", HttpMethod.PATCH)]
        public void Patch(HttpContext context)
        {
            try
            {
                _proxyHandler.ProcessPatch(context);
            }
            catch (Exception e)
            {
                LogError(e, _addStackTraceToErrorLog);
                throw e;
            }
        }

        /// <summary>
        /// Adds the message of the passed <see cref="Exception"/> as well messages of all nested inner <see cref="Exception"/>s to the error log.
        /// If the second argument is 'true', the method appends the <see cref="Exception.StackTrace"/> to the error log (after adding all nested messages).
        /// </summary>
        /// <param name="e"></param>
        private void LogError(Exception e, bool addStackTrace)
        {
            Exception exception = e;
            while (e != null)
            {
                Log.Error(TAG, e.Message);
                e = e.InnerException;
            }
            if (addStackTrace)
            {
                Log.Error(TAG, exception.StackTrace);
            }
        }
    }
}
