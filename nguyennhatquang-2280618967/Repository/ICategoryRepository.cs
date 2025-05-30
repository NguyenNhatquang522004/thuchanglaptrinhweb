using nguyennhatquang_2280618967.Models;

namespace nguyennhatquang_2280618967.Repository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);

        Task DeleteWithProductsAsync(int id);
        Task<Category> GetCategoryWithProductsAsync(int id);
        Task<int> GetProductCountByCategoryAsync(int categoryId);
    }
}
