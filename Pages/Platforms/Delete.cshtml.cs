using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Platforms
{
    public class DeleteModel(AppDbContext context) : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int PlatformId { get; set; }
        public Platform? SelectedPlatform {  get; set; }
        public IActionResult OnGet()
        {
            if (PlatformId == 1)
            {
                return RedirectToPage("/Platforms/Index");
            }
            SelectedPlatform = context.Platforms.Where(g => g.Id == PlatformId).FirstOrDefault();
            return Page();
        }

        public IActionResult OnPostConfirmDelete()
        {
            SelectedPlatform = context.Platforms
                .Where(g => g.Id == PlatformId)
                .Include(p => p.Games)
                .FirstOrDefault();
            if (SelectedPlatform  != null)
            {
                if (SelectedPlatform.Id != 1)
                {
                    SetGamesToDefault();
                    context.Platforms.Remove(SelectedPlatform);
                    context.SaveChanges();
                }
            }
            return RedirectToPage("/Platforms/Index");
        }

        public IActionResult OnPostCancelChanges()
        {
            return RedirectToPage("/Platforms/Index");
        }

        private void SetGamesToDefault()
        {
            if (SelectedPlatform == null) return;
            foreach(var game in SelectedPlatform.Games)
            {
                game.PlatformId = 1;
            }
        }
    }
}
