using Headlight.AppCode.Globals;
using Headlight.CustomPages;
using Headlight.Data;
using Headlight.Models;
using Headlight.Models.JSON;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Headlight.Pages.Settings
{
    public class ExportDataModel(AppDbContext context) : PageTempData
    {
        public void OnGet()
        {
        }

        public IActionResult OnGetDownloadData()
        {
            List<GameJSON> games = HandleGames();
            return File(
                JsonSerializer.SerializeToUtf8Bytes(games),
                "application/json",
                "headlight-data.json"
            );
        }

        private List<GameJSON> HandleGames()
        {
            List<GameJSON> result = [];
            foreach(Game game in context.Games.Include(g => g.Status).Include(g => g.Platform))
            {
                GameJSON json = new()
                {
                    Id = game.Id,
                    Name = game.Name,
                    Platform = game.Platform.Name,
                    Status = game.Status.Name
                };
                result.Add(json);
            }
            return result;
        }
    }
}
