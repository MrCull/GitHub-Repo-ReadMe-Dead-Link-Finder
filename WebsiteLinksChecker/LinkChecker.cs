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
            int loopLimit = 30;
            int numberOfLoops = 0;
            var random = new Random();
            try
            {
                do
                {
                    numberOfLoops += 1;
                    if (numberOfLoops > loopLimit)
                    {
                        throw new TooManyRequestsLoopsException($"Loop limit exceeded for 429:TooManyRequests for {uriWithinPage}");
                    }

                    httpResponseMessage = await _httpClient.GetAsync(uriWithinPage);

                    if (httpResponseMessage.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        // Make this task thread sleep between 1 and x seconds to try and not overload the server again
                        int sleepSeconds = random.Next(1, 5);
                        Console.WriteLine($"{HttpStatusCode.TooManyRequests} received from server for {uriWithinPage} so sleeping for {sleepSeconds} seconds before trying again");
                        Thread.Sleep(sleepSeconds * 1000);
                    }
                } while (httpResponseMessage.StatusCode == HttpStatusCode.TooManyRequests);

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
