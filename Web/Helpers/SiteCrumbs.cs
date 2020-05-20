using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Helpers
{
    public static class SiteCrumbs
    {
        public static CrumbViewModel Home(IUrlHelper urlHelper) => new CrumbViewModel { Name = "Namai", Link = urlHelper.Action("Index", "User") };

        public static CrumbViewModel ExpenseList(IUrlHelper urlHelper) => new CrumbViewModel { Name = "Išlaidos", Link = urlHelper.Action("OpenExpenseList", "Expense") };

        public static CrumbViewModel LoanList(IUrlHelper urlHelper) => new CrumbViewModel
            { Name = "Paskolos", Link = urlHelper.Action("Index", "Loan") };
    }
}
