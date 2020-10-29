using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebsiteLinksChecker
{
    public interface ILinkChecker
    {
        Task<Dictionary<string, HttpResponseMessage>> CheckLinksAsync(Uri urlForMainPage);

        string ElementId { get; }
    }
}