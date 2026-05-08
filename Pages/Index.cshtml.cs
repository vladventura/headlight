using Headlight.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Headlight.Pages
{
    public class IndexModel(AppDbContext context) : PageModel
    {
        public void OnGet()
        {

        }
    }
}
