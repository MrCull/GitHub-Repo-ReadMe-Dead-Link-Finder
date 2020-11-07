using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
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
        }

        public async Task<Dictionary<string, HttpResponseMessage>> CheckLinksAsync(Uri uri)
        {
            var results = new Dictionary<string, HttpResponseMessage>();
            try
            {
                HttpResponseMessage httpResponseMessage;
                HttpStatusCode httpStatusCode = HttpStatusCode.OK;
                int loopLimit = 10;
                int numberOfLoops = 0;
                do
                {
                    numberOfLoops = +1;
                    if (numberOfLoops > loopLimit)
                    {
                        throw new TooManyRequestsLoopsException($"Loop limit exceeded for 429:TooManyRequests for {uri}");
                    }

                    httpResponseMessage = await _httpClient.GetAsync(uri);
                    if (httpStatusCode == HttpStatusCode.TooManyRequests)
                    {
                        Console.WriteLine($"{HttpStatusCode.TooManyRequests} received from server for {uri} so sleeping for 10 seconds before trying again");
                        Thread.Sleep(10000);
                    }
                } while (httpStatusCode == HttpStatusCode.TooManyRequests);

                string rawHtml = await httpResponseMessage.Content.ReadAsStringAsync();

                try
                {
                    List<Uri> urisFromWithinPageOfMainUri = _linkGetter.GetUrisOutOfPageFromMainUri(uri);
                    results = CheckUrisHttpStatus(urisFromWithinPageOfMainUri);
                }
                catch (ElementIdNotFoundException exception)
                {
                    Console.WriteLine($"For {uri} {exception.Message}");
                }

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


        private async Task<HttpResponseMessage> GetHttpResponseAsync(Uri uriWithinPage)
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
