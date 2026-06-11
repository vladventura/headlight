using Headlight.AppCode.Globals;
using Headlight.CustomPages;
using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;

namespace Headlight.Pages.Games
{
    public class MultiEditModel(AppDbContext context) : PageTempData
    {
        [BindProperty(SupportsGet = true)]
            public List<string> SelectedGames { get; set; } = [];
        [BindProperty]
            public int StatusId { get; set; }
        [BindProperty]
            public int PlatformId { get; set; }
        [BindProperty]
            public DateTime AddedDateTime { get; set; }

        public List<Status> Statuses { get; set; } = [];
        public List<Platform> Platforms { get; set; } = [];
        private readonly string KeySelectedGames = "SelectedGames";


        public IActionResult OnGet()
        {
            if (SelectedGames.Count == 1)
            {
                return RedirectToPage("/Games/Edit", new { GameId = SelectedGames[0] });
            }
            Statuses = [.. context.Statuses];
            Platforms = [.. context.Platforms];
            TempData[KeySelectedGames] = SelectedGames;
            return Page();
        }

        public IActionResult OnPostSaveChanges()
        {
            TempData[KeySelectedGames] = TempData[KeySelectedGames] ?? new List<string>();
            SelectedGames = [.. (string[])TempData[KeySelectedGames]!];
            foreach(string gameId in SelectedGames)
            {
                Game? selectedGame = context.Games.Where(g => g.Id == int.Parse(gameId)).FirstOrDefault();
                if (selectedGame != null)
                {
                    selectedGame.StatusId = StatusId;
                    if (StatusId == 3)
                    {
                        selectedGame.FinishedDateTime = DateTime.Now.ToUniversalTime().Date;
                    }
                    else
                    {
                        selectedGame.FinishedDateTime = null;
                    }
                    selectedGame.PlatformId = PlatformId;
                    selectedGame.AddedDateTime = AddedDateTime.ToUniversalTime().Date;
                }
            }
            context.SaveChanges();
            TempData[TempDataVars.MessageResult] = PageMessageResult.Success;
            TempData[TempDataVars.Message] = string.Format("Finished editing {0} game(s).", SelectedGames.Count);
            return RedirectToPage("/Games/Index");
        }

        public IActionResult OnPostCancelChanges()
        {
            return RedirectToPage("/Games/Index");
        }
    }
}
