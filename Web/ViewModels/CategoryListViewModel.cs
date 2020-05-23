using System.Collections.Generic;
using DataAccess.Models;

namespace Web.ViewModels
{
    public class CategoryListViewModel
    {
        public IList<Category> Categories { get; set; }
    }
}