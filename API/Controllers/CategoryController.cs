using API.Models;
using API.Repositories;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [Authorize(Roles = "HR, Employee, Finance")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryViewModel>>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            if (categories == null || !categories.Any())
                return NotFound(new
                {
                    statusCode = 404,
                    message = "Oops! No categories found. Please add some categories first!"
                });

            var categoryViewModels = categories.Select(c => new CategoryViewModel
            {
                Id_Category = c.Id_Category,
                Category_Name = c.Category_Name
            });

            return Ok(new
            {
                statusCode = 200,
                message = "Great! Categories retrieved successfully.",
                data = categoryViewModels
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryViewModel>> GetCategoryById(string id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(new
                {
                    statusCode = 404,
                    message = $"Hmm, we couldn't find a category with ID '{id}'. Please check the ID and try again!"
                });

            var categoryViewModel = new CategoryViewModel
            {
                Id_Category = category.Id_Category,
                Category_Name = category.Category_Name
            };
            return Ok(new
            {
                statusCode = 200,
                message = "Success! Category details retrieved.",
                data = categoryViewModel
            });
        }

        [HttpPost]
        public async Task<ActionResult<CategoryViewModel>> CreateCategory(CategoryViewModel categoryViewModel)
        {
            // Validasi: Jika Category_Name kosong
            if (string.IsNullOrEmpty(categoryViewModel.Category_Name))
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "Oops! Category name cannot be empty. Please provide a valid category name."
                });
            }

            var category = new Category
            {
                Category_Name = categoryViewModel.Category_Name
            };

            var createdCategory = await _categoryRepository.CreateCategoryAsync(category);
            var resultViewModel = new CategoryViewModel
            {
                Id_Category = createdCategory.Id_Category,
                Category_Name = createdCategory.Category_Name
            };

            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id_Category }, new
            {
                statusCode = 201,
                message = "Hooray! The category was created successfully.",
                data = resultViewModel
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, CategoryViewModel categoryViewModel)
        {
            // Validasi: Jika Category_Name kosong
            if (string.IsNullOrEmpty(categoryViewModel.Category_Name))
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "Oops! Category name cannot be empty. Please provide a valid category name."
                });
            }

            var category = new Category
            {
                Id_Category = id,
                Category_Name = categoryViewModel.Category_Name
            };

            var updatedCategory = await _categoryRepository.UpdateCategoryAsync(id, category);
            if (updatedCategory == null)
            {
                return NotFound(new
                {
                    statusCode = 404,
                    message = $"Oh no! We couldn't find a category with ID '{id}' to update. Please check the ID."
                });
            }

            return Ok(new
            {
                statusCode = 200,
                message = "Success! The category was updated."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var result = await _categoryRepository.DeleteCategoryAsync(id);
            if (!result)
                return NotFound(new
                {
                    statusCode = 404,
                    message = $"Yikes! No category found with ID '{id}' to delete. Please double-check and try again."
                });

            return Ok(new
            {
                statusCode = 200,
                message = "Done! The category has been deleted successfully."
            });
        }
    }
}
