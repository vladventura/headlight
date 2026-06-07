using Headlight.CustomPages;
using Headlight.Data;
using Headlight.Models;
using Headlight.Models.Components;
using Headlight.Strategies.SearchableTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations.Schema;

namespace Headlight.Pages.Games
{
    public class StatusFilterOptions
    {
        public int StatusId { get; set; }
        public string StatusIsChecked { get; set; } = "";
        [NotMapped]
        public string StatusName { get; set; } = "";
    }

    public class PlatformFilterOptions
    {
        public int PlatformId { get; set; }
        public string PlatformIsChecked { get; set; } = "";
        [NotMapped]
        public string PlatformName { get; set; } = "";
    }

    public class IndexModel(AppDbContext context) : PageTempData, ISearchablePage
    {
        [BindProperty(Name = "SearchInput", SupportsGet = true)]
            public string SearchInput { get; set; } = "";
        public ISearchStrategy? Strategy { get; set; }
        public SearchableTableData SearchableTableData { get; set; } = new();

        [BindProperty]
            public List<PlatformFilterOptions> PlatformFilters { get; set; } = [];
        [BindProperty]
            public List<StatusFilterOptions> StatusFilters { get; set; } = [];
        [FromQuery(Name = "PlatformIdFilter")]
            public List<int> PlatformIdFilter { get; set; } = [];
        [FromQuery(Name = "StatusIdFilter")]
            public List<int> StatusIdFilter { get; set; } = [];
        [FromQuery(Name = "SortField")]
            public string SortField { get; set; } = "";
        [FromQuery(Name = "SortDirection")]
            public string SortDirection { get; set; } = "";

        [ViewData]
            public string PageTitle { get; set; } = "Games";

        private int GamesPage = 1;
        public void OnGet()
        {
            AssemblePlatformFilters();
            AssembleStatusFilters();
            FillSearchableTableData();
        }

        public IActionResult OnPostAddGame()
        {
            return RedirectToPage("/Games/Add");
        }

        public IActionResult OnPostQuery()
        {
            if (StatusFilters.Count > 0)
            {
                StatusIdFilter = [.. StatusFilters
                    .Where(so => so.StatusIsChecked == "true")
                    .Select(so => so.StatusId)
                ];
            }
            if (PlatformFilters.Count > 0)
            {
                PlatformIdFilter = [.. PlatformFilters
                    .Where(po => po.PlatformIsChecked == "true")
                    .Select(po => po.PlatformId)
                ];
            }
            return RedirectToPage("/Games/Index", new { PlatformIdFilter, StatusIdFilter, SearchInput });
        }

        public PartialViewResult OnGetRows(int incomingPage)
        {
            GamesPage = incomingPage;
            FillSearchableTableData();
            return new()
            {
                ViewName = "_SearchableTableRowsPartial",
                ViewData = new ViewDataDictionary<SearchableTableData>(ViewData, SearchableTableData),
            };
        }

        private void AssemblePlatformFilters()
        {
            foreach (Platform platform in context.Platforms.ToList())
            {
                PlatformFilterOptions option = new()
                {
                    PlatformName = platform.Name,
                    PlatformId = platform.Id,
                    PlatformIsChecked = PlatformIdFilter.Where(sid => sid == platform.Id).Any() ? "true" : "false",
                };
                PlatformFilters.Add(option);
            }
        }


        private void AssembleStatusFilters()
        {
            foreach(Status status in context.Statuses.ToList())
            {
                StatusFilterOptions option = new() 
                {
                    StatusName = status.Name,
                    StatusId = status.Id,
                    StatusIsChecked = StatusIdFilter.Where(sid => sid == status.Id).Any()? "true" : "false",
                };
                StatusFilters.Add(option);
            }
        }

        private void FillSearchableTableData()
        {
            Strategy = new GameSearchStrategy(
                Url, context, SearchInput,
                SortField, SortDirection, GamesPage,
                PageTitle, StatusIdFilter, PlatformIdFilter
            );
            SearchableTableData = Strategy.GetTableData();
            PageTitle = SearchableTableData.PageTitle ?? PageTitle;
        }
    }
}
