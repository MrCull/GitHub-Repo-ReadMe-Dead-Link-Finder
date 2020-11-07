using System;
using System.Collections.Generic;

namespace WebsiteLinksChecker
{
    public interface ILinkGetter
    {
        string ElementId { get; }

        List<Uri> GetUrisOutOfPageFromMainUri(Uri uriForMainPage);
    }
}