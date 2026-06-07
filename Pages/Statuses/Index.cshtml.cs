using Headlight.CustomPages;
using Headlight.Data;
using Headlight.Models.Components;
using Headlight.Strategies.SearchableTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Headlight.Pages.Statuses
{
    public class IndexModel(AppDbContext context) : PageTempData, ISearchablePage
    {
        [BindProperty(Name = "SearchInput", SupportsGet = true)]
            public string SearchInput { get; set; } = "";
        public ISearchStrategy? Strategy { get; set; }
        public SearchableTableData SearchableTableData { get; set; } = new();
        [FromQuery(Name = "SortField")]
            public string SortField { get; set; } = "";
        [FromQuery(Name = "SortDirection")]
            public string SortDirection { get; set; } = "";
        private int PaginationPage { get; set; } = 1;

        public void OnGet()
        {
            FillSearchableTableData();
        }

        public PartialViewResult OnGetRows(int incomingPage)
        {
            PaginationPage = incomingPage;
            FillSearchableTableData();
            return new()
            {
                ViewName = "_SearchableTableRowsPartial",
                ViewData = new ViewDataDictionary<SearchableTableData>(ViewData, SearchableTableData)
            };
        }

        public IActionResult OnPostAddStatus()
        {
            return RedirectToPage("/Statuses/Add");
        }

        public IActionResult OnPostQuery()
        {
            return RedirectToPage("/Statuses/Index", new { SearchInput });
        }

        private void FillSearchableTableData()
        {
            Strategy = new StatusSearchStrategy(context, Url, SearchInput, PaginationPage, SortField, SortDirection);
            SearchableTableData = Strategy.GetTableData();
        }
    }
}
