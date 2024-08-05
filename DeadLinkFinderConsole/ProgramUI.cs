using GitHubRepoFinder;
using LinksChecker;
using Microsoft.Extensions.Configuration;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DeadLinkFinderConsole;

class ProgramUI
{
    private readonly IConfigurationRoot _configuration;
    private readonly IRepoFinder _repoFinder;
    private readonly ILinkChecker _linkChecker;
    private readonly IFileNameFromUri _fileNameFromUri;
    private readonly SearchRepositoriesRequest _searchRepositoriesRequest;
    private readonly string _outputDirectory;
    private readonly string _webUiSearchPathSingleRepo;
    private readonly string _webUiSearchPathUser;

    public ProgramUI(IConfigurationRoot configuration, IRepoFinder repoFinder, ILinkChecker linkChecker, IFileNameFromUri fileNameFromUri, SearchRepositoriesRequest searchRepositoriesRequest)
    {
        _configuration = configuration;
        _repoFinder = repoFinder;
        _linkChecker = linkChecker;
        _fileNameFromUri = fileNameFromUri;
        _searchRepositoriesRequest = searchRepositoriesRequest;

        _outputDirectory = _configuration["outputDirectory"];
        _webUiSearchPathSingleRepo = _configuration["webUiSearchPathSingleRepo"];
        _webUiSearchPathUser = _configuration["webUiSearchPathUser"];
    }

    public async Task RunAsync()
    {
        try
        {
            int numberOfGitHubPagesToCheck = 1;

            if (_repoFinder.GetType() == typeof(GitHubActiveReposFinder))
            {
                Credentials credential = GetGitHubCredentialsFromUser();
                if (credential != null)
                {
                    _repoFinder.SetGitHubCredentials(credential);
                }

                numberOfGitHubPagesToCheck = GetNumberOfGitHubPagesToCheckFromUser();

                DisplayGitHubSearchRequestParameters(_searchRepositoriesRequest);
            }

            IEnumerable<RepoSearchResult> repoSearchResults = _repoFinder.GetRepoSearchResults(numberOfGitHubPagesToCheck, _searchRepositoriesRequest);

            LogGitHubRateLimitFromThisConnection(_repoFinder.GetRateLimit());


            foreach (RepoSearchResult repoSearchResult in repoSearchResults)
            {
                Console.WriteLine($"Checking: {repoSearchResult.Uri}");

                Dictionary<string, HttpResponseMessage> linkWithResponse = await _linkChecker.CheckLinks(repoSearchResult.Uri.AbsoluteUri, repoSearchResult.Branch);
                SaveOutput(repoSearchResult, linkWithResponse, _outputDirectory);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"Search completed. See [{_outputDirectory}[ for further details of fond GitHub Repos with bad links in their README.md files");

        }
        catch (AuthorizationException ae)
        {
            Console.WriteLine(ae);
        }
    }

    private void DisplayGitHubSearchRequestParameters(SearchRepositoriesRequest searchRepositoriesRequest)
    {
        Console.WriteLine("Searching for Repos with");
        Console.WriteLine($" - Min Stars: {searchRepositoriesRequest.Stars}");
        Console.WriteLine($" - Udated: {searchRepositoriesRequest.Updated}");
        Console.WriteLine($" - SortField {searchRepositoriesRequest.SortField}");
        Console.WriteLine($" - Order {searchRepositoriesRequest.Order}");
        Console.WriteLine();

    }

    private int GetNumberOfGitHubPagesToCheckFromUser()
    {
        int defaultNumberOfGitHubPagesToCheck = 25;

        Console.Clear();
        Console.WriteLine("How many GitHub repo pages do you want to search for bad links?");
        Console.WriteLine($"Enter key to default to {defaultNumberOfGitHubPagesToCheck}");

        string line = Console.ReadLine();
        Console.Clear();

        if (!int.TryParse(line, out int numberOfGitHubPagesToCheck))
        {
            Console.WriteLine("Invalid input");
            numberOfGitHubPagesToCheck = defaultNumberOfGitHubPagesToCheck;
        }

        Console.WriteLine($"Searching for {numberOfGitHubPagesToCheck} GitHub repo pages");
        Console.WriteLine();
        return numberOfGitHubPagesToCheck;
    }

    private void LogGitHubRateLimitFromThisConnection(RateLimit rateLimit)
    {
        Console.WriteLine($"The https://api.github.com http request RateLimit from this connection is Limit[{rateLimit.Limit}] Remaining[{rateLimit.Remaining}] which will reset after[{rateLimit.Reset}]");
        Console.WriteLine();
    }

