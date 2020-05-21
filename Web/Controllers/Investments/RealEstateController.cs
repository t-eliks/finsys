using DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Web.ViewModels;

namespace Web.Controllers.Investments
{
    public class RealEstateController : Controller
    {
        private readonly Repository repository;

        public RealEstateController(Repository repository)
        {
            this.repository = repository;
        }
        
        [HttpGet]
        [Route("real-estates")]
        public IActionResult OpenRealEstateList()
        {
            return View("RealEstateList", new RealEstateListViewModel() {RealEstates = FetchUserRealEstate()});
        }
        
        [Route("real-estates/edit/{id:int}")]
        [HttpGet]
        public IActionResult OpenEditForm(int id)
        {
            var realEstate = FetchRealEstate(id);
        
            if (realEstate == null)
            {
                return View("RealEstateList");
            }
        
            return View("RealEstateForm", new RealEstateViewModel()
            {
                Id = realEstate.Id,
                Name = realEstate.Name,
                Address = realEstate.Address,
                IsRented = realEstate.IsRented,
                SquareSpace = realEstate.SquareSpace,
                RoomNumber = realEstate.RoomNumber,
                Valuation = realEstate.Valuation,
            });
        }
        
        [HttpPost]
        public IActionResult Edit(RealEstateViewModel viewModel)
        {
            var validation = ValidateData(viewModel);
        
            if (!string.IsNullOrWhiteSpace(validation))
            {
                TempData["Error"] = validation;
        
                return View("RealEstateForm", viewModel);
            }
        
            var realEstate = FetchRealEstate(viewModel.Id);
        
            if (realEstate == null)
            {
                TempData["Error"] = "Something went wrong.";
        
                return View("RealEstateForm", viewModel);
            }
        
            TempData["Success"] = "Sėkmingai atnaujintas NT!";
        
            Update(realEstate, viewModel);
        
            var realEstates = FetchUserRealEstate();
        
            return View("RealEstateList", new RealEstateListViewModel() {RealEstates = realEstates});
        }
        
        [HttpGet]
        public IActionResult OpenCreationForm()
        {
            return View("RealEstateForm", new RealEstateViewModel());
        }
        
        [HttpPost]
        public IActionResult Create(RealEstateViewModel viewModel)
        {
            var validation = ValidateData(viewModel);
        
            if (!string.IsNullOrWhiteSpace(validation))
            {
                TempData["Error"] = validation;
        
                return View("RealEstateForm", viewModel);
            }
        
            Store(viewModel);
        
            TempData["Success"] = "NT sėkmingai pridėtas!";
        
            var realEstates = FetchUserRealEstate();
        
            return View("RealEstateList", new RealEstateListViewModel() {RealEstates = realEstates});
        }
        
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("DeleteConfirmForm",
                new DeletionViewModel {LtName = "nekilnojamas turtas", Id = id, Controller = "RealEstate", Method = "DeleteRealEstate"});
        }
        
        [HttpDelete]
        public IActionResult DeleteRealEstate(int id)
        {
            var realEstate = repository.RealEstate.FirstOrDefault(x => x.Id == id);
        
            repository.Remove(realEstate);
        
            repository.SaveChanges();
        
            TempData["Success"] = "NT sėkmingai panaikintas!";
        
            return Ok(Url.Action("OpenRealEstateList", "RealEstate"));
        }
        
        private void Store(RealEstateViewModel viewModel)
        {
            var realEstate = new DataAccess.Models.RealEstate
            {
                Name = viewModel.Name,
                Address = viewModel.Address,
                IsRented = viewModel.IsRented,
                SquareSpace = viewModel.SquareSpace.Value,
                RoomNumber = viewModel.RoomNumber.Value,
                Valuation = viewModel.Valuation.Value,
            };
        
            repository.Add(realEstate);
        
            repository.SaveChanges();
        }

        private void Update(DataAccess.Models.RealEstate realEstate, RealEstateViewModel viewModel)
        {
            realEstate.Name = viewModel.Name;
            realEstate.Address = viewModel.Address;
            realEstate.IsRented = viewModel.IsRented;
            realEstate.SquareSpace = viewModel.SquareSpace.Value;
            realEstate.RoomNumber = viewModel.RoomNumber.Value;
            realEstate.Valuation = viewModel.Valuation.Value;
            
            repository.Update(realEstate);
        
            repository.SaveChanges();
        }
        
        private string ValidateData(RealEstateViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Name))
            {
                return "Pavadinimo laukas turi būti užpildytas.";
            }
        
            if (string.IsNullOrWhiteSpace(viewModel.Address))
            {
                return "Adreso laukas turi būti užpildytas.";
            }
            
            if (!viewModel.SquareSpace.HasValue)
            {
                return "Plotas yra privalomas";
            }

            if (viewModel.SquareSpace <= 0)
            {
                return "Plotas turi būti daugiau už 0.";
            }
            
            if (!viewModel.RoomNumber.HasValue)
            {
                return "Kambarių skaičius yra privalomas";
            }

            if (viewModel.RoomNumber <= 0)
            {
                return "Kambarių skaičius būti daugiau už 0.";
            }
            
            if (!viewModel.Valuation.HasValue)
            {
                return "Vertė yra privaloma";
            }

            if (viewModel.Valuation <= 0)
            {
                return "Vertė turi būti daugiau už 0.";
            }
            
            return string.Empty;
        }
        
        private IList<DataAccess.Models.RealEstate> FetchUserRealEstate()
        {
            return repository.RealEstate.ToList();
        }
        
        private DataAccess.Models.RealEstate FetchRealEstate(int id)
        {
            return repository.RealEstate.FirstOrDefault(x => x.Id == id);
        }
    }
}