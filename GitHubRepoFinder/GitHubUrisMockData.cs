using Octokit;
using System;
using System.Collections.Generic;

namespace GitHubRepoFinder;

/// <summary>
/// This class can be used to DI an object that will return some select hardcoded GitHub repo links.
/// It can be used during developement/debugging when consistent input if helpful. 
/// </summary>
public class GitHubUrisMockData : IRepoFinder
{
    public RateLimit GetRateLimit()
    {
        //stub for mock data class
        return new RateLimit();

    }

    public IEnumerable<RepoSearchResult> GetRepoSearchResults(int numberOfUris, SearchRepositoriesRequest searchRepositoriesRequest)
    {
        // mock data 
        return new List<RepoSearchResult>
            {
                new RepoSearchResult(new Uri("https://github.com/MrCull/MrCullDevTools"), "main", new Random().Next(100)),
                new RepoSearchResult(new Uri("https://github.com/MrCull/GitHubWithNoReadMe"), "main", new Random().Next(100)),
                new RepoSearchResult(new Uri("https://github.com/l1ving/youtube-dl"), "main", new Random().Next(100)),
                new RepoSearchResult(new Uri("https://github.com/EbookFoundation/free-programming-books"), "main", new Random().Next(100))
            };
    }

    public void SetGitHubCredentials(Credentials credential)
    {
        //stub for mock data class
    }
}
