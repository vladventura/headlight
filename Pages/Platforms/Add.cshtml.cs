using Headlight.AppCode.Globals;
using Headlight.CustomPages;
using Headlight.Data;
using Headlight.Models;
using Microsoft.AspNetCore.Mvc;

namespace Headlight.Pages.Platforms
{
    public class AddModel(AppDbContext context) : PageTempData
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostSaveChanges(Platform platform)
        {
            context.Platforms.Add(platform);
            context.SaveChanges();

            TempData[TempDataVars.MessageResult] = PageMessageResult.Success;
            TempData[TempDataVars.Message] = string.Format("{0} has been added!", platform.Name);

            return RedirectToPage("/Platforms/Add");
        }

        public IActionResult OnPostCancelChanges()
        {
            return RedirectToPage("/Platforms/Index");
        }
    }
}
