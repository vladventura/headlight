using Headlight.AppCode.Globals;
using Headlight.CustomPages;
using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;

namespace Headlight.Pages.Statuses
{
    public class AddModel(AppDbContext context) : PageTempData
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostSaveChanges(Status status)
        {
            context.Statuses.Add(status);
            context.SaveChanges();

            TempData[TempDataVars.MessageResult] = PageMessageResult.Success;
            TempData[TempDataVars.Message] = string.Format("{0} has been added!", status.Name);

            return RedirectToPage("/Statuses/Add");
        }

        public IActionResult OnPostCancelChanges()
        {
            return RedirectToPage("/Statuses/Index");
        }
    }
}
