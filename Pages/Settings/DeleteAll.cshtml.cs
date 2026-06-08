using Headlight.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Headlight.Pages.Settings
{
    public class DeleteAllModel(AppDbContext context) : PageModel
    {
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostConfirmDelete()
        {
            context.Games.RemoveRange(context.Games);
            context.Statuses.RemoveRange(context.Statuses.Where(s => s.Id > 3));
            context.Platforms.RemoveRange(context.Platforms.Where(p => p.Id > 1));
            await context.SaveChangesAsync();
            return RedirectToPage("/Index");
        }

        public IActionResult OnPostCancelChanges()
        {
            return RedirectToPage("/Index");
        }
    }
}
