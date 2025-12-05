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

        int itemsPerPage = 100; // seems to be a maximum of 100 for GitHub
        int pagesNeeded = (numberOfUris / itemsPerPage) + 1;

        for (int page = 1; page <= pagesNeeded; page++)
        {

            searchRepositoriesRequest.PerPage = itemsPerPage;
            searchRepositoriesRequest.Page = page;

            SearchRepositoryResult result = _githubClient.Search.SearchRepo(searchRepositoriesRequest).Result;

            repoSearchResult.AddRange(result.Items.Select(repo => new RepoSearchResult(new Uri(repo.HtmlUrl), repo.DefaultBranch, repo.StargazersCount, repo.UpdatedAt)));

        }

        return repoSearchResult
            .Distinct()
            .Take(numberOfUris);
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
