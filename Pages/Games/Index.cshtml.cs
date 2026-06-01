using Headlight.AppCode.Globals;
using Headlight.CustomPages;
using Headlight.Data;
using Headlight.Models;
using Headlight.Models.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Games
{
    public class IndexModel(AppDbContext context) : PageTempData
    {
        public string PageMessageCssClass { get; set; } = "";
        [FromQuery(Name = "PlatformIdFilter")]
        public int PlatformIdFilter { get; set; }
        [FromQuery(Name = "StatusIdFilter")]
        public int StatusIdFilter { get; set; }
        [ViewData]
        public string PageTitle { get; set; } = "Games";
        public List<Game> AllGames { get; set; } = [];
        public SearchableTableData SearchableTableData { get; set; } = new();
        public void OnGet()
        {
            if (MessageResult != null)
            {
                switch ((PageMessageResult)MessageResult)
                {
                    case PageMessageResult.Success:
                        Message ??= "Deleted successfully";
                        PageMessageCssClass = "page-message-success";
                        break;
                    default:
                        break;
                }
            }
            LoadAllGames();
            FillSearchableTableData();
        }

        public IActionResult OnPostAddGame()
        {
            return RedirectToPage("/Games/Add");
        }

        private void LoadAllGames()
        {
            IQueryable<Game>? query = context.Games.Include(o => o.Platform).Include(o => o.Status).OrderBy(g => g.Name);

            Platform? proposedFilteredPlatform = context.Platforms.Where(p => p.Id == PlatformIdFilter).FirstOrDefault();
            if (proposedFilteredPlatform != null)
            {
                query = query.Where(g => g.PlatformId == proposedFilteredPlatform.Id);
                PageTitle = string.Format("{0} Games", proposedFilteredPlatform.Name);
            }

            Status? proposedFilteredStatus = context.Statuses.Where(p => p.Id == StatusIdFilter).FirstOrDefault();
            if (proposedFilteredStatus != null)
            {
                query = query.Where(g => g.StatusId == proposedFilteredStatus.Id);
                PageTitle = string.Format("{0}: {1}", PageTitle, proposedFilteredStatus.Name);
            }

            AllGames = [.. query];
        }

        private void FillSearchableTableData()
        {
            var nameCol = SearchableTableData.AddColumn("Name");
            var statusCol = SearchableTableData.AddColumn("Status");
            var platformCol = SearchableTableData.AddColumn("Platform");

            foreach (Game game in AllGames)
            {
                var row = SearchableTableData.AddRow();
                row.HtmlAttributes = string.Format("id=\"{0}\"", game.Id);
                var nameCell = row.AddCell(nameCol.Index, game.Name);
                nameCell.Clickable = true;
                string nameHref = Url.Page("/Games/View", new { GameId = game.Id }) ?? "";
                nameCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", nameHref);
                row.AddCell(statusCol.Index, game.Status.Name);
                row.AddCell(platformCol.Index, game.Platform.Name);
                var deleteCell = row.AddCell(-1, "");
                string deleteHref = Url.Page("/Games/Delete", new { GameId = game.Id }) ?? "";
                deleteCell.Clickable = true;
                deleteCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", deleteHref);
                deleteCell.Icon = SvgIcon.Delete;
            }
        }
    }
}
