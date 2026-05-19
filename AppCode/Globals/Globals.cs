using Headlight.CustomPages;

namespace Headlight.AppCode.Globals
{
    public enum PageMessageResult
    {
        Success,
        Error
    }

    public class TempDataVars
    {
        public const string Message = nameof(PageTempData.Message);
        public const string MessageResult = nameof(PageTempData.MessageResult);
    }
}
