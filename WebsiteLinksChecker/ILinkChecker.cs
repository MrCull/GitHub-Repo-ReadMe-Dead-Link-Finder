using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LinksChecker;

public interface ILinkChecker
{
    Task<Dictionary<string, HttpResponseMessage>> CheckLinks(string projectBaseUrl, string branch);
    Task<HttpResponseMessage> GetHttpResponseAsync(string urlWithinPage);
}