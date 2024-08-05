using System.Threading.Tasks;

namespace LinksChecker;

public interface IMarkdownGetter
{
    Task<string> GetReadMeMarkDownAsync(string rawProjectBaseUrlWithBranch);
}