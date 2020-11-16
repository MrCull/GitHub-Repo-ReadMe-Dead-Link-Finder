using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteLinksChecker
{
    public class LinkGetter : ILinkGetter
    {
        private readonly HttpClient _httpClient;
        public string ElementId { get; private set; }


        public LinkGetter(HttpClient httpClient, string elementId)
        {
            _httpClient = httpClient;
            ElementId = elementId;
        }

        public List<Uri> GetUrisOutOfPageFromMainUri(Uri uriForMainPage)
        {
            var urisFoundWithinPageFromMainUri = new List<Uri>();

            try
            {
                HtmlDocument htmlDocumente = GetHtmlDocumentFromUri(uriForMainPage).Result;
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
                            if (!urisFoundWithinPageFromMainUri.Contains(new Uri(uriWithinPage)))
                            {
                                urisFoundWithinPageFromMainUri.Add(new Uri(uriWithinPage));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return urisFoundWithinPageFromMainUri;
        }

        private async Task<HtmlDocument> GetHtmlDocumentFromUri(Uri uriForMainPage)
        {
            HttpResponseMessage httpResponseMessage;
            HttpStatusCode httpStatusCode = HttpStatusCode.OK;
            int loopLimit = 10;
            int numberOfLoops = 0;
            do
            {
                numberOfLoops += 1;
                if (numberOfLoops > loopLimit)
                {
                    throw new TooManyRequestsLoopsException($"Loop limit exceeded for 429:TooManyRequests for {uriForMainPage}");
                }

                httpResponseMessage = await _httpClient.GetAsync(uriForMainPage);
                if (httpStatusCode == HttpStatusCode.TooManyRequests)
                {
                    Console.WriteLine($"{HttpStatusCode.TooManyRequests} received from server for {uriForMainPage} so sleeping for 10 seconds before trying again");
                    Thread.Sleep(10000);
                }
            } while (httpStatusCode == HttpStatusCode.TooManyRequests);

            string rawHtml = await httpResponseMessage.Content.ReadAsStringAsync();

            HtmlDocument htmlDocumente = GetHtmlDocument(rawHtml);

            return htmlDocumente;
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
