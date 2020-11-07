using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebsiteLinksChecker
{
    public interface ILinkChecker
    {
        string ElementId { get; }
        Dictionary<string, HttpResponseMessage> CheckLinks(Uri uri);
        Task<HttpResponseMessage> GetHttpResponseAsync(Uri uriWithinPage);
    }
}