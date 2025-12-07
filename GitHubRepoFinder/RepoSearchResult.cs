using System;
using System.Collections.Generic;

namespace GitHubRepoFinder;

#nullable enable

public record RepoSearchResult(
    Uri Uri, 
    string Branch, 
    int Stars, 
    DateTimeOffset? UpdatedAt = null, 
    int? Watchers = null, 
    int? Forks = null,
    string? Description = null,
    string? Language = null,
    string? License = null,
    List<string>? Topics = null
);

