using System.Net;

namespace DeadLinkFinderWeb.Models;

public class UriStatus
{
    public string UriText { get; set; }
    public HttpStatusCode HttpStatusCode { get; set; }
    public string HttpStatusCodeText { get; set; }
}
