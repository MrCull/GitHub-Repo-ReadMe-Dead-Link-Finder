using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebsiteLinksChecker
{
    public class LinkChecker : ILinkChecker
    {
        private readonly HttpClient _httpClient;
        private readonly ILinkGetter _linkGetter;

        public string ElementId { get; private set; }

        public LinkChecker(HttpClient httpClient, ILinkGetter linkGetter)
        {
            _httpClient = httpClient;
            _linkGetter = linkGetter;
            ElementId = linkGetter.ElementId;
        }

        public Dictionary<string, HttpResponseMessage> CheckLinks(Uri uri)
        {
            var results = new Dictionary<string, HttpResponseMessage>();
            try
            {
                List<Uri> urisFromWithinPageOfMainUri = _linkGetter.GetUrisOutOfPageFromMainUri(uri);
                results = CheckUrisHttpStatus(urisFromWithinPageOfMainUri);

            }
            catch (HttpRequestException hre)
            {
                Console.WriteLine($"HttpRequestException Error for {uri}");
                Console.WriteLine(hre);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error for {uri}");
                Console.WriteLine(ex);
            }

            return results;
        }


        private Dictionary<string, HttpResponseMessage> CheckUrisHttpStatus(List<Uri> urisToCheck)
        {
            var results = new Dictionary<string, HttpResponseMessage>();

            try
            {
                var taskList = new Dictionary<string, Task<HttpResponseMessage>>();

                foreach (Uri uriTocheck in urisToCheck)
                {

                    try
                    {
                        Task<HttpResponseMessage> response = GetHttpResponseAsync(uriTocheck);
                        taskList.Add(uriTocheck.AbsoluteUri, response);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                Task.WaitAll(taskList.Values.ToArray());

                foreach (Uri checkedUri in urisToCheck)
                {
                    HttpResponseMessage httpResponseMessage = taskList[checkedUri.AbsoluteUri].Result;
                    if (httpResponseMessage != null)
                    {
                        results.Add(checkedUri.AbsoluteUri, httpResponseMessage);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return results;
        }


        public async Task<HttpResponseMessage> GetHttpResponseAsync(Uri uriWithinPage)
        {
            HttpResponseMessage httpResponseMessage = null;
            try
            {
                httpResponseMessage = await _httpClient.GetAsync(uriWithinPage);
            }
            catch (HttpRequestException hre)
            {
                Console.WriteLine($"Suppressing error {hre.InnerException.Message} from {uriWithinPage}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType());
                throw;

            }

            return httpResponseMessage;
        }

    }
}
