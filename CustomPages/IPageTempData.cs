using Microsoft.AspNetCore.Mvc;

namespace Headlight.CustomPages
{
    public interface IPageTempData
    {
        [TempData]
        public string? Message { get; set; }
        [TempData]
        public int? MessageResult { get; set; }
    }
}
