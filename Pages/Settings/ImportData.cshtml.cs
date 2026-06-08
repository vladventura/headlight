using Headlight.AppCode.Globals;
using Headlight.CustomPages;
using Headlight.Data;
using Headlight.Models;
using Headlight.Models.JSON;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Headlight.Pages.Settings
{
    public class ImportDataModel(AppDbContext context) : PageTempData
    {
        [BindProperty]
        public IFormFile? UploadedFile { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPostHandleUploadData()
        {
            if (UploadedFile != null)
            {
                using var reader = new StreamReader(UploadedFile.OpenReadStream());
                string content = reader.ReadToEnd();
                
                ImportJSON? jsonData;
                try
                {
                    jsonData = JsonSerializer.Deserialize<ImportJSON>(content);
                    if (jsonData != null)
                    {
                        ProcessJsonData(jsonData);
                        TempData[TempDataVars.MessageResult] = PageMessageResult.Success;
                        TempData[TempDataVars.Message] = string.Format("File uploaded successfully! {0} games processed", jsonData.Games.Count);
                    } 
                    else
                    {
                        TempData[TempDataVars.MessageResult] = PageMessageResult.Error;
                        TempData[TempDataVars.Message] = string.Format("Could not process file!");
                    }
                } 
                catch
                {
                    TempData[TempDataVars.MessageResult] = PageMessageResult.Error;
                    TempData[TempDataVars.Message] = string.Format("Invalid format!");
                }

            }
            else
            {
                TempData[TempDataVars.MessageResult] = PageMessageResult.Error;
                TempData[TempDataVars.Message] = string.Format("No file uploaded~!");
            }
            return RedirectToPage("/Settings/ImportData");
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
                DateTime addedDateTime = statusId == 3 ? DateTime.Now.AddMonths(-1) : DateTime.Now;
                addedDateTime = addedDateTime.ToUniversalTime().Date;
                DateTime? finishedDateTime = statusId == 3 ? DateTime.Now.ToUniversalTime().Date : null;
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
                    AddedDateTime = addedDateTime
                };
                context.Games.Add(newGame);
            }
        }
    }
}
