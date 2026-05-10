using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Games
{
    public class IndexModel(AppDbContext context) : PageModel
    {
        public List<Game> AllGames { get; set; } = [];
        public void OnGet()
        {
            AllGames = [.. context.Games.Include(o => o.Platform).Include(o => o.Status)];
        }

        public IActionResult OnPostAddGame()
        {
            return RedirectToPage("/Games/Add");
        }
    }
}
