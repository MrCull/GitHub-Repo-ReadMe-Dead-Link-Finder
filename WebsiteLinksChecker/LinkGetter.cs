using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinksChecker;

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
        // 
        string projectBlobUrl = ConvertProjectUrlToBlobUrl(projectBaseUrl, branch);

        // Regex to extract URLs encapsulated in parentheses, capturing absolute and relative paths
        Regex regex = new(@"\[[^\]]+\]\(((http[^)]+)|([^)]+))\)");
        MatchCollection matches = regex.Matches(markdownContent);

        string urlToAdd = string.Empty;

        // Process each match to extract URLs
        foreach (Match match in matches)
        {
            // Group 1 captures the entire URL in the Markdown link (both absolute and relative)
            string path = match.Groups[1].Value;
            path = path.TrimStart('/').TrimStart('<').TrimStart().TrimEnd('>');

            if (path.StartsWith("http") || path.StartsWith("https"))
            {
                // Absolute URL directly added
                urlToAdd = path;
            }
            else if (!path.StartsWith("mailto"))
            {
                // Relative URL, need to create a full URL
                string relativePath = path.TrimStart('/').TrimStart('<');
                Uri absoluteUrl = new($"{projectBlobUrl}/{relativePath}");
                urlToAdd = absoluteUrl.ToString();
            }

            if (!string.IsNullOrEmpty(urlToAdd) && !urls.Contains(urlToAdd))
            {
                urls.Add(urlToAdd);
            }
        }

        return urls;
    }

    /// <summary>
    /// Converts:
    /// https://github.com/user/project/
    /// To
    /// https://github.com/user/project/blob/main
    /// </summary>
    /// <param name="projectBaseUrl"></param>
    /// <param name="branch"></param>
    /// <returns></returns>
    private string ConvertProjectUrlToBlobUrl(string projectBaseUrl, string branch)
    {
        string blobUrl = projectBaseUrl;
        blobUrl += $"/blob/{branch}";
        return blobUrl;
    }
}
