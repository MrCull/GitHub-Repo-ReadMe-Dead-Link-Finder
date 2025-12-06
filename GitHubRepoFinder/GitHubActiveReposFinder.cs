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

        // If requesting a very large number (e.g., 10000), fetch all repos via pagination
        bool fetchAll = numberOfUris >= 10000;

        int itemsPerPage = 100; // Maximum allowed by GitHub
        int page = 1;
        bool hasMorePages = true;

        while (hasMorePages)
        {
            searchRepositoriesRequest.PerPage = itemsPerPage;
            searchRepositoriesRequest.Page = page;

            SearchRepositoryResult result = _githubClient.Search.SearchRepo(searchRepositoriesRequest).Result;

            if (result.Items.Count == 0)
            {
                break;
            }

            repoSearchResult.AddRange(result.Items.Select(repo => new RepoSearchResult(
                Uri: new Uri(repo.HtmlUrl),
                Branch: repo.DefaultBranch,
                Stars: repo.StargazersCount,
                UpdatedAt: repo.UpdatedAt,
                Watchers: repo.SubscribersCount,
                Forks: repo.ForksCount,
                Description: repo.Description,
                Language: repo.Language,
                License: repo.License?.SpdxId ?? repo.License?.Name,
                Topics: repo.Topics?.ToList()
            )));

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
            .Take(fetchAll ? repoSearchResult.Count : numberOfUris);
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
