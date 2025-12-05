using System;

namespace GitHubRepoFinder;

public record RepoSearchResult(Uri Uri, string Branch, int Stars, DateTimeOffset? UpdatedAt = null, int? Watchers = null, int? Forks = null);

