using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebsiteLinksChecker
{
    public class LinkGetter : ILinkGetter
    {

        public List<Uri> GetUrisOutOfPageFromMainUri(Uri uriForMainPage, HtmlDocument htmlDocumente)
        {
            var urisFoundWithinPageFromMainUri = new List<Uri>();

            try
            {
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

    }
}
