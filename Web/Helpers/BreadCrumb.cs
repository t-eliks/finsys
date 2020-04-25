using System.Collections.Generic;
using Web.ViewModels;

namespace Web.Helpers
{
    public class BreadCrumb
    {
        public List<CrumbViewModel> Crumbs { get; set; } = new List<CrumbViewModel>();

        public BreadCrumb Append(CrumbViewModel crumb)
        {
            Crumbs.Add(crumb);

            return this;
        }
    }
}
