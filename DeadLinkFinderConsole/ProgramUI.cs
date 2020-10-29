using GitHubRepoFinder;
using Microsoft.Extensions.Configuration;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebsiteLinksChecker;

namespace DeadLinkFinderConsole
{
    class ProgramUI
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IUriFinder _uriFinder;
        private readonly ILinkChecker _linkChecker;
        private readonly IFileNameFromUri _fileNameFromUri;
        private readonly SearchRepositoriesRequest _searchRepositoriesRequest;
        private readonly string _outputDirectory;


        public ProgramUI(IConfigurationRoot configuration, IUriFinder uriFinder, ILinkChecker linkChecker, IFileNameFromUri fileNameFromUri, SearchRepositoriesRequest searchRepositoriesRequest)
        {
            _configuration = configuration;
            _uriFinder = uriFinder;
            _linkChecker = linkChecker;
            _fileNameFromUri = fileNameFromUri;
            _searchRepositoriesRequest = searchRepositoriesRequest;

            _outputDirectory = _configuration["outputDirectory"];
        }

        public async Task RunAsync()
        {
            try
            {
                int numberOfGitHubPagesToCheck = 1;

                if (_uriFinder.GetType() == typeof(GitHubActiveReposFinder))
                {
                    Credentials credential = GetGitHubCredentialsFromUser();
                    if (credential != null)
                    {
                        _uriFinder.SetGitHubCredentials(credential);
                    }

                    numberOfGitHubPagesToCheck = GetNumberOfGitHubPagesToCheckFromUser();

                    DisplayGitHubSearchRequestParameters(_searchRepositoriesRequest);
                }

                IEnumerable<Uri> uris = _uriFinder.GetUris(numberOfGitHubPagesToCheck, _searchRepositoriesRequest);

                LogGitHubRateLimitFromThisConnection(_uriFinder.GetRateLimit());


                foreach (Uri uri in uris)
                {
                    Console.WriteLine($"Checking: {uri}");
                    Dictionary<string, HttpResponseMessage> linkWithResponse = await _linkChecker.CheckLinksAsync(uri);
                    //SaveOutput(uri, linkWithResponse, _configuration["outputDirectory"]);
                    SaveOutput(uri, linkWithResponse, _outputDirectory);
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
            Console.WriteLine($" - Min Stars: {searchRepositoriesRequest.Stars.ToString()}");
            Console.WriteLine($" - Udated: {searchRepositoriesRequest.Updated.ToString()}");
            Console.WriteLine($" - SortField {searchRepositoriesRequest.SortField.ToString()}");
            Console.WriteLine($" - Order {searchRepositoriesRequest.Order.ToString()}");
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

            if (authenticationType.Key == ConsoleKey.D2 || authenticationType.Key == ConsoleKey.NumPad2)
            {
                Console.Write("Enter your user name (or Enter to skip login): ");
                string gitHubUserName = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(gitHubUserName))
                {
                    string gitHubPassword = RequestPassword();
                    credentials = new Credentials(gitHubUserName, gitHubPassword);
                }
            }
            else if (authenticationType.Key == ConsoleKey.D1 || authenticationType.Key == ConsoleKey.NumPad1)
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

        private void SaveOutput(Uri uri, Dictionary<string, HttpResponseMessage> linkCheckerResults, string outputDirectory)
        {
            int successLinkCount = linkCheckerResults.Count(lcr => lcr.Value.StatusCode == HttpStatusCode.OK);
            IEnumerable<KeyValuePair<string, HttpResponseMessage>> httpUnSuccessfullResponseMessages = linkCheckerResults.Where(lcr => lcr.Value.StatusCode == HttpStatusCode.NotFound);
            IEnumerable<KeyValuePair<string, HttpResponseMessage>> httpOtherResponseMessages = linkCheckerResults.Where(lcr => lcr.Value.StatusCode != HttpStatusCode.OK && lcr.Value.StatusCode != HttpStatusCode.NotFound);

            int c = linkCheckerResults.Count - (successLinkCount + httpUnSuccessfullResponseMessages.Count());

            if (httpUnSuccessfullResponseMessages.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.WriteLine($"ok[{successLinkCount}] - bad[{httpUnSuccessfullResponseMessages.Count()}] other: [{httpOtherResponseMessages.Count()}] site: {uri}");
            Console.ResetColor();
            Console.WriteLine();
            if (httpUnSuccessfullResponseMessages.Any())
            {
                string logFileName = $@"{_fileNameFromUri.ConvertToWindowsFileName(uri)}.txt";
                Directory.CreateDirectory(outputDirectory);
                string logFilePath = Path.Combine(outputDirectory, logFileName);
                using (StreamWriter streamWriter = File.CreateText(logFilePath))
                {

                    var logFileHeader = $"At {DateTime.UtcNow.ToString("s")}Z [{httpUnSuccessfullResponseMessages.Count()}] bad links found in [{uri}] under document section[{_linkChecker.ElementId}]";
                    streamWriter.WriteLine(logFileHeader);
                    streamWriter.WriteLine();

                    LogLinkWithStatus(streamWriter, httpUnSuccessfullResponseMessages);
                    streamWriter.WriteLine();
                    streamWriter.WriteLine();
                    LogLinkWithStatus(streamWriter, httpOtherResponseMessages);
                }
            }
        }

        private static void LogLinkWithStatus(StreamWriter streamWriter, IEnumerable<KeyValuePair<string, HttpResponseMessage>> httpResponseMessages)
        {
            foreach (KeyValuePair<string, HttpResponseMessage> linkCheckerResult in httpResponseMessages)
            {
                string httpResponseMessageLog = $"Status code [{linkCheckerResult.Value.StatusCode:D}:{linkCheckerResult.Value.StatusCode}] - Link [{linkCheckerResult.Key}]";
                streamWriter.WriteLine(httpResponseMessageLog);
            }
        }
    }
}