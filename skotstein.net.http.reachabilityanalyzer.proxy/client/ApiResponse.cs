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
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace skotstein.net.http.reachabilityanalyzer.proxy.client
{
    public class ApiResponse
    {
        private HttpResponseMessage _responseMessage = new HttpResponseMessage();

        public int StatusCode
        {
            get
            {
                return (int)_responseMessage.StatusCode;
            }
        }

        public HttpResponseMessage ResponseMessage
        {
            get
            {
                return _responseMessage;
            }

            internal set
            {
                _responseMessage = value;
            }
        }

        internal ApiResponse(HttpResponseMessage responseMessage)
        {
            ResponseMessage = responseMessage;
        }

        public void CopyTo(HttpResponse httpResponse)
        {
            //step 1: set status code
            httpResponse.Status = (HttpStatus)((int)_responseMessage.StatusCode);

            //step 2: set header
            //TODO: add header

            //step 3: set payload
            if (_responseMessage.Content.Headers.ContentType != null)
            {
                string contentType = _responseMessage.Content.Headers.ContentType.MediaType;
                if (!String.IsNullOrWhiteSpace(_responseMessage.Content.Headers.ContentType.CharSet))
                {
                    contentType += "; charset=" + _responseMessage.Content.Headers.ContentType.CharSet;
                }
                foreach (NameValueHeaderValue parameter in _responseMessage.Content.Headers.ContentType.Parameters)
                {
                    if (!String.IsNullOrWhiteSpace(parameter.Name))
                    {
                        contentType += "; " + parameter.Name;
                        if (!String.IsNullOrWhiteSpace(parameter.Value))
                        {
                            contentType += "=" + parameter.Value;
                        }
                    }
                }
                httpResponse.Headers.Set("Content-Type", contentType);
            }
            byte[] payload = _responseMessage.Content.ReadAsByteArrayAsync().Result;
            httpResponse.Payload.WriteBytes(payload);
        }

        public override string ToString()
        {
            return ((int)_responseMessage.StatusCode) + " " + _responseMessage.StatusCode.ToString();
        }
    }
}
