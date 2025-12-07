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
        LogTelemetry(repoChecker);

        if (!string.IsNullOrWhiteSpace(repoChecker.SingleRepoUri))
        {
            repoChecker.RepoUrlsAndDefaultBranch.Add(new RepoCheckerModel.RepoUrlAndDefaultBranch { RepoUri = new Uri(repoChecker.SingleRepoUri), Branch = "main" });
        }
        else
        {
            // Simplified workflow: auto-set to get 5 most recent repos
            _searchRepositoriesRequest.SortField = RepoSearchSort.Updated;
            _searchRepositoriesRequest.Order = SortDirection.Descending;
            _searchRepositoriesRequest.Stars = Octokit.Range.GreaterThanOrEquals(0);
            _searchRepositoriesRequest.User = repoChecker.User;

            int maxRepos = repoChecker.NumberOfReposToSearchFor ?? 5;
            // prevent to many repos to search
            if (maxRepos > 25)
            {
                maxRepos = 25;
            }

            if (repoChecker.IncludeForks)
            {
                _searchRepositoriesRequest.Fork = ForkQualifier.IncludeForks;
            }

            IEnumerable<RepoSearchResult> repoSearchResults = _gitHubActiveReposFinder.GetRepoSearchResults(maxRepos, _searchRepositoriesRequest);
            repoChecker.RepoUrlsAndDefaultBranch.AddRange(repoSearchResults.Select(r => new RepoCheckerModel.RepoUrlAndDefaultBranch { RepoUri = r.Uri, Branch = r.Branch }));

        }

        return View("Index", repoChecker);
    }

    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet]
    public JsonResult GetUserRepos(string userOrOrg, bool includeForks = false, int maxRepos = 10000)
    {
        try
        {
            var searchRequest = new SearchRepositoriesRequest();
            searchRequest.User = userOrOrg;
            searchRequest.SortField = RepoSearchSort.Updated;
            searchRequest.Order = SortDirection.Descending;
            searchRequest.Stars = Octokit.Range.GreaterThanOrEquals(0);

            if (includeForks)
            {
                searchRequest.Fork = ForkQualifier.IncludeForks;
            }

            // Fetch all repos - no limit
            IEnumerable<RepoSearchResult> repoSearchResults = _gitHubActiveReposFinder.GetRepoSearchResults(maxRepos, searchRequest);

            var repos = repoSearchResults.Select(r => new
            {
                name = r.Uri.Segments.Last().TrimEnd('/'),
                fullName = r.Uri.ToString().Replace("https://github.com/", ""),
                url = r.Uri.ToString(),
                branch = r.Branch,
                stars = r.Stars,
                watchers = r.Watchers ?? 0,
                forks = r.Forks ?? 0,
                updatedAt = r.UpdatedAt?.ToString("O") ?? null,
                description = r.Description,
                language = r.Language,
                license = r.License,
                topics = r.Topics ?? new List<string>()
            }).ToList();

            return Json(repos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching repos for {UserOrOrg}", userOrOrg);
            return Json(new { error = "Failed to fetch repositories. Please check the username/organization name." });
        }
    }


    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<JsonResult> CheckRepo(string projectBaseUrl, string branch)
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