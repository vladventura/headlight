using Headlight.AppCode.Globals;
using Headlight.Data;
using Headlight.Models;
using Headlight.Models.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Strategies.SearchableTable
{
    public class GameSearchStrategy(
            IUrlHelper url, AppDbContext context, 
            string searchInput, string sortField,
            string sortDirection, int gamesPage,
            string pageTitle,
            List<int> statusIdFilter, List<int> platformIdFilter,
            bool? multiSelect
    ) : ISearchStrategy
    {
        private IQueryable<Game>? AllGames { get; set; }

        public SearchableTableData GetTableData()
        {
            LoadAllGames();

            SearchableTableData tableData = new() { 
                Paginate = true,
                PageTitle = pageTitle,
                CheckableRows = multiSelect
            };

            var nameCol = tableData.AddColumn("Name");
            nameCol.IsSortField = true;
            var statusCol = tableData.AddColumn("Status");
            statusCol.IsSortField = true;
            var platformCol = tableData.AddColumn("Platform");
            platformCol.IsSortField = true;

            if (AllGames != null)
            {
                foreach (Game game in AllGames)
                {
                    var row = tableData.AddRow();
                    row.CheckableValue = multiSelect == true ? game.Id.ToString() : null;
                    row.HtmlAttributes = string.Format("id=\"{0}\"", game.Id);
                    var nameCell = row.AddCell(nameCol.Index, game.Name);
                    nameCell.Clickable = true;
                    string nameHref = url.Page("/Games/View", new { GameId = game.Id }) ?? "";
                    nameCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", nameHref);
                    row.AddCell(statusCol.Index, game.Status.Name);
                    row.AddCell(platformCol.Index, game.Platform.Name);
                    var deleteCell = row.AddCell(-1, "");
                    string deleteHref = url.Page("/Games/Delete", new { GameId = game.Id }) ?? "";
                    deleteCell.Clickable = true;
                    deleteCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", deleteHref);
                    deleteCell.Icon = SvgIcon.Delete;
                }
            }

            return tableData;
        }

        private void LoadAllGames()
        {
            IQueryable<Game>? query = context.Games.Include(o => o.Platform).Include(o => o.Status);

            if (!string.IsNullOrEmpty(searchInput))
            {
                string toLike = string.Format("%{0}%", searchInput.ToLower());
                query = query?.Where(g => EF.Functions.ILike(g.Name, toLike));
            }

            query = sortField switch
            {
                "Name" => NameSortField(query),
                "Status" => StatusSortField(query),
                "Platform" => PlatformSortField(query),
                _ => query?.OrderBy(g => g.Name),
            };

            IQueryable<Platform>? proposedFilteredPlatform = context.Platforms.Where(p => platformIdFilter.Contains(p.Id));
            if (!proposedFilteredPlatform.Any())
            {
                proposedFilteredPlatform = null;
            }
            if (proposedFilteredPlatform != null)
            {
                query = query?.Where(g => proposedFilteredPlatform
                    .Select(p => p.Id)
                    .ToList()
                    .Contains(g.PlatformId));
                pageTitle = string.Format("{0} - {1}",
                    proposedFilteredPlatform.Count() > 1 ? string.Format("{0} Platforms", proposedFilteredPlatform.Count()) :
                    proposedFilteredPlatform.First().Name,
                    pageTitle
                );
            }

            IQueryable<Status>? proposedFilteredStatus = context.Statuses.Where(s => statusIdFilter.Contains(s.Id));
            if (!proposedFilteredStatus.Any())
            {
                proposedFilteredStatus = null;
            }
            if (proposedFilteredStatus != null)
            {
                query = query?.Where(g => proposedFilteredStatus
                    .Select(s => s.Id)
                    .ToList()
                    .Contains(g.StatusId)
                );
                pageTitle = string.Format("{0}: {1}",
                    pageTitle,
                    proposedFilteredStatus.Count() > 1 ? string.Format("{0} Statuses", proposedFilteredStatus.Count()) :
                    proposedFilteredStatus.First().Name
                );
            }

            AllGames = query?.Skip((gamesPage - 1) * 50).Take(50);
        }

        private IQueryable<Game>? NameSortField(IQueryable<Game>? query)
        {
            return sortDirection switch
            {
                "Desc" => query?.OrderByDescending(g => g.Name),
                _ => query?.OrderBy(g => g.Name),
            };
        }

        private IQueryable<Game>? StatusSortField(IQueryable<Game>? query)
        {
            return sortDirection switch
            {
                "Desc" => query?.OrderByDescending(g => g.StatusId),
                _ => query?.OrderBy(g => g.StatusId),
            };
        }

        private IQueryable<Game>? PlatformSortField(IQueryable<Game>? query)
        {
            return sortDirection switch
            {
                "Desc" => query?.OrderByDescending(g => g.PlatformId),
                _ => query?.OrderBy(g => g.PlatformId),
            };
        }
    }
}
