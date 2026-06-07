using Headlight.Models.Components;
using Headlight.Strategies.SearchableTable;
using Microsoft.AspNetCore.Mvc;

namespace Headlight.CustomPages
{
    public interface ISearchablePage
    {
        [BindProperty(Name = "SearchInput", SupportsGet = true)]
        public string SearchInput { get; set; }
        [FromQuery(Name = "SortField")]
        public string SortField { get; set; }
        [FromQuery(Name = "SortDirection")]
        public string SortDirection { get; set; }
        public ISearchStrategy? Strategy { get; set; }
        public SearchableTableData SearchableTableData { get; set; }

        public IActionResult OnPostQuery();
        public PartialViewResult OnGetRows(int incomingPage);
    }
}
