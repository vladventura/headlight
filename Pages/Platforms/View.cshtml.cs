using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Platforms
{
    public class ViewModel(AppDbContext context) : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int PlatformId { get; set; }
        [ViewData]
        public string PageTitle { get; set; } = "";

        public Platform? SelectedPlatform{ get; set; }

        public void OnGet()
        {
            SelectedPlatform = context.Platforms
                .Where(g => g.Id == PlatformId)
                .Include(p => p.Games)
                .FirstOrDefault();
            PageTitle = SelectedPlatform != null ? "View Platform" : "Not found";
        }

        public IActionResult OnPostEditPlatform()
        {
            return RedirectToPage("/Platforms/Edit", new { PlatformId });
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("/Platforms/Index");
        }
    }
}
