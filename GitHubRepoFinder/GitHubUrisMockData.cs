using Octokit;
using System;
using System.Collections.Generic;

namespace GitHubRepoFinder
{
    /// <summary>
    /// This class can be used to DI an object that will return some select hardcoded GitHub repo links.
    /// It can be used during developement/debugging when consistent input if helpful. 
    /// </summary>
    public class GitHubUrisMockData : IUriFinder
    {
        public RateLimit GetRateLimit()
        {
            //stub for mock data class
            return new RateLimit();

        }

        public IEnumerable<Uri> GetUris(int numberOfUris, SearchRepositoriesRequest searchRepositoriesRequest)
        {
            var uris = new List<Uri>
            {
                new Uri("https://github.com/MrCull/MrCullDevTools"),
                new Uri("https://github.com/MrCull/GitHubWithNoReadMe"),
                new Uri("https://github.com/l1ving/youtube-dl"),
                new Uri("https://github.com/EbookFoundation/free-programming-books")

            };
            return uris;
        }

        public void SetGitHubCredentials(Credentials credential)
        {
            //stub for mock data class
        }
    }
}
