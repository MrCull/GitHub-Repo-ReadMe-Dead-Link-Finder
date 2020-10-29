using HtmlAgilityPack;
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

        public string ElementId { get; private set; }

        public LinkChecker(HttpClient httpClient, string elementId)
        {
            _httpClient = httpClient;
            ElementId = elementId;
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
                    HtmlDocument htmlDocumente = GetHtmlDocument(rawHtml);
                    results = CheckLinks(uri, htmlDocumente);
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

        private Dictionary<string, HttpResponseMessage> CheckLinks(Uri uriForMainPage, HtmlDocument htmlDocumente)
        {
            var results = new Dictionary<string, HttpResponseMessage>();

            try
            {
                var checkedLinks = new List<string>();

                var taskList = new Dictionary<string, Task<HttpResponseMessage>>();

                HtmlNodeCollection htmlDocumentes = htmlDocumente.DocumentNode.SelectNodes("//a[@href]");

                // If there are no links then htmlDocumentes will be null
                if (htmlDocumentes != null)
                {
                    foreach (HtmlNode link in htmlDocumentes)
                    {
                        string uriWithinPage = link.Attributes["href"].Value;

                        // If it's relative uri then combine with host path
                        if (uriWithinPage.StartsWith(@"/"))
                        {
                            uriWithinPage = uriForMainPage.Scheme + Uri.SchemeDelimiter + uriForMainPage.Host + uriWithinPage;
                        }

                        if (Uri.IsWellFormedUriString(uriWithinPage, UriKind.RelativeOrAbsolute) && !uriWithinPage.Contains("mailto"))
                        {
                            if (!checkedLinks.Contains(uriWithinPage))
                            {
                                checkedLinks.Add(uriWithinPage);

                                try
                                {
                                    Task<HttpResponseMessage> response = GetHttpResponseAsync(uriWithinPage);
                                    taskList.Add(uriWithinPage, response);

                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                            }
                        }
                    }
                }

                Task.WaitAll(taskList.Values.ToArray());

                foreach (string checkedLink in checkedLinks)
                {
                    HttpResponseMessage httpResponseMessage = taskList[checkedLink].Result;
                    if (httpResponseMessage != null)
                    {
                        results.Add(checkedLink, httpResponseMessage);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return results;
        }


        private async Task<HttpResponseMessage> GetHttpResponseAsync(string urlWithinPage)
        {
            HttpResponseMessage httpResponseMessage = null;
            try
            {
                httpResponseMessage = await _httpClient.GetAsync(urlWithinPage);
            }
            catch (HttpRequestException hre)
            {
                Console.WriteLine($"Suppressing error {hre.InnerException.Message} from {urlWithinPage}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType());
                throw;

            }

            return httpResponseMessage;
        }

        private HtmlDocument GetHtmlDocument(string rawHtml)
        {
            var htmlDocumente = new HtmlDocument();
            htmlDocumente.LoadHtml(rawHtml);

            if (ElementId != null)
            {
                HtmlNode documentElement = htmlDocumente.GetElementbyId(ElementId);
                if (documentElement == null)
                {
                    throw new ElementIdNotFoundException($"ElementId [{ElementId}] not found in document");
                }
                htmlDocumente.LoadHtml(documentElement.InnerHtml);
            }

            return htmlDocumente;
        }
    }
}
