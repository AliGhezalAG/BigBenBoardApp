using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using log4net;
using RestWCFServiceLibrary.WiiMote.Model;

namespace RestWCFServiceLibrary.WiiMote
{
    class ProxyStatiqueServices
    {
        private static readonly ILog LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly HttpClient client = new HttpClient();


        public async Task<string> PushAsync(string guid, AcquisitionFile file, string authorizationHeader)
        {
            LOG.DebugFormat("<<< Push(): guid={0}, file.Name={1}", guid, file.Name);

            string url = BuildPushFileUrl(guid, file);
            StreamContent payload = BuildPayload(file);

            var response = await PostAsync(url, authorizationHeader, payload);

            LOG.Debug(">>> Push(string,AcquisitionFile)");
            return response;
        }

        private static string BuildPushFileUrl(string guid, AcquisitionFile file)
        {
            var url = string.Concat(
                ConfigurationManager.AppSettings["proxy-statique-url"],
                ConfigurationManager.AppSettings["upload-raw-files"],
                "?guid=",
                guid,
                "&filename=",
                file.Name);

            return url;
        }

        private static StreamContent BuildPayload(AcquisitionFile file)
        {
            var content = new StreamContent(new MemoryStream(file.Blob));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/csv");
            return content;
        }

        public async Task<string> CreateTempFolderAsync(string guid, string authorizationHeader)
        {
            LOG.DebugFormat("<<< CreateTempFolder(): guid={0}", guid);
            string url = BuildTempFolderUrl(guid);

            var result = await PostAsync(url, authorizationHeader);

            LOG.Debug(">>> CreateTempFolder()");
            return result;
        }

        private static string BuildTempFolderUrl(string guid)
        {
            var url = string.Concat(
                ConfigurationManager.AppSettings["proxy-statique-url"],
                ConfigurationManager.AppSettings["temp-folder"],
                "/",
                guid);

            LOG.DebugFormat("Built temp folder creation url: {0}", url);

            return url;
        }

        private async Task<string> PostAsync(string url, string authorizationHeader, HttpContent payload = null)
        {
            HttpResponseMessage hrm = null;
            try
            {
                UpdateAuthorisationHeader(client, authorizationHeader);
                hrm = await client.PostAsync(url, payload);

                hrm.EnsureSuccessStatusCode();
                var response = await hrm.Content.ReadAsStringAsync();
                
                LOG.InfoFormat("HTTP result for {0} --> {1}", url, response);
                return response;
            }
            catch (Exception e)
            {
                string error;
                try
                {
                    error = await hrm?.Content.ReadAsStringAsync();
                }
                catch 
                {
                    error = null;
                }
                LOG.Warn(string.Format("HTTP request failed for {0} => message : " + url, error), e);
                throw new WebFaultException<string>(error ?? e.Message, HttpStatusCode.InternalServerError);
            }
        }

        private static void UpdateAuthorisationHeader(HttpClient client, string authorizationHeader)
        {
            client.DefaultRequestHeaders.Remove("authorization");
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                LOG.Warn("No Authorization token. Request should fail.");
            }
            else
            {
                client.DefaultRequestHeaders.Add("authorization", authorizationHeader);
            }
        }

    }
}
