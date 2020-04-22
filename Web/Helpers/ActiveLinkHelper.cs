using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Helpers
{
    public static class ActiveLinkHelper
    {
        public static string IsActiveLink(string actionName, ViewContext viewContext)
        {
            return viewContext.RouteData.Values["Action"].ToString() == actionName ? "link-active" : string.Empty;
        }
    }
}
