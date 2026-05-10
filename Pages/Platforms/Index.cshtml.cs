using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Platforms
{
    public class IndexModel(AppDbContext context) : PageModel
    {
        public List<Platform> AllPlatforms { get; set; } = [];
        public void OnGet()
        {
            AllPlatforms = [.. context.Platforms.Include(o => o.Games).OrderBy(g => g.Name)];
        }

        public IActionResult OnPostAddPlatform()
        {
            return RedirectToPage("/Platforms/Add");
        }
    }
}
