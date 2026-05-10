using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Games
{
    public class EditModel(AppDbContext context) : PageModel
    {
        private enum CssClass
        {
            Error,
            Success
        }

        [BindProperty(SupportsGet = true)]
        public int GameId { get; set; }
        public Game? SelectedGame { get; set; }
        [BindProperty(SupportsGet = true)]
        public string PageMessage { get; set; } = "";
        public string PageMessageCssClass { get; set; } = "";

        public List<Status> Statuses { get; set; } = [];
        public List<Platform> Platforms { get; set; } = [];

        public void OnGet()
        {
            GetSelectedGame();
            Statuses = [.. context.Statuses];
            Platforms = [.. context.Platforms];
        }

        public IActionResult OnPostSaveChanges(Game game)
        {
            GetSelectedGame();
            if (SelectedGame != null)
            {
                SelectedGame.PlatformId = game.PlatformId;
                SelectedGame.StatusId = game.StatusId;
                if (SelectedGame.StatusId == 3)
                {
                    SelectedGame.FinishedDateTime = DateTime.Now.ToUniversalTime();
                }
                else
                {
                    SelectedGame.FinishedDateTime = null;
                }
                SelectedGame.Name = game.Name;
                context.SaveChanges();
                return RedirectToPage("/Games/Index");
            }
            else
            {
                SetPageMessage(string.Format("Could not find id: {0}", GameId), CssClass.Error);
                return Page();
            }
        }

        public IActionResult OnPostCancelChanges()
        {
            return RedirectToPage("/Games/Index");
        }

        private void GetSelectedGame()
        {
            SelectedGame = context.Games
            .Where(g => g.Id == GameId)
            .Include(o => o.Status)
            .Include(o => o.Platform)
            .FirstOrDefault();
        }

        private void SetPageMessage(string message, CssClass css)
        {
            PageMessage = message;
            PageMessageCssClass = "page-message-" + css.ToString().ToLower();
        }
    }
}
