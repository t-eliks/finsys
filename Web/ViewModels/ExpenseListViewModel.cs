﻿using DataAccess.Models;
using System.Collections.Generic;

namespace Web.ViewModels
{
    public class ExpenseListViewModel
    {
        public IList<ExpenseViewModel> Expenses { get; set; }
    }
}
