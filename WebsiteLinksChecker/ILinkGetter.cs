using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace WebsiteLinksChecker
{
    public interface ILinkGetter
    {
        List<Uri> GetUrisOutOfPageFromMainUri(Uri uriForMainPage, HtmlDocument htmlDocumente);
    }
}