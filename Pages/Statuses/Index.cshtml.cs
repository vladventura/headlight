using Headlight.AppCode.Globals;
using Headlight.Data;
using Headlight.Models;
using Headlight.Models.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Statuses
{
    public class IndexModel(AppDbContext context) : PageModel
    {
        public List<Status> AllStatuses { get; set; } = [];
        public SearchableTableData SearchableTableData { get; set; } = new();
        public void OnGet()
        {
            AllStatuses = [.. context.Statuses.Include(o => o.Games).OrderBy(g => g.Name)];
            FillSearchableTableData();
        }

        public IActionResult OnPostAddStatus()
        {
            return RedirectToPage("/Statuses/Add");
        }

        private void FillSearchableTableData()
        {
            var nameCol = SearchableTableData.AddColumn("Name");
            var countCol = SearchableTableData.AddColumn("Game Count");

            foreach (Status status in AllStatuses)
            {
                var row = SearchableTableData.AddRow();
                row.HtmlAttributes = string.Format("id=\"{0}\"", status.Id);

                var nameCell = row.AddCell(nameCol.Index, status.Name);
                string nameHref = Url.Page("/Statuses/View", new { StatusId = status.Id }) ?? "";
                nameCell.Clickable = true;
                nameCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", nameHref);

                var countCell = row.AddCell(countCol.Index, status.Games?.Count.ToString() ?? "0");
                string countHref = Url.Page("/Games/Index", new { StatusIdFilter = status.Id }) ?? "";
                countCell.Clickable = true;
                countCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", countHref);

                var deleteCell = row.AddCell(-1, "");
                string deleteHref = Url.Page("/Statuses/Delete", new { StatusId = status.Id }) ?? "";
                deleteCell.Clickable = true;
                deleteCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", deleteHref);
                deleteCell.Icon = SvgIcon.Delete;
                deleteCell.ShouldRender = status.Id > 3;
            }
        }
    }
}
