using DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Web.ViewModels;

namespace Web.Controllers.Category
{
    public class CategoryController : Controller
    {
        private readonly Repository repository;

        public CategoryController(Repository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Route("categories")]
        public IActionResult OpenCategoriesList()
        {
            return View("CategoryList", new CategoryListViewModel() {Categories = FetchUserCategories()});
        }

        [Route("category/edit/{id:int}")]
        [HttpGet]
        public IActionResult OpenEditForm(int id)
        {
            var category = repository.Categories.FirstOrDefault(x => x.Id == id);
        
            if (category == null)
            {
                return View("CategoryList");
            }
        
            return View("CategoryForm", new CategoryViewModel()
            {
                Id = category.Id,
                Name = category.Name
            });
        }
        
        [HttpPost]
        public IActionResult Edit(CategoryViewModel viewModel)
        {
            var validation = ValidateData(viewModel);
        
            if (!string.IsNullOrWhiteSpace(validation))
            {
                TempData["Error"] = validation;
        
                return View("CategoryForm", viewModel);
            }
        
            var expense = FetchCategory(viewModel.Id);
        
            if (expense == null)
            {
                TempData["Error"] = "Something went wrong.";
        
                return View("CategoryForm", viewModel);
            }
        
            TempData["Success"] = "Sėkmingai atnaujinta kategorija!";
        
            UpdateCategory(expense, viewModel);
        
            var categories = FetchUserCategories();
        
            return View("CategoryList", new CategoryListViewModel() {Categories = categories});
        }
        
        [HttpGet]
        public IActionResult OpenCreationForm()
        {
            return View("CategoryForm", new CategoryViewModel());
        }
        
        [HttpPost]
        public IActionResult Create(CategoryViewModel viewModel)
        {
            var validation = ValidateData(viewModel);
        
            if (!string.IsNullOrWhiteSpace(validation))
            {
                TempData["Error"] = validation;
        
                return View("CategoryForm", viewModel);
            }
        
            Store(viewModel);
        
            TempData["Success"] = "Kategorija sėkmingai pridėta!";
        
            var expenses = FetchUserCategories();
        
            return View("CategoryList", new CategoryListViewModel() {Categories = expenses});
        }
        
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("DeleteConfirmForm",
                new DeletionViewModel {LtName = "kategoriją", Id = id, Controller = "Category", Method = "DeleteCategory"});
        }
        
        [HttpDelete]
        public IActionResult DeleteCategory(int id)
        {
            var category = repository.Categories.FirstOrDefault(x => x.Id == id);
        
            repository.Remove(category);
        
            repository.SaveChanges();
        
            TempData["Success"] = "Kategorija sėkmingai panaikinta!";
        
            return Ok(Url.Action("OpenCategoriesList", "Category"));
        }
        
        
        private void Store(CategoryViewModel viewModel)
        {
            var category = new DataAccess.Models.Category
            {
                Name = viewModel.Name?.Trim()
            };
        
            repository.Add(category);
        
            repository.SaveChanges();
        }
        
        private string ValidateData(CategoryViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Name))
            {
                return "Pavadinimo laukas turi būti užpildytas.";
            }
        
            return string.Empty;
        }
        
        private void UpdateCategory(DataAccess.Models.Category category, CategoryViewModel viewModel)
        {
            category.Name = viewModel.Name?.Trim();
            
            repository.Update(category);
        
            repository.SaveChanges();
        }
        
        
        private IList<DataAccess.Models.Category> FetchUserCategories()
        {
            return repository.Categories.ToList();
        }
        
        private DataAccess.Models.Category FetchCategory(int id)
        {
            return repository.Categories.FirstOrDefault(x => x.Id == id);
        }
    }
}