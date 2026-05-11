using Headlight.Data;
using Headlight.Models;
using Headlight.Models.JSON;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Headlight.Pages.Settings
{
    public class ImportDataModel(AppDbContext context) : PageModel
    {
        [BindProperty]
        public IFormFile? UploadedFile { get; set; }
        [BindProperty(SupportsGet = true)]
        public string PageMessage { get; set; } = "";
        [BindProperty(SupportsGet = true)]
        public string PageMessageCssClass { get; set; } = "";
        public void OnGet()
        {
        }

        public IActionResult OnPostHandleUploadData()
        {
            if (UploadedFile != null)
            {
                using var reader = new StreamReader(UploadedFile.OpenReadStream());
                string content = reader.ReadToEnd();
                ImportJSON? jsonData = JsonSerializer.Deserialize<ImportJSON>(content);
                if (jsonData != null)
                {
                    ProcessJsonData(jsonData);
                    PageMessage = string.Format("File uploaded successfully! {0} games processed", jsonData.Games.Count);
                    return Page();
                }
            }
            return RedirectToPage("/Index");
        }

        private void ProcessJsonData(ImportJSON jsonData)
        {
            HandlePlatforms(jsonData.Platforms);
            HandleGames(jsonData.Games);
            context.SaveChanges();
        }

        private void HandlePlatforms(List<string> platforms)
        {
            List<string> existingPlatforms = [.. context.Platforms.Select(p => p.Name)];
            foreach(string platform in platforms)
            {
                if (existingPlatforms.Contains(platform))
                {
                    continue;
                }
                Platform newPlatform = new()
                {
                    Name = platform,
                };
                context.Platforms.Add(newPlatform);
            }
        }

        private void HandleGames(List<GameJSON> games)
        {
            foreach (GameJSON incomingGame in games)
            {
                int platformId = context.Platforms
                    .Where(p => p.Name == incomingGame.Platform)
                    .Select(p => p.Id)
                    .FirstOrDefault();
                platformId = (platformId <= 0) ? 1 : platformId;
                int statusId = context.Statuses
                    .Where(p => p.Name == incomingGame.Status)
                    .Select(p => p.Id)
                    .FirstOrDefault();
                statusId = (statusId <= 0) ? 1 : statusId;
                DateTime? finishedDateTime = statusId == 3 ? DateTime.Now.ToUniversalTime() : null;
                Game? foundGame = context.Games
                    .Where(g => g.Name ==  incomingGame.Name && g.PlatformId == platformId)
                    .FirstOrDefault();
                if (foundGame != null)
                {
                    continue;
                }
                Game newGame = new()
                {
                    Name = incomingGame.Name,
                    StatusId = statusId,
                    PlatformId = platformId,
                    FinishedDateTime = finishedDateTime,
                };
                context.Games.Add(newGame);
            }
        }
    }
}
