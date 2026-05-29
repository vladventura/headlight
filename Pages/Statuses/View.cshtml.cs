using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Statuses
{
    public class ViewModel(AppDbContext context) : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int StatusId { get; set; }
        [ViewData]
        public string PageTitle { get; set; } = "";

        public Status? SelectedStatus{ get; set; }

        public void OnGet()
        {
            SelectedStatus = context.Statuses
                .Where(s => s.Id == StatusId)
                .Include(s => s.Games)
                .FirstOrDefault();
            PageTitle = SelectedStatus != null ? "View Status" : "Not found";
        }

        public IActionResult OnPostEditStatus()
        {
            return RedirectToPage("/Statuses/Edit", new { StatusId });
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("/Statuses/Index");
        }
    }
}
