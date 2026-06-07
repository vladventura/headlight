using Headlight.AppCode.Globals;
using Headlight.CustomPages;
using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;

namespace Headlight.Pages.Platforms
{
    public class EditModel(AppDbContext context) : PageTempData
    {
        [BindProperty(SupportsGet = true)]
        public int PlatformId { get; set; }
        public Platform? SelectedPlatform { get; set; }

        public void OnGet()
        {
            GetSelectedPlatform();
        }

        public IActionResult OnPostSaveChanges(Game game)
        {
            GetSelectedPlatform();
            if (SelectedPlatform != null)
            {
                SelectedPlatform.Name = game.Name;
                context.SaveChanges();
                return RedirectToPage("/Platforms/Index");
            }
            else
            {
                TempData[TempDataVars.MessageResult] = PageMessageResult.Error;
                TempData[TempDataVars.Message] = string.Format("Could not find id: {0}", PlatformId);
                return RedirectToPage("/Platforms/Edit", new { PlatformId });
            }
        }

        public IActionResult OnPostCancelChanges()
        {
            return RedirectToPage("/Platforms/Index");
        }

        private void GetSelectedPlatform()
        {
            SelectedPlatform = context.Platforms
            .Where(g => g.Id == PlatformId)
            .FirstOrDefault();
        }
    }
}
