using Microsoft.AspNetCore.Mvc;
using nguyennhatquang_2280618967.Models;
using nguyennhatquang_2280618967.Repository;

namespace nguyennhatquang_2280618967.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading categories.";
                return View(new List<Category>());
            }
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryWithProductsAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.ProductCount = await _categoryRepository.GetProductCountByCategoryAsync(id);
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading category details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _categoryRepository.AddAsync(category);
                    TempData["SuccessMessage"] = $"Category '{category.Name}' has been created successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the category.";
            }

            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(category);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the category.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                TempData["ErrorMessage"] = "Invalid category data.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _categoryRepository.UpdateAsync(category);
                    TempData["SuccessMessage"] = $"Category '{category.Name}' has been updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the category.";
            }

            return View(category);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryWithProductsAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.ProductCount = await _categoryRepository.GetProductCountByCategoryAsync(id);
                return View(category);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the category.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Category/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                var productCount = await _categoryRepository.GetProductCountByCategoryAsync(id);

                // Sử dụng cascade delete (đã cấu hình trong DbContext)
                await _categoryRepository.DeleteAsync(id);

                if (productCount > 0)
                {
                    TempData["SuccessMessage"] = $"Category '{category.Name}' and {productCount} associated products have been deleted successfully.";
                }
                else
                {
                    TempData["SuccessMessage"] = $"Category '{category.Name}' has been deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the category. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        // API endpoint to get categories for dropdown
        [HttpGet]
        public async Task<IActionResult> GetCategoriesJson()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                var result = categories.Select(c => new { id = c.Id, name = c.Name });
                return Json(result);
            }
            catch
            {
                return Json(new List<object>());
            }
        }

        // Check if category name already exists
        [HttpPost]
        public async Task<IActionResult> CheckCategoryName(string name, int? id = null)
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                var exists = categories.Any(c => c.Name.ToLower() == name.ToLower() && c.Id != (id ?? 0));
                return Json(!exists);
            }
            catch
            {
                return Json(true); // If error, allow the name
            }
        }
    }
}