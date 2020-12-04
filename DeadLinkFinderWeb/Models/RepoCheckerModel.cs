using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DeadLinkFinderWeb.Models
{
    public class RepoCheckerModel
    {
        [Range(0, 25, ErrorMessage = "Enter number between 0 and 25")]
        [Display(Name = "Max Repos to check")]
        public int? NumberOfReposToSearchFor { get; set; }

        [Url]
        [Display(Name = "Single Repo check URI")]
        public string SingleRepoUri { get; set; }

        [Display(Name = "Minimum Repo Star rating")]
        public int? MinStar { get; set; }

        [Display(Name = "Repos update after")]
        public DateTime? UpdatedAfter { get; set; }

        [Display(Name = "Repos for GitHub User/Account")]
        public string User { get; set; }

        [Display(Name = "Sort Field")]
        public RepoSearchSort? SearchSort { get; set; }

        [Display(Name = "Sort Asc/Dsc")]
        public SortDirection? SortAscDsc { get; set; }

        public bool IncludeForks { get; set; } = false;

        public List<Uri> Uris;

        public RepoCheckerModel()
        {
            Uris = new List<Uri>();
        }

        public enum RepoSearchSort
        {
            Stars = 0,
            Forks = 1,
            Updated = 2
        }

        public enum SortDirection
        {
            Ascending = 0,
            Descending = 1
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine($"NumberOfReposToSearchFor : {NumberOfReposToSearchFor}");
            output.AppendLine($"SingleRepoUri : {SingleRepoUri}");
            output.AppendLine($"MinStar : {MinStar}");
            output.AppendLine($"UpdatedAfter : {UpdatedAfter}");
            output.AppendLine($"User : {User}");
            output.AppendLine($"SearchSort : {SearchSort}");
            output.AppendLine($"SortAscDsc : {SortAscDsc}");
            output.AppendLine($"IncludeForks : {IncludeForks}");

            return output.ToString();
        }
    }
}
