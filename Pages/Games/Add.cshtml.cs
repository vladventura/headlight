using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Headlight.Pages.Games
{
    public class AddModel(AppDbContext context) : PageModel
    {
        public List<Platform> Platforms { get; set; } = [];
        public List<Status> Statuses { get; set; } = [];
        [BindProperty(SupportsGet = true)]
        public string PageMessage { get; set; } = "";
        [BindProperty(SupportsGet = true)]
        public string PageMessageCssClass { get; set; } = "";

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
            string pageMessage = string.Format("{0} has been added!", game.Name);
            SetPageMessage(pageMessage, CssClass.Success);
            SetupPage();
            return Page();
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

        private void SetPageMessage(string message, CssClass css)
        {
            PageMessage = message;
            PageMessageCssClass = "page-message-" + css.ToString().ToLower();
        }
    }
}
