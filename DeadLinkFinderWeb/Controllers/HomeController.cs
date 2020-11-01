using DeadLinkFinderWeb.Models;
using GitHubRepoFinder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using WebsiteLinksChecker;

namespace DeadLinkFinderWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Search(RepoCheckerModel repoChecker)
        {
            var gitHubClient = new GitHubClient(new ProductHeaderValue("GitHub-repo-finder-for-dead-links-in-readmes-web"));
            var uriFinder = new GitHubActiveReposFinder(gitHubClient);

            if (!string.IsNullOrWhiteSpace(repoChecker.SingleRepoUri))
            {
                repoChecker.Uris.Add(new Uri(repoChecker.SingleRepoUri));
            }
            else
            {
                var searchRepositoriesRequest = new SearchRepositoriesRequest
                {
                    SortField = (RepoSearchSort)repoChecker.SearchSort,
                    Order = (SortDirection)repoChecker.SortAscDsc,
                };

                if (repoChecker.MinStar.HasValue && repoChecker.MinStar >= 0)
                {
                    searchRepositoriesRequest.Stars = Octokit.Range.GreaterThanOrEquals(repoChecker.MinStar.Value);
                }
                else
                {
                    searchRepositoriesRequest.Stars = Octokit.Range.GreaterThanOrEquals(0);
                }

                if (repoChecker.UpdatedAfter.HasValue && repoChecker.UpdatedAfter < DateTime.UtcNow)
                {
                    searchRepositoriesRequest.Updated = DateRange.Between(repoChecker.UpdatedAfter.Value.ToUniversalTime(), DateTimeOffset.UtcNow);
                }

                searchRepositoriesRequest.User = repoChecker.User;

                int maxRepos = repoChecker.NumberOfReposToSearchFor ?? 5;

                IEnumerable<Uri> uris = uriFinder.GetUris(maxRepos, searchRepositoriesRequest);
                repoChecker.Uris.AddRange(uris);

            }

            return View("Index", repoChecker);
        }


        public JsonResult CheckRepo(string uri)
        {

            LinkChecker linkChecker = new LinkChecker(new HttpClient(), "readme");
            Dictionary<string, HttpResponseMessage> linkWithResponse = linkChecker.CheckLinksAsync(new Uri(uri)).Result;

            var linkWithStatusCode = new Dictionary<string, HttpStatusCode>();
            foreach (var item in linkWithResponse)
            {
                linkWithStatusCode.Add(item.Key, item.Value.StatusCode);
            }

            //UriWithResults UriWithResults = new UriWithResults() { Uri = uri, LinksWithStatusCode = linkWithStatusCode };

            return Json(linkWithStatusCode);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
