using Headlight.AppCode.Globals;
using Headlight.CustomPages;
using Headlight.Data;
using Headlight.Models;
using Headlight.Models.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
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

    public class IndexModel(AppDbContext context) : PageTempData
    {

        [FromQuery(Name = "PlatformIdFilter")]
        public List<int> PlatformIdFilter { get; set; } = [];
        [FromQuery(Name = "StatusIdFilter")]
        public List<int> StatusIdFilter { get; set; } = [];

        [ViewData]
        public string PageTitle { get; set; } = "Games";

        public string PageMessageCssClass { get; set; } = "";
        public SearchableTableData SearchableTableData { get; set; } = new();

        public List<PlatformFilterOptions> PlatformFilters { get; set; } = [];
        public List<StatusFilterOptions> StatusFilters { get; set; } = [];
        private List<Game> AllGames { get; set; } = [];
        private int GamesPage = 1;
        public void OnGet()
        {
            LoadAllGames();
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
            AssemblePlatformFilters();
            AssembleStatusFilters();
            FillSearchableTableData();
        }

        public IActionResult OnPostAddGame()
        {
            return RedirectToPage("/Games/Add");
        }

        public IActionResult OnPostFilters(List<StatusFilterOptions>? statusOptions, List<PlatformFilterOptions>? platformOptions)
        {
            if (statusOptions != null)
            {
                StatusIdFilter = [.. statusOptions
                    .Where(so => so.StatusIsChecked == "true")
                    .Select(so => so.StatusId)
                ];
            }
            if (platformOptions != null)
            {
                PlatformIdFilter = [.. platformOptions
                    .Where(po => po.PlatformIsChecked == "true")
                    .Select(po => po.PlatformId)
                ];
            }
            return RedirectToPage("/Games/Index", new { PlatformIdFilter, StatusIdFilter });
        }

        public PartialViewResult OnGetRows(int incomingPage)
        {
            GamesPage = incomingPage;
            LoadAllGames();
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
                    PlatformIsChecked = StatusIdFilter.Where(sid => sid == platform.Id).Count() > 0 ? "true" : "false",
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
                    StatusIsChecked = StatusIdFilter.Where(sid => sid == status.Id).Count() > 0 ? "true" : "false",
                };
                StatusFilters.Add(option);
            }
        }

        private void LoadAllGames()
        {
            IQueryable<Game>? query = context.Games.Include(o => o.Platform).Include(o => o.Status).OrderBy(g => g.Name);
            IQueryable<Platform>? proposedFilteredPlatform = context.Platforms.Where(p => PlatformIdFilter.Contains(p.Id));
            if (proposedFilteredPlatform.Count() <= 0)
            {
                proposedFilteredPlatform = null;
            }
            if (proposedFilteredPlatform != null)
            {
                query = query.Where(g => proposedFilteredPlatform
                    .Select(p => p.Id)
                    .ToList()
                    .Contains(g.PlatformId));
                PageTitle = string.Format("{0} - {1}",
                    proposedFilteredPlatform.Count() > 1 ? string.Format("{0} Platforms", proposedFilteredPlatform.Count()) :
                    proposedFilteredPlatform.First().Name,
                    PageTitle
                );
            }

            IQueryable<Status>? proposedFilteredStatus = context.Statuses.Where(s => StatusIdFilter.Contains(s.Id));
            if (proposedFilteredStatus.Count() <= 0)
            {
                proposedFilteredStatus = null;
            }
            if (proposedFilteredStatus != null)
            {
                query = query.Where(g => proposedFilteredStatus
                    .Select(s => s.Id)
                    .ToList()
                    .Contains(g.StatusId)
                );
                PageTitle = string.Format("{0}: {1}", 
                    PageTitle,
                    proposedFilteredStatus.Count() > 1 ? string.Format("{0} Statuses", proposedFilteredStatus.Count()) :
                    proposedFilteredStatus.First().Name
                );
            }

            AllGames = [.. query.Skip((GamesPage - 1) * 50).Take(50)];
        }

        private void FillSearchableTableData()
        {
            this.SearchableTableData.Paginate = true;
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
