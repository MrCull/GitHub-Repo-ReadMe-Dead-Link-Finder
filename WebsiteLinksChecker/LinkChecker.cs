using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinksChecker;

public class LinkChecker(HttpClient httpClient, ILinkGetter linkGetter, IMarkdownGetter markdownGetter) : ILinkChecker
{
    public async Task<Dictionary<string, HttpResponseMessage>> CheckLinks(string projectBaseUrl, string branch)
    {
        Dictionary<string, HttpResponseMessage> results = [];
        try
        {
            string rawProjectUrl = ConvertProjectUrlToRawUrl(projectBaseUrl, branch);
            string markdownContent = await markdownGetter.GetReadMeMarkDownAsync(rawProjectUrl);
            List<string> urisFromWithinPageOfMainUri = await linkGetter.ExtractUrlsAsync(projectBaseUrl, branch, markdownContent);
            results = CheckUrisHttpStatus(urisFromWithinPageOfMainUri);

        }
        catch (HttpRequestException hre)
        {
            Console.WriteLine($"HttpRequestException Error for {projectBaseUrl}");
            Console.WriteLine(hre);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error for {projectBaseUrl}");
            Console.WriteLine(ex);
        }

        return results;
    }

    private static string ConvertProjectUrlToRawUrl(string projectBaseUrl, string branch)
    {
        string rawUrl = projectBaseUrl.Replace("github.com", "raw.githubusercontent.com");
        rawUrl += $"/{branch}";
        return rawUrl;
    }

    private Dictionary<string, HttpResponseMessage> CheckUrisHttpStatus(List<string> urlsToCheck)
    {
        Dictionary<string, HttpResponseMessage> results = [];

        try
        {
            Dictionary<string, Task<HttpResponseMessage>> taskList = [];

            foreach (string urlTocheck in urlsToCheck)
            {

                try
                {
                    if (!taskList.ContainsKey(urlTocheck))
                    {
                        Task<HttpResponseMessage> response = GetHttpResponseAsync(urlTocheck);
                        taskList.Add(urlTocheck, response);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Task.WaitAll(taskList.Values.ToArray(), millisecondsTimeout: 120000);

            foreach (string checkedUrl in urlsToCheck)
            {
                if (taskList[checkedUrl].IsCompleted)
                {
                    HttpResponseMessage httpResponseMessage = taskList[checkedUrl].Result;
                    if (httpResponseMessage != null)
                    {
                        results.Add(checkedUrl, httpResponseMessage);
                    }
                }
                else
                {
                    results.Add(checkedUrl, new HttpResponseMessage(HttpStatusCode.RequestTimeout));
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return results;
    }


    public async Task<HttpResponseMessage> GetHttpResponseAsync(string urlWithinPage)
    {
        HttpResponseMessage httpResponseMessage = null;
        int loopLimit = 30;
        int numberOfLoops = 0;
        Random random = new();
        try
        {
            HttpStatusCode statusCode = HttpStatusCode.OK;
            do
            {
                numberOfLoops += 1;
                if (numberOfLoops > loopLimit)
                {
                    throw new TooManyRequestsLoopsException($"Loop limit exceeded for 429:TooManyRequests for {urlWithinPage}");
                }

                statusCode = HttpStatusCode.OK;
                try
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");

                    httpResponseMessage = await httpClient.GetAsync(urlWithinPage);
                    statusCode = httpResponseMessage.StatusCode;
                }
                catch (HttpRequestException hre)
                {
                    if (hre.InnerException.Message == "A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond.")
                    {
                        statusCode = HttpStatusCode.TooManyRequests;
                    }
                    else
                    {
                        Console.WriteLine($"Suppressing error {hre.InnerException.Message} from {urlWithinPage}");
                    }
                }

                if (statusCode == HttpStatusCode.TooManyRequests)
                {
                    // Make this task thread sleep between 1 and x seconds to try and not overload the server again
                    int sleepSeconds = random.Next(3, 15);
                    Console.WriteLine($"{HttpStatusCode.TooManyRequests} received from server for {urlWithinPage} so sleeping for {sleepSeconds} seconds before trying again");
                    Thread.Sleep(sleepSeconds * 1000);
                }
            } while (statusCode == HttpStatusCode.TooManyRequests);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.GetType());
            throw;

        }

        return httpResponseMessage;
    }

}
