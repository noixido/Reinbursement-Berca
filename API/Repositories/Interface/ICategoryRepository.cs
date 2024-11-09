using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;

namespace API.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(string id);
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(string id, Category category);
        Task<bool> DeleteCategoryAsync(string id);
    }
}
