using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using skotstein.net.http.reachabilityanalyzer.proxy.client;
using skotstein.net.http.reachabilityanalyzer.proxy.server;
using skotstein.net.http.reachabilityanalyzer.proxy.tools;
using skotstein.net.http.reachabilityanalyzer.tools;
using skotstein.net.http.urimodel;
using SKotstein.Net.Http.Core;
using SKotstein.Net.Http.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skotstein.net.http.reachabilityanalyzer.proxy
{
    public class Program
    {
        public static string TAG = "MAIN";

        public static void Main(string[] args)
        {
            //step 1: load arguments
            CustomizedArguments arguments = CustomizedArguments.Parse(args);

            //step 2: load log writer
            if (arguments.Has(CustomizedArguments.ARG_LOG_OUT))
            {
                //console
                if (arguments.Get(CustomizedArguments.ARG_LOG_OUT).CompareTo(CustomizedArguments.OPT_LOG_OUT_CONSOLE) == 0)
                {
                    Log.LogWriters.Add(new CustomizedConsoleLogWriter() { LogLevel = arguments.LogLevel });
                }
                //file
                else
                {
                    Log.LogWriters.Add(new FileLogWriter(arguments.LogPath + DateTime.Now.ToString(String.Format("yyyy-MM-dd_HH.mm.ss.FFF")) + ".log") { LogLevel = arguments.LogLevel });
                }
                Log.Debug(TAG, "log_out:" + arguments.Get(CustomizedArguments.ARG_LOG_OUT));
            }
            //default (console)
            else
            {
                Log.LogWriters.Add(new CustomizedConsoleLogWriter() { LogLevel = arguments.LogLevel });
            }

            //step 3: load OpenAPI documentation
            Log.Info(TAG, "Load OpenAPI documentation from '" + arguments.Get(CustomizedArguments.ARG_SOURCE) + "'");
            OpenApiDiagnostic openApiDiagnostic = new OpenApiDiagnostic();
            OpenApiDocument openApiDocument = new OpenApiStringReader().Read(File.ReadAllText(arguments.Get(CustomizedArguments.ARG_SOURCE)), out openApiDiagnostic);
            foreach(OpenApiError openApiError in openApiDiagnostic.Errors)
            {
                Log.Error(TAG, "OpenAPI Reader Error: " + openApiError.Message);
            }
            Log.Info(TAG, "Load OpenAPI documentation - completed\n");

            //step 4: create URI Model
            Log.Info(TAG, "Create URI Model:");
            UriModel uriModel = CustomizedUriModelFactory.Instance.Create(openApiDocument);
            Log.Info(TAG, "\n" + uriModel.ToString() + "\n");

            //step 5: identify reachability associations
            Log.Info(TAG, "Identify reachability associations:");
            ILinkAnalyzer linkAnalyzer = LinkAnalyzerFactory.Create();
            IList<PathSegment> pathSegmentsRepresentingResources = uriModel.Root.QuerySubTree.HasOperations().Results;
            foreach (PathSegment pathSegmentRepresentingResources in pathSegmentsRepresentingResources)
            {
                ((CustomizedPathSegment)pathSegmentRepresentingResources).IdentifyReachabilityPaths(linkAnalyzer);
                Log.Info(TAG, pathSegmentRepresentingResources.UriPath);
                foreach(ReachabilityPath association in ((CustomizedPathSegment)pathSegmentRepresentingResources).ReachabilityPaths)
                {
                    Log.Info(TAG, association.ToString());
                }
            }

            //step 6: set base path
            string basePath = "";
            if (arguments.Has(CustomizedArguments.ARG_API_BASE_PATH))
            {
                basePath = arguments.Get(CustomizedArguments.ARG_API_BASE_PATH);
            }
            else
            {
                foreach(OpenApiServer openApiServer in openApiDocument.Servers)
                {
                    basePath = openApiServer.Url;
                }
            }
            Log.Info(TAG, "\nBase path of remote API: '" + basePath + "'");

            //step 7: load property name prefix of injected properties
            string propertyNamePrefix = "";
            if (arguments.Has(CustomizedArguments.ARG_INJECTION_PREFIX))
            {
                propertyNamePrefix = arguments.Get(CustomizedArguments.ARG_INJECTION_PREFIX);
            }

            //step 8: prepare proxy
            IProxyHandler handler = new CustomizedProxyHandler(basePath, uriModel,propertyNamePrefix,arguments.Has(CustomizedArguments.ARG_EXTENDED_HYPERLINK),arguments.Has(CustomizedArguments.ARG_PREFLIGHT_HEAD_COUNTER)?Int32.Parse(arguments.Get(CustomizedArguments.ARG_PREFLIGHT_HEAD_COUNTER)):10);

            ApiRequest.IgnoreBadCertificates();

            if (arguments.Has(CustomizedArguments.ARG_ACCESS_TOKEN))
            {
                handler.UpdateAcessTokenAutomatically = false;
                handler.UpdateAuthorization(arguments.Get(CustomizedArguments.ARG_ACCESS_TOKEN));
                Log.Info(TAG, "Access token has been loaded from arguments and will be injected into API requests automatically");
            }
            else
            {
                handler.UpdateAcessTokenAutomatically = true;
                Log.Warning(TAG, "Access token is not specified in arguments. If the API expects an access token, make sure that the client specifies this token in each request.");
            }

            HttpController controller = new ProxyController(handler);

            //step 9: start proxy
            HttpService service = new DefaultHttpSysService(false, "+", Int32.Parse(arguments.Get(CustomizedArguments.ARG_PORT)));
            service.AddController(controller);

            Log.Info(TAG, "Proxy routes:");
            Log.Info(TAG, service.Routes);
            service.Start();
            Log.Info(TAG, "Proxy is listening on port '" + arguments.Get(CustomizedArguments.ARG_PORT) + "'");

            Console.WriteLine("PRESS KEY TO TERMINATE SERVICE");
            Console.ReadKey();
        }
    }
}
