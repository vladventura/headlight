using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Headlight.Pages.Statuses
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

        public IActionResult OnPostSaveChanges(Status status)
        {
            context.Statuses.Add(status);
            context.SaveChanges();
            string pageMessage = string.Format("{0} has been added!", status.Name);
            SetPageMessage(pageMessage, CssClass.Success);
            // TODO: Consider moving this to ViewData
            return RedirectToPage("/Statuses/Add", new { 
                PageMessage,
                PageMessageCssClass
            });
        }

        public IActionResult OnPostCancelChanges()
        {
            return RedirectToPage("/Statuses/Index");
        }

        private void SetPageMessage(string message, CssClass css)
        {
            PageMessage = message;
            PageMessageCssClass = "page-message-" + css.ToString().ToLower();
        }
    }
}
