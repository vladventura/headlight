using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Games
{
    public class ViewModel(AppDbContext context) : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int GameId { get; set; }
        [ViewData]
        public string PageTitle { get; set; } = "";

        public Game? SelectedGame { get; set; }

        public void OnGet()
        {
            SelectedGame = context.Games.Where(g => g.Id == GameId)
                .Include(o => o.Platform)
                .Include(o => o.Status).FirstOrDefault();
            PageTitle = SelectedGame != null ? "View Game" : "Not found.";
        }

        public IActionResult OnPostEditGame()
        {
            return RedirectToPage("/Games/Edit", new { GameId });
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("/Games/Index");
        }
    }
}
