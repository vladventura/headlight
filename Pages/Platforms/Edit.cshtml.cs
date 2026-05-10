using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Headlight.Pages.Platforms
{
    public class EditModel(AppDbContext context) : PageModel
    {
        private enum CssClass
        {
            Error,
            Success
        }

        [BindProperty(SupportsGet = true)]
        public int PlatformId { get; set; }
        public Platform? SelectedPlatform { get; set; }
        [BindProperty(SupportsGet = true)]
        public string PageMessage { get; set; } = "";
        public string PageMessageCssClass { get; set; } = "";


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
                SetPageMessage(string.Format("Could not find id: {0}", PlatformId), CssClass.Error);
                return Page();
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

        private void SetPageMessage(string message, CssClass css)
        {
            PageMessage = message;
            PageMessageCssClass = "page-message-" + css.ToString().ToLower();
        }
    }
}
