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
using skotstein.net.http.reachabilityanalyzer.proxy.client;
using skotstein.net.http.reachabilityanalyzer.proxy.model;
using skotstein.net.http.reachabilityanalyzer.tools;
using skotstein.net.http.urimodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace skotstein.net.http.reachabilityanalyzer.proxy.server
{
    public abstract class ProxyHandler : IProxyHandler
    {
        public static string TAG = "HANDLER";

        protected ApiClient _apiClient = new ApiClient();
        protected UriModel _uriModel;

        protected string _remoteApiBasePath;
        protected string _authorization = null;

        private int _headRetryCounter = -1;

        private bool _updateAcessTokenAutomatically = true;

        public string RemoteApiBasePath
        {
            get
            {
                return _remoteApiBasePath;
            }

            set
            {
                _remoteApiBasePath = value;
            }
        }

        public int HeadRetryCounter
        {
            get
            {
                return _headRetryCounter;
            }

            set
            {
                _headRetryCounter = value;
            }
        }

        public bool UpdateAcessTokenAutomatically
        {
            get
            {
                return _updateAcessTokenAutomatically;
            }

            set
            {
                _updateAcessTokenAutomatically = value;
            }
        }

        public abstract void ProcessDelete(SKotstein.Net.Http.Context.HttpContext context);
        public abstract void ProcessGet(SKotstein.Net.Http.Context.HttpContext context);
        public abstract void ProcessPatch(SKotstein.Net.Http.Context.HttpContext context);
        public abstract void ProcessPost(SKotstein.Net.Http.Context.HttpContext context);
        public abstract void ProcessPut(SKotstein.Net.Http.Context.HttpContext context);


        /// <summary>
        /// Forwards the passed <see cref="HttpContext.Request"/> to the wrapped API and writes the result back into the <see cref="HttpResponse"/> of the passed <see cref="HttpContext"/> for
        /// further processing. Furthermore, this method extracts the authorization header value (if present) and updates the local copy
        /// </summary>
        /// <param name="context"></param>
        protected void Forward(SKotstein.Net.Http.Context.HttpContext context)
        {
            ApiRequest apiRequest = ApiRequest.Create(context.Request, _remoteApiBasePath);


            if (!UpdateAcessTokenAutomatically)
            {
                apiRequest.WithAuthHeader(_authorization);
            }

            ApiResponse response = _apiClient.Send(apiRequest);
            response.CopyTo(context.Response);

            if (context.Request.Headers.Has("Authorization"))
            {
                if (UpdateAcessTokenAutomatically)
                {
                    UpdateAuthorization(context.Request.Headers.Get("Authorization"));
                }
            }
        }

        public void UpdateAuthorization(string authorization)
        {
            _authorization = authorization;
        }

        protected PreflightResult SendPreflight(string path)
        {
            try
            {
                if(HeadRetryCounter == 0)
                {
                    //HTTP GET
                    return SendPreflightRequest(path, System.Net.Http.HttpMethod.Get);
                }
                else
                {
                    //HTTP HEAD
                    PreflightResult preflightResult = SendPreflightRequest(path, System.Net.Http.HttpMethod.Head);
                    if(preflightResult.StatusCode != 200)
                    {
                        Log.Warning(ProxyHandler.TAG, "It seems, that remote API does not support HEAD. Retry with GET ...");
                        preflightResult = SendPreflightRequest(path, System.Net.Http.HttpMethod.Get);
                        if(HeadRetryCounter != -1)
                        {
                            HeadRetryCounter--;
                        }
                    }
                    return preflightResult;
                }
            }
            catch(Exception e)
            {
                return new PreflightResult(999, e.Message);
            }

        }

        protected PreflightResult SendPreflightRequest(string path, System.Net.Http.HttpMethod method)
        {
            try
            {
                ApiRequest apiRequest = ApiRequest.Create(method, _remoteApiBasePath + path);
                if (!String.IsNullOrWhiteSpace(_authorization))
                {
                    apiRequest.WithAuthHeader(_authorization);
                }
                ApiResponse response = _apiClient.Send(apiRequest);
                return new PreflightResult(response.StatusCode, response.ResponseMessage.Content.ReadAsStringAsync().Result);
            }
            catch(Exception e)
            {
                return new PreflightResult(999, e.Message);
            }
        }
    }
}
