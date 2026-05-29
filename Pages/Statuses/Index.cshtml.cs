using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Statuses
{
    public class IndexModel(AppDbContext context) : PageModel
    {
        public List<Status> AllStatuses { get; set; } = [];
        public void OnGet()
        {
            AllStatuses = [.. context.Statuses.Include(o => o.Games).OrderBy(g => g.Name)];
        }

        public IActionResult OnPostAddStatus()
        {
            return RedirectToPage("/Statuses/Add");
        }
    }
}
