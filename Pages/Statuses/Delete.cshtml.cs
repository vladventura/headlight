using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Statuses
{
    public class DeleteModel(AppDbContext context) : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int StatusId { get; set; }
        public Status? SelectedStatus {  get; set; }
        public IActionResult OnGet()
        {
            if (StatusId < 4)
            {
                return RedirectToPage("/Statuses/Index");
            }
            SelectedStatus = context.Statuses.Where(s => s.Id == StatusId).FirstOrDefault();
            return Page();
        }

        public IActionResult OnPostConfirmDelete()
        {
            SelectedStatus = context.Statuses
                .Where(s => s.Id == StatusId)
                .Include(s => s.Games)
                .FirstOrDefault();
            if (SelectedStatus  != null)
            {
                if (SelectedStatus.Id > 3)
                {
                    SetGamesToDefault();
                    context.Statuses.Remove(SelectedStatus);
                    context.SaveChanges();
                }
            }
            return RedirectToPage("/Statuses/Index");
        }

        public IActionResult OnPostCancelChanges()
        {
            return RedirectToPage("/Statuses/Index");
        }

        private void SetGamesToDefault()
        {
            if (SelectedStatus == null) return;
            foreach(var game in SelectedStatus.Games)
            {
                game.StatusId = 1;
            }
        }
    }
}
