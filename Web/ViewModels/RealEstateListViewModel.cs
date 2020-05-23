using DataAccess.Models;
using System.Collections.Generic;

namespace Web.ViewModels
{
    public class RealEstateListViewModel
    {
        public IList<RealEstate> RealEstates { get; set; }
    }
}