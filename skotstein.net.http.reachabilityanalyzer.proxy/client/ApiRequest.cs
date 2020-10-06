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
using SKotstein.Net.Http.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace skotstein.net.http.reachabilityanalyzer.proxy.client
{
    public class ApiRequest
    {
        private HttpRequestMessage _requestMessage = new HttpRequestMessage();

        /// <summary>
        /// Gets or sets the nested <see cref="HttpRequestMessage"/>
        /// </summary>
        internal HttpRequestMessage RequestMessage
        {
            get
            {
                return _requestMessage;
            }

            set
            {
                _requestMessage = value;
            }
        }

        private ApiRequest()
        {

        }

        public override string ToString()
        {
            return _requestMessage.Method.ToString() + " " + _requestMessage.RequestUri.ToString();
        }

        /// <summary>
        /// Gets or sets the authorization header.
        /// The authorization header must start with "Bearer", otherwise it won't be set/changed.
        /// </summary>
        public string AuthorizationHeader
        {
            set
            {
                if (value.Contains("Bearer"))
                {
                    _requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", value.Replace("Bearer", "").TrimStart(' '));
                }
            }
            get
            {
                return _requestMessage.Headers.Authorization.ToString();
            }
        }

        /// <summary>
        /// Sets the authorization header.
        /// The authorization header must start with "Bearer", otherwise it won't be set/changed.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ApiRequest WithAuthHeader(string value)
        {
            if (value.Contains("Bearer"))
            {
                _requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", value.Replace("Bearer", "").TrimStart(' '));
            }
            return this;
        }

        /// <summary>
        /// Creates a minimal <see cref="ApiRequest"/> having the passed method and url.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static ApiRequest Create(System.Net.Http.HttpMethod method, string url)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = new Uri(url);
            requestMessage.Method = method;

            ApiRequest apiRequest = new ApiRequest();
            apiRequest.RequestMessage = requestMessage;
            return apiRequest;
        }

        /// <summary>
        /// Creates an <see cref="ApiRequest"/> having the passed method, url, payload and content-type header.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="payload"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static ApiRequest Create(System.Net.Http.HttpMethod method, string url, string payload, string contentType)
        {
            return ApiRequest.Create(method, url, new StringContent(payload), contentType);
        }

        /// <summary>
        /// Creates an <see cref="ApiRequest"/> having the passed method, url, payload and content-type header.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="payload"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static ApiRequest Create(System.Net.Http.HttpMethod method, string url, byte[] payload, string contentType)
        {
            return ApiRequest.Create(method, url, new ByteArrayContent(payload), contentType);
        }

        /// <summary>
        /// Creates an <see cref="ApiRequest"/> having the passed method, url, payload and content-type header.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="payload"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static ApiRequest Create(System.Net.Http.HttpMethod method, string url, HttpContent payload, string contentType)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = new Uri(url);
            requestMessage.Method = method;

            requestMessage.Content = payload;
            //try to split content-type
            string[] split = contentType.Split(';');
            requestMessage.Content.Headers.ContentType.MediaType = split[0];

            for (int i = 1; i < split.Length; i++)
            {
                string[] parameter = split[i].Trim().Split('=');
                if (parameter.Length == 1 && !String.IsNullOrWhiteSpace(parameter[0]))
                {
                    requestMessage.Content.Headers.ContentType.Parameters.Add(new System.Net.Http.Headers.NameValueHeaderValue(parameter[0].Trim()));
                }
                else if (parameter.Length == 2 && !String.IsNullOrWhiteSpace(parameter[0]) && !String.IsNullOrWhiteSpace(parameter[1]))
                {
                    if (parameter[0].CompareTo("charset") == 0)
                    {
                        requestMessage.Content.Headers.ContentType.CharSet = parameter[1].Trim();
                    }
                    else
                    {
                        requestMessage.Content.Headers.ContentType.Parameters.Add(new System.Net.Http.Headers.NameValueHeaderValue(parameter[0].Trim(), parameter[1].Trim()));
                    }
                }
            }

            ApiRequest apiRequest = new ApiRequest();
            apiRequest.RequestMessage = requestMessage;
            return apiRequest;
        }

        /// <summary>
        /// Creates an <see cref="ApiRequest"/> based on the data of the passed <see cref="HttpRequest"/>.
        /// Note that the host segement of the URI of the <see cref="HttpRequest"/> is replaced by the host value passed as second argument.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public static ApiRequest Create(HttpRequest request, string host)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();

            //step 1: set method
            switch (request.Method)
            {
                case SKotstein.Net.Http.Context.HttpMethod.GET:
                    requestMessage.Method = System.Net.Http.HttpMethod.Get;
                    break;
                case SKotstein.Net.Http.Context.HttpMethod.POST:
                    requestMessage.Method = System.Net.Http.HttpMethod.Post;
                    break;
                case SKotstein.Net.Http.Context.HttpMethod.PUT:
                    requestMessage.Method = System.Net.Http.HttpMethod.Put;
                    break;
                case SKotstein.Net.Http.Context.HttpMethod.DELETE:
                    requestMessage.Method = System.Net.Http.HttpMethod.Delete;
                    break;
                case SKotstein.Net.Http.Context.HttpMethod.PATCH:
                    requestMessage.Method = new System.Net.Http.HttpMethod("PATCH");
                    break;
                case SKotstein.Net.Http.Context.HttpMethod.HEAD:
                    requestMessage.Method = System.Net.Http.HttpMethod.Head;
                    break;
                case SKotstein.Net.Http.Context.HttpMethod.OPTIONS:
                    requestMessage.Method = System.Net.Http.HttpMethod.Options;
                    break;
                case SKotstein.Net.Http.Context.HttpMethod.TRACE:
                    requestMessage.Method = System.Net.Http.HttpMethod.Trace;
                    break;
                case SKotstein.Net.Http.Context.HttpMethod.CONNECT:
                    requestMessage.Method = new System.Net.Http.HttpMethod("CONNECT");
                    break;
            }

            //step 2: set path
            string path = host + request.Path;
            if (!String.IsNullOrWhiteSpace(request.Query))
            {
                path += "?" + request.Query;
            }
            if (!String.IsNullOrWhiteSpace(request.Fragment))
            {
                path += "#" + request.Fragment;
            }
            requestMessage.RequestUri = new Uri(path);

            //step 3: set header
            //set authorization header
            if (request.Headers.Has("Authorization"))
            {
                string value = request.Headers.Get("Authorization");
                if (value.Contains("Bearer"))
                {
                    requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", value.Replace("Bearer", "").TrimStart(' '));
                }
            }

            //add further header
            foreach (string headerName in request.Headers.GetHeaderNames())
            {
                requestMessage.Headers.Add(headerName, request.Headers.Get(headerName));
            }

            //step 4: set payload
            byte[] payload = request.Payload.ReadAllBytes();
            if (payload.Length > 0)
            {
                requestMessage.Content = new ByteArrayContent(payload);

                //set Content-Type header
                if (request.Headers.Has("Content-Type"))
                {
                    //try to split content
                    string[] split = request.Headers.Get("Content-Type").Split(';');
                    requestMessage.Content.Headers.ContentType.MediaType = split[0];

                    for (int i = 1; i < split.Length; i++)
                    {
                        string[] parameter = split[i].Trim().Split('=');
                        if (parameter.Length == 1 && !String.IsNullOrWhiteSpace(parameter[0]))
                        {
                            requestMessage.Content.Headers.ContentType.Parameters.Add(new System.Net.Http.Headers.NameValueHeaderValue(parameter[0].Trim()));
                        }
                        else if (parameter.Length == 2 && !String.IsNullOrWhiteSpace(parameter[0]) && !String.IsNullOrWhiteSpace(parameter[1]))
                        {
                            if (parameter[0].CompareTo("charset") == 0)
                            {
                                requestMessage.Content.Headers.ContentType.CharSet = parameter[1].Trim();
                            }
                            else
                            {
                                requestMessage.Content.Headers.ContentType.Parameters.Add(new System.Net.Http.Headers.NameValueHeaderValue(parameter[0].Trim(), parameter[1].Trim()));
                            }
                        }
                    }
                }
            }
            return ApiRequest.Create(requestMessage);
        }

        /// <summary>
        /// Creates an <see cref="ApiRequest"/> based on the passed <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static ApiRequest Create(HttpRequestMessage request)
        {
            ApiRequest apiRequest = new ApiRequest();
            apiRequest.RequestMessage = request;
            return apiRequest;
        }

        public static void IgnoreBadCertificates()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        }

        private static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }
}
