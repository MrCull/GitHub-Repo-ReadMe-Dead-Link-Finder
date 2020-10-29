using System;

namespace GitHubRepoFinder
{
    public class GitHubActiveReposFinderException : Exception
    {
        public GitHubActiveReposFinderException(string message) : base(message)
        {
        }
    }
}
