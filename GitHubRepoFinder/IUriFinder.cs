using Octokit;
using System;
using System.Collections.Generic;

namespace GitHubRepoFinder
{
    public interface IUriFinder
    {
        public IEnumerable<Uri> GetUris(int numberOfUris, SearchRepositoriesRequest searchRepositoriesRequest);

        public void SetGitHubCredentials(Credentials credential);

        public RateLimit GetRateLimit();

    }
}