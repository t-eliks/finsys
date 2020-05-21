﻿using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Helpers
{
    public static class SiteCrumbs
    {
        public static CrumbViewModel Home(IUrlHelper urlHelper) => new CrumbViewModel { Name = "Namai", Link = urlHelper.Action("Index", "User") };

        public static CrumbViewModel ExpenseList(IUrlHelper urlHelper) => new CrumbViewModel { Name = "Išlaidos", Link = urlHelper.Action("OpenExpenseList", "Expense") };
        
        public static CrumbViewModel IncomeList(IUrlHelper urlHelper) => new CrumbViewModel { Name = "Pajamos", Link = urlHelper.Action("OpenIncomeList", "Income") };
        
        public static CrumbViewModel BudgetPlanning(IUrlHelper urlHelper) => new CrumbViewModel { Name = "Biudžeto planavimas", Link = urlHelper.Action("OpenPlanningPage", "Budget") };

        public static CrumbViewModel CategoryList(IUrlHelper urlHelper) => new CrumbViewModel { Name = "Kategorijų valdymas", Link = urlHelper.Action("OpenCategoriesList", "Category") };
    }
}
