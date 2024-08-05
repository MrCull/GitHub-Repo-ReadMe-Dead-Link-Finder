using System;

namespace GitHubRepoFinder;

public record RepoSearchResult(Uri Uri, string Branch, int Stars);

