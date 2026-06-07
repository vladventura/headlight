using Headlight.AppCode.Globals;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Headlight.CustomPages
{
    public class PageTempData : PageModel, IPageTempData
    {
        [TempData]
        public string? Message { get; set; } = "";
        [TempData]
        public int? MessageResult { get; set; }

        public string PageMessageCssClass
        {
            get
            {
                if (MessageResult == null)
                {
                    return "";
                }

                return (PageMessageResult)MessageResult! switch
                {
                    PageMessageResult.Success => "page-message-success",
                    PageMessageResult.Error => "page-message-error",
                    _ => ""
                };
            }
        }
    }
}