    private Credentials GetGitHubCredentialsFromUser()
    {
        Credentials credentials = null;

        Console.WriteLine("Optionally providing credentials to GitHub will slightly incrase the throttled HTTP request 'rate limit' and help prevent the [429:TooManyRequests] errors.");
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("Press 1 to use you Personal Access Token (see https://github.com/settings/tokens).");
        //Console.WriteLine("Press 2 to enter GitHub Username and Password");
        Console.WriteLine("Or just press Enter to skip authentication and use github.com's slightly lower default http 'rate limit'.");
        ConsoleKeyInfo authenticationType = Console.ReadKey();
        Console.Clear();

        if (authenticationType.Key is ConsoleKey.D2 or ConsoleKey.NumPad2)
        {
            Console.Write("Enter your user name (or Enter to skip login): ");
            string gitHubUserName = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(gitHubUserName))
            {
                string gitHubPassword = RequestPassword();
                credentials = new Credentials(gitHubUserName, gitHubPassword);
            }
        }
        else if (authenticationType.Key is ConsoleKey.D1 or ConsoleKey.NumPad1)
        {
            Console.Write("Enter github personal access token (see https://github.com/settings/tokens): ");
            string personalAccessToken = Console.ReadLine();
            credentials = new Credentials(personalAccessToken);
        }
        else
        {
            Console.WriteLine("Skipping authentication");
        }
        Console.Clear();

        return credentials;
    }

    private string RequestPassword()
    {
        string password = "";

        Console.Write("Enter your password  : ");
        ConsoleKeyInfo info = Console.ReadKey(true);
        while (info.Key != ConsoleKey.Enter)
        {
            if (info.Key != ConsoleKey.Backspace)
            {
                password += info.KeyChar;
                info = Console.ReadKey(true);
            }
            else if (info.Key == ConsoleKey.Backspace)
            {
                if (!string.IsNullOrEmpty(password))
                {
                    password = password[0..^1];
                }
                info = Console.ReadKey(true);
            }
        }
        return password;
    }

    private void SaveOutput(RepoSearchResult repoSearchResult, Dictionary<string, HttpResponseMessage> linkCheckerResults, string outputDirectory)
    {
        int successLinkCount = linkCheckerResults.Count(lcr => lcr.Value.StatusCode == HttpStatusCode.OK);
        IEnumerable<KeyValuePair<string, HttpResponseMessage>> httpUnSuccessfullResponseMessages = linkCheckerResults.Where(lcr => lcr.Value.StatusCode == HttpStatusCode.NotFound);
        IEnumerable<KeyValuePair<string, HttpResponseMessage>> httpOtherResponseMessages = linkCheckerResults.Where(lcr => lcr.Value.StatusCode is not HttpStatusCode.OK and not HttpStatusCode.NotFound);

        int c = linkCheckerResults.Count - (successLinkCount + httpUnSuccessfullResponseMessages.Count());

        if (httpUnSuccessfullResponseMessages.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        Console.WriteLine($"ok[{successLinkCount}] - bad[{httpUnSuccessfullResponseMessages.Count()}] other: [{httpOtherResponseMessages.Count()}] site: {repoSearchResult}");
        Console.ResetColor();
        Console.WriteLine();
        if (httpUnSuccessfullResponseMessages.Any())
        {
            string logFileName = $@"{_fileNameFromUri.ConvertToWindowsFileName(repoSearchResult.Uri)}.txt";
            Directory.CreateDirectory(outputDirectory);
            string logFilePath = Path.Combine(outputDirectory, logFileName);
            using StreamWriter streamWriter = File.CreateText(logFilePath);

            string logFileHeader = $"At {DateTime.UtcNow:s}Z [{httpUnSuccessfullResponseMessages.Count()}] bad links found in [{repoSearchResult}]";
            streamWriter.WriteLine(logFileHeader);
            streamWriter.WriteLine();

            LogLinkWithStatus(streamWriter, httpUnSuccessfullResponseMessages);
            streamWriter.WriteLine();
            streamWriter.WriteLine();
            streamWriter.WriteLine("Re-check this Repo via: " + WebUiLinkForUri(repoSearchResult.Uri));
            streamWriter.WriteLine("Check all Repos for this GitHub account: " + WebUiLinkForGitHubAccountLinkedToUri(repoSearchResult.Uri));
            streamWriter.WriteLine();
            streamWriter.WriteLine();
            LogLinkWithStatus(streamWriter, httpOtherResponseMessages);
        }
    }

    private string WebUiLinkForGitHubAccountLinkedToUri(Uri uri)
    {
        /// Gets the github user account
        /// e.g for https://github.com/MrCull/GitHub-Repo-ReadMe-Dead-Link-Finder
        /// It will return "MrCull"
        string gitHubUserAccountName = uri.Segments.Skip(1).First().Replace(@"/", "");

        return _webUiSearchPathUser + gitHubUserAccountName;
    }

    private string WebUiLinkForUri(Uri uri)
    {
        return _webUiSearchPathSingleRepo + HttpUtility.UrlEncode(uri.ToString());
    }

    private static void LogLinkWithStatus(StreamWriter streamWriter, IEnumerable<KeyValuePair<string, HttpResponseMessage>> httpResponseMessages)
    {
        foreach (KeyValuePair<string, HttpResponseMessage> linkCheckerResult in httpResponseMessages)
        {
            string httpResponseMessageLog = $"Status code [{linkCheckerResult.Value.StatusCode:D}:{linkCheckerResult.Value.StatusCode}] - Link: {linkCheckerResult.Key}";
            streamWriter.WriteLine(httpResponseMessageLog);
        }
    }
}