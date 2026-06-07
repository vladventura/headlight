using Headlight.AppCode.Globals;
using Headlight.Data;
using Headlight.Models;
using Headlight.Models.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Strategies.SearchableTable
{
    public class StatusSearchStrategy(
        AppDbContext context, IUrlHelper url,
        string searchInput, int page,
        string sortField, string sortDirection
    ) : ISearchStrategy
    {
        public SearchableTableData GetTableData()
        {
            IQueryable<Status> allStatuses = GetStatuses();

            SearchableTableData tableData = new()
            {
                Paginate = true,
            };
            var nameCol = tableData.AddColumn("Name");
            nameCol.IsSortField = true;
            var countCol = tableData.AddColumn("Game Count", id: "GameCount");
            countCol.IsSortField = true;

            foreach (Status status in allStatuses)
            {
                var row = tableData.AddRow();
                row.HtmlAttributes = string.Format("id=\"{0}\"", status.Id);

                var nameCell = row.AddCell(nameCol.Index, status.Name);
                string nameHref = url.Page("/Statuses/View", new { StatusId = status.Id }) ?? "";
                nameCell.Clickable = true;
                nameCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", nameHref);

                var countCell = row.AddCell(countCol.Index, status.Games?.Count.ToString() ?? "0");
                string countHref = url.Page("/Games/Index", new { StatusIdFilter = status.Id }) ?? "";
                countCell.Clickable = true;
                countCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", countHref);

                var deleteCell = row.AddCell(-1, "");
                string deleteHref = url.Page("/Statuses/Delete", new { StatusId = status.Id }) ?? "";
                deleteCell.Clickable = true;
                deleteCell.ClickableHtmlAttributes = string.Format("onclick=\"location.href = '{0}'\"", deleteHref);
                deleteCell.Icon = SvgIcon.Delete;
                deleteCell.ShouldRender = status.Id > 3;
            }
            return tableData;
        }

        private IQueryable<Status> GetStatuses()
        {
            IQueryable<Status> query = context.Statuses.Include(o => o.Games).OrderBy(g => g.Name);

            if (!string.IsNullOrEmpty(searchInput))
            {
                string toLike = string.Format("%{0}%", searchInput.ToLower());
                query = query.Where(s => EF.Functions.ILike(s.Name, toLike));
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

        private IQueryable<Status> NameSortField(IQueryable<Status> query)
        {
            return sortDirection switch
            {
                "Desc" => query.OrderByDescending(s => s.Name),
                _ => query.OrderBy(s => s.Name),
            };
        }

        private IQueryable<Status> GameCountSortField(IQueryable<Status> query)
        {
            return sortDirection switch
            {
                "Desc" => query.OrderByDescending(s => s.Games.Count()),
                _ => query.OrderBy(s => s.Games.Count()),
            };
        }
    }
}