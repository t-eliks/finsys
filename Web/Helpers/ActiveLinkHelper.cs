using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Web.Helpers
{
    public static class ActiveLinkHelper
    {
        public static string IsActiveLink(string actionName, ViewDataDictionary viewData)
        {
            return viewData["Menu"]?.ToString() == actionName ? "link-active" : string.Empty;
        }
    }
}
