using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Headlight.Pages.Platforms
{
    public class AddModel(AppDbContext context) : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string PageMessage { get; set; } = "";
        [BindProperty(SupportsGet = true)]
        public string PageMessageCssClass { get; set; } = "";

        private enum CssClass
        {
            Error,
            Success
        }

        public void OnGet()
        {
        }

        public IActionResult OnPostSaveChanges(Platform platform)
        {
            context.Platforms.Add(platform);
            context.SaveChanges();
            string pageMessage = string.Format("{0} has been added!", platform.Name);
            SetPageMessage(pageMessage, CssClass.Success);
            return Page();
        }

        public IActionResult OnPostCancelChanges()
        {
            return RedirectToPage("/Platforms/Index");
        }

        private void SetPageMessage(string message, CssClass css)
        {
            PageMessage = message;
            PageMessageCssClass = "page-message-" + css.ToString().ToLower();
        }
    }
}
