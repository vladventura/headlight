using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Games
{
    public class IndexModel(AppDbContext context) : PageModel
    {
        [FromQuery(Name = "PlatformIdFilter")]
        public int PlatformIdFilter { get; set; }
        [FromQuery(Name = "StatusIdFilter")]
        public int StatusIdFilter { get; set; }
        [ViewData]
        public string PageTitle { get; set; } = "Games";
        public List<Game> AllGames { get; set; } = [];
        public void OnGet()
        {
            IQueryable<Game>? query = context.Games.Include(o => o.Platform).Include(o => o.Status).OrderBy(g => g.Name);

            Platform? proposedFilteredPlatform = context.Platforms.Where(p => p.Id == PlatformIdFilter).FirstOrDefault();
            if (proposedFilteredPlatform != null)
            {
                query = query.Where(g => g.PlatformId == proposedFilteredPlatform.Id);
                PageTitle = string.Format("{0} Games", proposedFilteredPlatform.Name);
            }

            Status? proposedFilteredStatus = context.Statuses.Where(p => p.Id ==  StatusIdFilter).FirstOrDefault();
            if (proposedFilteredStatus != null)
            {
                query = query.Where(g => g.StatusId == proposedFilteredStatus.Id);
                PageTitle = string.Format("{0}: {1}", PageTitle, proposedFilteredStatus.Name);
            }

            AllGames = [.. query];
        }

        public IActionResult OnPostAddGame()
        {
            return RedirectToPage("/Games/Add");
        }
    }
}
