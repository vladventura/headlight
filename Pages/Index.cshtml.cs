using Headlight.Data;
using Headlight.Models.JSON;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Headlight.Pages
{
    public class IndexModel(AppDbContext context) : PageModel
    {
        [BindProperty]
        public IFormFile? UploadedFile { get; set; }
        public void OnGet()
        {

        }

        public IActionResult OnPostAsync()
        {
            if (UploadedFile != null)
            {
                using var reader = new StreamReader(UploadedFile.OpenReadStream());
                string content = reader.ReadToEnd();
                var jsonData = JsonSerializer.Deserialize<ImportJSON>(content);
            }

            return RedirectToPage("/Index");
        }
    }
}
