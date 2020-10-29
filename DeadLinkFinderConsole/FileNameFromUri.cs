using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DeadLinkFinderConsole
{
    /// <summary>
    /// Found on GitHub Gits with a few further changes made...
    /// https://gist.github.com/vkbandi/4442c9cc2e60d2edd04c
    /// </summary>
    public class FileNameFromUri : IFileNameFromUri
    {
        public string ConvertToWindowsFileName(Uri uri)
        {
            Regex regex = new Regex(@"[a-z0-9.]+", RegexOptions.IgnoreCase);

            List<string> excludeList = new List<string> { "www", "http", "https" };

            var urlParts = new List<string>();
            foreach (Match m in regex.Matches(uri.ToString()))
            {
                if (!excludeList.Contains(m.Value))
                {
                    urlParts.Add(m.Value);
                }
            }

            return string.Join("_", urlParts);
        }
    }
}