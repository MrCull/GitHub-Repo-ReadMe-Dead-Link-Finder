using DeadLinkFinderWeb.Models;
using GitHubRepoFinder;
using LinksChecker;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TelemetryLib;

namespace DeadLinkFinderWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ILinkGetter _linkGetter;
    private readonly ILinkChecker _linkChecker;
    private readonly SearchRepositoriesRequest _searchRepositoriesRequest;
    private readonly GitHubActiveReposFinder _gitHubActiveReposFinder;
    private readonly ITelemetry _telemetry;

    public HomeController(ILogger<HomeController> logger, ILinkGetter linkGetter, ILinkChecker linkChecker, SearchRepositoriesRequest searchRepositoriesRequest,
        GitHubActiveReposFinder gitHubActiveReposFinder, ITelemetry telemetry)
    {
        _logger = logger;
        _linkGetter = linkGetter;
        _linkChecker = linkChecker;
        _searchRepositoriesRequest = searchRepositoriesRequest;
        _gitHubActiveReposFinder = gitHubActiveReposFinder;
        _telemetry = telemetry;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Search(RepoCheckerModel repoChecker)
    {
        return View("Index", ApiSearch(repoChecker));
    }

    public object ApiSearch(RepoCheckerModel repoChecker)
    {
        LogTelemetry(repoChecker);

        if (!string.IsNullOrWhiteSpace(repoChecker.SingleRepoUri))
        {
            repoChecker.RepoUrlsAndDefaultBranch.Add(new RepoCheckerModel.RepoUrlAndDefaultBranch { RepoUri = new Uri(repoChecker.SingleRepoUri), Branch = "main" });
        }
        else
        {
            if (repoChecker.SearchSort != null)
            {
                _searchRepositoriesRequest.SortField = (RepoSearchSort)repoChecker.SearchSort;
            }

            if (repoChecker.SortAscDsc == null)
            {
                _searchRepositoriesRequest.Order = SortDirection.Descending;
            }
            else
            {
                _searchRepositoriesRequest.Order = (SortDirection)repoChecker.SortAscDsc;
            }

            if (repoChecker.MinStar.HasValue && repoChecker.MinStar >= 0)
            {
                _searchRepositoriesRequest.Stars = Octokit.Range.GreaterThanOrEquals(repoChecker.MinStar.Value);
            }
            else
            {
                _searchRepositoriesRequest.Stars = Octokit.Range.GreaterThanOrEquals(0);
            }

            if (repoChecker.UpdatedAfter.HasValue && repoChecker.UpdatedAfter < DateTime.UtcNow)
            {
                _searchRepositoriesRequest.Updated = DateRange.Between(repoChecker.UpdatedAfter.Value.ToUniversalTime(), DateTimeOffset.UtcNow);
            }

            _searchRepositoriesRequest.User = repoChecker.User;

            int maxRepos = repoChecker.NumberOfReposToSearchFor ?? 5;
            // prevent to many repos to search
            if (maxRepos > 25)
            {
                maxRepos = 2;
            }

            if (repoChecker.IncludeForks)
            {
                _searchRepositoriesRequest.Fork = ForkQualifier.IncludeForks;
            }

            IEnumerable<RepoSearchResult> repoSearchResults = _gitHubActiveReposFinder.GetRepoSearchResults(maxRepos, _searchRepositoriesRequest);
            repoChecker.RepoUrlsAndDefaultBranch.AddRange(repoSearchResults.Select(r => new RepoCheckerModel.RepoUrlAndDefaultBranch { RepoUri = r.Uri, Branch = r.Branch }));

        }

        return Json(repoChecker);
    }


    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<JsonResult> CheckRepoAsync(string projectBaseUrl, string branch)
    {
        Dictionary<string, HttpResponseMessage> linkWithResponse = await _linkChecker.CheckLinks(projectBaseUrl, branch);

        List<UriStatus> uriStatuses = [];

        foreach (KeyValuePair<string, HttpResponseMessage> item in linkWithResponse)
        {
            uriStatuses.Add(new UriStatus { UriText = item.Key, HttpStatusCode = item.Value.StatusCode, HttpStatusCodeText = item.Value.StatusCode.ToString() });
        }

        return Json(uriStatuses);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private void LogTelemetry(RepoCheckerModel repoChecker)
    {
        try
        {
            StringBuilder logText = new();
            logText.AppendLine("DatetimeUtc: " + DateTime.UtcNow.ToString());
            logText.AppendLine("IP: " + HttpContext.Connection.RemoteIpAddress.ToString());
            logText.AppendLine("Search info:");
            logText.AppendLine(repoChecker.ToString());

            _telemetry.RecordSearch(logText.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

    }

}
