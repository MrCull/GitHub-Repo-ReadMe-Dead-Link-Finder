using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebsiteLinksChecker;

public class LinkGetter : ILinkGetter
{
    /// <summary>
    /// This method serves as the async wrapper for the URL extraction process.
    /// </summary>
    /// <param name="projectBaseUrl">Base URL of the project.</param>
    /// <param name="branch">Branch name for the repository.</param>
    /// <param name="markdownContent">Markdown content as a string.</param>
    /// <returns>List of URLs extracted from the markdown.</returns>
    public async Task<List<string>> ExtractUrlsAsync(string projectBaseUrl, string branch, string markdownContent)
    {
        return await Task.Run(() => ExtractUrls(projectBaseUrl, branch, markdownContent));
    }

    private List<string> ExtractUrls(string projectBaseUrl, string branch, string markdownContent)
    {
        List<string> urls = [];

        // Convert project base URL to a raw content URL for GitHub
        Uri rawBase = TransformProjectUrlToRaw(projectBaseUrl, branch);

        // Regex to extract URLs encapsulated in parentheses, capturing absolute and relative paths
        Regex regex = new(@"\((http[^)]+)|\(([^)]+)\)");
        MatchCollection matches = regex.Matches(markdownContent);

        // Process each match to extract URLs
        foreach (Match match in matches)
        {
            // Group 1 captures absolute URLs (http:// or https://)
            if (match.Groups[1].Success)
            {
                string url = match.Groups[1].Value;
                urls.Add(url);
            }
            // Group 2 captures relative paths that start with '/'
            else if (match.Groups[2].Success)
            {
                string relativePath = match.Groups[2].Value.TrimStart('/');
                Uri absoluteUrl = new(rawBase, relativePath);
                urls.Add(absoluteUrl.ToString());
            }
        }

        return urls;
    }

    /// <summary>
    /// Convert the GitHub project URL to a raw content URL for direct file access. 
    /// For example, if the project URL is "https://github.com/user/repository/",
    /// this method transforms it to "https://github.com/user/repository/blob/main/",
    /// which is the base URL format for accessing raw content from the GitHub repository.
    /// </summary>
    /// <param name="projectBaseUrl">The project's base URL.</param>
    /// <param name="branch">The repository branch.</param>
    /// <returns>Uri object of the transformed base URL.</returns>
    private static Uri TransformProjectUrlToRaw(string projectBaseUrl, string branch)
    {
        Uri projectBaseUri = new(projectBaseUrl);
        string[] segments = projectBaseUri.AbsolutePath.Trim('/').Split('/');
        string user = segments[0];
        string repository = segments[1];
        string rawUrl = $"https://github.com/{user}/{repository}/blob/{branch}/";
        return new Uri(rawUrl);
    }
}
