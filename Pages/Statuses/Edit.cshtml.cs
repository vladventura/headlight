using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Statuses
{
    public class EditModel(AppDbContext context) : PageModel
    {
        private enum CssClass
        {
            Error,
            Success
        }

        [BindProperty(SupportsGet = true)]
        public int StatusId { get; set; }
        public Status? SelectedStatus { get; set; }
        [BindProperty(SupportsGet = true)]
        public string PageMessage { get; set; } = "";
        public string PageMessageCssClass { get; set; } = "";


        public void OnGet()
        {
            GetSelectedStatus();
        }

        public IActionResult OnPostSaveChanges(Game game)
        {
            GetSelectedStatus();
            if (SelectedStatus != null)
            {
                SelectedStatus.Name = game.Name;
                context.SaveChanges();
                return RedirectToPage("/Statuses/Index");
            }
            else
            {
                SetPageMessage(string.Format("Could not find id: {0}", StatusId), CssClass.Error);
                return Page();
            }
        }

        public IActionResult OnPostCancelChanges()
        {
            return RedirectToPage("/Statuses/Index");
        }

        private void GetSelectedStatus()
        {
            SelectedStatus = context.Statuses
            .Where(s => s.Id == StatusId)
            .FirstOrDefault();
        }

        private void SetPageMessage(string message, CssClass css)
        {
            PageMessage = message;
            PageMessageCssClass = "page-message-" + css.ToString().ToLower();
        }
    }
}
