using Headlight.AppCode.Globals;
using Headlight.Data;
using Headlight.Models;
using Headlight.Models.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Strategies.SearchableTable
{
    public class PlatformSearchStrategy(
        AppDbContext context, IUrlHelper url, 
        string searchInput, int page,
        string sortField, string sortDirection
    ) : ISearchStrategy
    {
        public SearchableTableData GetTableData()
        {
            IQueryable<Platform> allPlatforms = GetPlatforms();

            SearchableTableData tableData = new()
            {
                Paginate = true,
            };
            var nameCol = tableData.AddColumn("Name");
            nameCol.IsSortField = true;
            var countCol = tableData.AddColumn("Game Count", id: "GameCount");
            countCol.IsSortField = true;

            foreach (Platform platform in allPlatforms)
            {
                var row = tableData.AddRow();
                row.HtmlAttributes = string.Format("id=\"{0}\"", platform.Id);

                var nameCell = row.AddCell(nameCol.Index, platform.Name);
                string nameHref = url.Page("/Platforms/View", new { PlatformId = platform.Id }) ?? "";
                nameCell.Clickable = true;
                nameCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", nameHref);

                var countCell = row.AddCell(countCol.Index, platform.Games?.Count.ToString() ?? "0");
                string countHref = url.Page("/Games/Index", new { PlatformIdFilter = platform.Id }) ?? "";
                countCell.Clickable = true;
                countCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", countHref);

                var deleteCell = row.AddCell(-1, "");
                string deleteHref = url.Page("/Platforms/Delete", new { PlatformId = platform.Id }) ?? "";
                deleteCell.Clickable = true;
                deleteCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", deleteHref);
                deleteCell.Icon = SvgIcon.Delete;
                deleteCell.ShouldRender = platform.Id > 1;
            }
            return tableData;
        }

        private IQueryable<Platform> GetPlatforms()
        {
            IQueryable<Platform> query = context.Platforms.Include(o => o.Games).OrderBy(g => g.Name);

            if (!string.IsNullOrEmpty(searchInput))
            {
                string toLike = string.Format("%{0}%", searchInput.ToLower());
                query = query.Where(p => EF.Functions.ILike(p.Name, toLike));
            }

            query = sortField switch
            {
                "Name" => NameSortField(query),
                "GameCount" => GameCountSortField(query),
                _ => query
            };


            query = query.Skip((page - 1) * 50).Take(50);

            return query;
        }

        private IQueryable<Platform> NameSortField(IQueryable<Platform> query)
        {
            return sortDirection switch
            {
                "Desc" => query.OrderByDescending(g => g.Name),
                _ => query.OrderBy(g => g.Name),
            };
        }

        private IQueryable<Platform> GameCountSortField(IQueryable<Platform> query)
        {
            return sortDirection switch
            {
                "Desc" => query.OrderByDescending(p => p.Games.Count()),
                _ => query.OrderBy(p => p.Games.Count()),
            };
        }
    }
}
