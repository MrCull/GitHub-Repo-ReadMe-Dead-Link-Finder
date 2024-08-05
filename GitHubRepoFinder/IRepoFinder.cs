using Octokit;
using System.Collections.Generic;

namespace GitHubRepoFinder;

public interface IRepoFinder
{
    public IEnumerable<RepoSearchResult> GetRepoSearchResults(int numberOfUris, SearchRepositoriesRequest searchRepositoriesRequest);

    public void SetGitHubCredentials(Credentials credential);

    public RateLimit GetRateLimit();

}