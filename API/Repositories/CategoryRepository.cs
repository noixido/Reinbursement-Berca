using API.Context;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MyContext _context;

        public CategoryRepository(MyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(string id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            var lastCategory = await _context.Categories
                .OrderByDescending(c => c.Id_Category)
                .FirstOrDefaultAsync();

            int nextId = 1;

            if (lastCategory != null && lastCategory.Id_Category.StartsWith("C"))
            {
                if (int.TryParse(lastCategory.Id_Category.Substring(1), out int lastId))
                {
                    nextId = lastId + 1;
                }
            }

            category.Id_Category = "C" + nextId.ToString("D3");

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }


        public async Task<Category> UpdateCategoryAsync(string id, Category category)
        {
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null) return null;

            existingCategory.Category_Name = category.Category_Name;
            await _context.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<bool> DeleteCategoryAsync(string id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
