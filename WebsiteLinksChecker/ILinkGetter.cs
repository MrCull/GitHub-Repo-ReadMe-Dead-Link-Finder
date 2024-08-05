using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinksChecker;

public interface ILinkGetter
{
    Task<List<string>> ExtractUrlsAsync(string projectBaseUrl, string branch, string markdownContent);
}
