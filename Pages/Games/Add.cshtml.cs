using Headlight.AppCode.Globals;
using Headlight.CustomPages;
using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;

namespace Headlight.Pages.Games
{
    public class AddModel(AppDbContext context) : PageTempData
    {
        public List<Platform> Platforms { get; set; } = [];
        public List<Status> Statuses { get; set; } = [];

        private enum CssClass
        {
            Error,
            Success
        }

        public void OnGet()
        {
            SetupPage();
        }

        public IActionResult OnPostSaveChanges(Game game)
        {
            DateTime addedDateTime = DateTime.Now;
            // If the game is finished, set a Finished Date of now
            if (game.StatusId == 3)
            {
                game.FinishedDateTime = addedDateTime;
            }

            game.AddedDateTime ??= addedDateTime;
            game.AddedDateTime = game.AddedDateTime?.Date.ToUniversalTime();

            context.Games.Add(game);
            context.SaveChanges();

            TempData[TempDataVars.MessageResult] = PageMessageResult.Success;
            TempData[TempDataVars.Message] = string.Format("{0} has been added!", game.Name);

            return RedirectToPage("/Games/Add");
        }

        public IActionResult OnPostCancelChanges()
        {
            return RedirectToPage("/Games/Index");
        }

        private void SetupPage()
        {
            Platforms = [.. context.Platforms];
            Statuses = [.. context.Statuses];
        }
    }
}
