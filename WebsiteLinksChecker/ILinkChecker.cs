using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LinksChecker
{
    public interface ILinkChecker
    {
        string ElementId { get; }
        Task<Dictionary<string, HttpResponseMessage>> CheckLinks(string projectBaseUrl, string branch, string markdownContent);
        Task<HttpResponseMessage> GetHttpResponseAsync(string urlWithinPage);
    }
}