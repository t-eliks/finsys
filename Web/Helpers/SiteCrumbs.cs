using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Helpers
{
    public static class SiteCrumbs
    {
        public static CrumbViewModel Home(IUrlHelper urlHelper) => new CrumbViewModel
            {Name = "Namai", Link = urlHelper.Action("Index", "User")};

        public static CrumbViewModel ExpenseList(IUrlHelper urlHelper) => new CrumbViewModel
            {Name = "Išlaidos", Link = urlHelper.Action("OpenExpenseList", "Expense")};

        public static CrumbViewModel LoanList(IUrlHelper urlHelper) => new CrumbViewModel
            {Name = "Paskolos", Link = urlHelper.Action("Index", "Loan")};

        public static CrumbViewModel LoanPlanner(IUrlHelper urlHelper) => new CrumbViewModel
            {Name = "Paskolų planuoklė", Link = urlHelper.Action("OpenLoanPlanner", "Loan")};

        public static CrumbViewModel StockList(IUrlHelper urlHelper) => new CrumbViewModel
            {Name = "Akcijos", Link = urlHelper.Action("OpenStockList", "Stock")};

        public static CrumbViewModel StockReport(IUrlHelper urlHelper) => new CrumbViewModel
            {Name = "Akcijų ataskaita", Link = urlHelper.Action("OpenStocksReport", "Stock")};

        public static CrumbViewModel IncomeList(IUrlHelper urlHelper) => new CrumbViewModel
            {Name = "Pajamos", Link = urlHelper.Action("OpenIncomeList", "Income")};

        public static CrumbViewModel BudgetPlanning(IUrlHelper urlHelper) => new CrumbViewModel
            {Name = "Biudžeto planavimas", Link = urlHelper.Action("OpenPlanningPage", "Budget")};

        public static CrumbViewModel CategoryList(IUrlHelper urlHelper) => new CrumbViewModel
            {Name = "Kategorijų valdymas", Link = urlHelper.Action("OpenCategoriesList", "Category")};

        public static CrumbViewModel RealEstateList(IUrlHelper urlHelper) => new CrumbViewModel
            {Name = "NT valdymas", Link = urlHelper.Action("OpenRealEstateList", "RealEstate")};

        public static CrumbViewModel ExportPage(IUrlHelper urlHelper) => new CrumbViewModel
            {Name = "Duomenų eksportavimas", Link = urlHelper.Action("OpenExportPage", "ExternalData")};
    }
}