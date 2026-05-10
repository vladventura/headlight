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
        [ViewData]
        public string PageTitle { get; set; } = "Games";
        public List<Game> AllGames { get; set; } = [];
        public void OnGet()
        {
            Platform? proposedFilteredPlatform = context.Platforms.Where(p => p.Id == PlatformIdFilter).FirstOrDefault();

            var query = context.Games.Include(o => o.Platform).Include(o => o.Status).OrderBy(g => g.Name);

            if (proposedFilteredPlatform != null)
            {
                AllGames = [.. query.Where(g => g.PlatformId == proposedFilteredPlatform.Id)];
                PageTitle = string.Format("{0} Games", proposedFilteredPlatform.Name);
            }
            else
            {
                AllGames = [.. query];
            }

        }

        public IActionResult OnPostAddGame()
        {
            return RedirectToPage("/Games/Add");
        }
    }
}
