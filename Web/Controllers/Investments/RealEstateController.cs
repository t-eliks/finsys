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
            return View("RealEstateList", new RealEstateListViewModel() {RealEstates = SelectUserRealEstate()});
        }
        
        [Route("real-estates/edit/{id:int}")]
        [HttpGet]
        public IActionResult OpenEditForm(int id)
        {
            var realEstate = repository.RealEstate.FirstOrDefault(x => x.Id == id);
        
            if (realEstate == null)
            {
                return View("RealEstateList");
            }
        
            return View("RealEstateForm", new RealEstateViewModel()
            {
                Id = realEstate.Id,
                Name = realEstate.Name
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
        
            UpdateRealEstate(realEstate, viewModel);
        
            var realEstates = SelectUserRealEstate();
        
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
        
            InsertRealEstate(viewModel);
        
            TempData["Success"] = "NT sėkmingai pridėtas!";
        
            var realEstates = SelectUserRealEstate();
        
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
        
        private void InsertRealEstate(RealEstateViewModel viewModel)
        {
            var realEstate = new DataAccess.Models.RealEstate
            {
                Name = viewModel.Name?.Trim()
            };
        
            repository.Add(realEstate);
        
            repository.SaveChanges();
        }

        private void UpdateRealEstate(DataAccess.Models.RealEstate realEstate, RealEstateViewModel viewModel)
        {
            realEstate.Name = viewModel.Name?.Trim();
            
            repository.Update(realEstate);
        
            repository.SaveChanges();
        }
        
        private string ValidateData(RealEstateViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Name))
            {
                return "Pavadinimo laukas turi būti užpildytas.";
            }
        
            return string.Empty;
        }
        
        private IList<DataAccess.Models.RealEstate> SelectUserRealEstate()
        {
            return repository.RealEstate.ToList();
        }
        
        private DataAccess.Models.RealEstate FetchRealEstate(int id)
        {
            return repository.RealEstate.FirstOrDefault(x => x.Id == id);
        }
    }
}