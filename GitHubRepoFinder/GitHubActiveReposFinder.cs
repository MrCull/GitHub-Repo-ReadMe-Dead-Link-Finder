using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GitHubRepoFinder;

public class GitHubActiveReposFinder : IRepoFinder
{
    private readonly GitHubClient _githubClient;

    public GitHubActiveReposFinder(GitHubClient gitHubClient)
    {
        _githubClient = gitHubClient;
    }

    public IEnumerable<RepoSearchResult> GetRepoSearchResults(int numberOfUris, SearchRepositoriesRequest searchRepositoriesRequest)
    {
        List<RepoSearchResult> repoSearchResult = [];

        if (numberOfUris < 1)
        {
            throw new GitHubActiveReposFinderException("Cannot look for less than 1 Uris");
        }
        if (numberOfUris > 999)
        {
            throw new GitHubActiveReposFinderException("Cannot look for more than 999 Uris");
        }



        while (hasMorePages)
        {
            searchRepositoriesRequest.PerPage = itemsPerPage;
            searchRepositoriesRequest.Page = page;

            SearchRepositoryResult result = _githubClient.Search.SearchRepo(searchRepositoriesRequest).Result;


            // If fetching all, continue until we get a page with fewer than 100 items
            if (fetchAll)
            {
                hasMorePages = result.Items.Count == itemsPerPage;
                page++;
            }
            else
            {
                // Otherwise, stop when we have enough
                hasMorePages = repoSearchResult.Count < numberOfUris && result.Items.Count == itemsPerPage;
                if (hasMorePages)
                {
                    page++;
                }
            }
        }

        return repoSearchResult
            .Distinct()
    }

    public void SetGitHubCredentials(Credentials credential)
    {
        _githubClient.Credentials = credential;
    }

    public RateLimit GetRateLimit()
    {
        return _githubClient.GetLastApiInfo().RateLimit;
    }
}
