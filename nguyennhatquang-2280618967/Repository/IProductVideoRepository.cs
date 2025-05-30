using nguyennhatquang_2280618967.Models;

namespace nguyennhatquang_2280618967.Repository
{
    public interface IProductVideoRepository
    {
        Task<IEnumerable<ProductVideo>> GetAllAsync();
        Task<ProductVideo?> GetByIdAsync(int id);
        Task<IEnumerable<ProductVideo>> GetByProductIdAsync(int productId);
        Task AddAsync(ProductVideo productVideo);
        Task UpdateAsync(ProductVideo productVideo);
        Task DeleteAsync(int id);
    }
}
