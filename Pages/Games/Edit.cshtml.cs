using Headlight.AppCode.Globals;
using Headlight.CustomPages;
using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Games
{
    public class EditModel(AppDbContext context) : PageTempData
    {
        [BindProperty(SupportsGet = true)]
        public int GameId { get; set; }
        public Game? SelectedGame { get; set; }

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
                SelectedGame.AddedDateTime = game.AddedDateTime?.ToUniversalTime().Date;
                if (SelectedGame.StatusId == 3)
                {
                    SelectedGame.FinishedDateTime = DateTime.Now.ToUniversalTime().Date;
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
                TempData[TempDataVars.MessageResult] = PageMessageResult.Error;
                TempData[TempDataVars.Message] = string.Format("Could not find id: {0}", GameId);
                return RedirectToPage("/Games/Edit", new { GameId });
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
    }
}
