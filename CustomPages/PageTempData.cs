using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Headlight.CustomPages
{
    public class PageTempData : PageModel
    {
        [TempData]
        public string? Message { get; set; } = "";
        [TempData]
        public int? MessageResult { get; set; }
    }
}
