using Headlight.AppCode.Globals;
using Headlight.Data;
using Headlight.Models;
using Headlight.Models.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Platforms
{
    public class IndexModel(AppDbContext context) : PageModel
    {
        public List<Platform> AllPlatforms { get; set; } = [];
        public SearchableTableData SearchableTableData { get; set; } = new();
        public void OnGet()
        {
            AllPlatforms = [.. context.Platforms.Include(o => o.Games).OrderBy(g => g.Name)];
            FillSearchableTableData();
        }

        public IActionResult OnPostAddPlatform()
        {
            return RedirectToPage("/Platforms/Add");
        }

        private void FillSearchableTableData()
        {
            var nameCol = SearchableTableData.AddColumn("Name");
            var countCol = SearchableTableData.AddColumn("Game Count");

            foreach (Platform platform in AllPlatforms)
            {
                var row = SearchableTableData.AddRow();
                row.HtmlAttributes = string.Format("id=\"{0}\"", platform.Id);
                
                var nameCell = row.AddCell(nameCol.Index, platform.Name);
                string nameHref = Url.Page("/Platforms/View", new { PlatformId = platform.Id }) ?? "";
                nameCell.Clickable = true;
                nameCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", nameHref);
                
                var countCell = row.AddCell(countCol.Index, platform.Games?.Count.ToString() ?? "0");
                string countHref = Url.Page("/Games/Index", new { PlatformIdFilter = platform.Id }) ?? "";
                countCell.Clickable = true;
                countCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", countHref);

                var deleteCell = row.AddCell(-1, "");
                string deleteHref = Url.Page("/Platforms/Delete", new { PlatformId = platform.Id }) ?? "";
                deleteCell.Clickable = true;
                deleteCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", deleteHref);
                deleteCell.Icon = SvgIcon.Delete;
                deleteCell.ShouldRender = platform.Id > 1;
            }
        }
    }
}
