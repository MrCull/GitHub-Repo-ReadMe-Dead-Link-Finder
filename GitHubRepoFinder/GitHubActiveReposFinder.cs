using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GitHubRepoFinder
{
    public class GitHubActiveReposFinder : IUriFinder
    {
        private GitHubClient _githubClient;

        public GitHubActiveReposFinder(GitHubClient gitHubClient)
        {
            _githubClient = gitHubClient;
        }

        public IEnumerable<Uri> GetUris(int numberOfUris, SearchRepositoriesRequest searchRepositoriesRequest)
        {
            var uris = new List<Uri>();

            if (numberOfUris < 1)
            {
                throw new GitHubActiveReposFinderException("Cannot look for less than 1 Uris");
            }
            if (numberOfUris > 999)
            {
                throw new GitHubActiveReposFinderException("Cannot look for more than 999 Uris");
            }

            var itemsPerPage = 100; // seems to be a maximum of 100 for GitHub
            int pagesNeeded = (numberOfUris / itemsPerPage) + 1;

            for (int page = 1; page <= pagesNeeded; page++)
            {

                searchRepositoriesRequest.PerPage = itemsPerPage;
                searchRepositoriesRequest.Page = page;

                SearchRepositoryResult result = _githubClient.Search.SearchRepo(searchRepositoriesRequest).Result;

                uris.AddRange(result.Items.Select(repos => new Uri(repos.HtmlUrl)));
            }

            return uris
                .Distinct()
                .Take(numberOfUris); ;
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
}
