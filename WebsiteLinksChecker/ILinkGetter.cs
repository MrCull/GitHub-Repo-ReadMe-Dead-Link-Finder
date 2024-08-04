using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebsiteLinksChecker;

public interface ILinkGetter
{
    Task<List<string>> ExtractUrlsAsync(string projectBaseUrl, string branch, string markdownContent);
}
