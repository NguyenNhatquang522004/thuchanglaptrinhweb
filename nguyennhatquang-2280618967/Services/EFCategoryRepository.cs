using Microsoft.EntityFrameworkCore;
using nguyennhatquang_2280618967.Models;
using nguyennhatquang_2280618967.Repository;

namespace nguyennhatquang_2280618967.Services
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        // CÁCH 1: Sử dụng Cascade Delete (nếu đã cấu hình trong DbContext)
        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                // Với Cascade Delete, tất cả products sẽ tự động bị xóa
            }
        }

        // CÁCH 2: Xóa thủ công (nếu vẫn muốn dùng DeleteBehavior.Restrict)
        public async Task DeleteWithProductsAsync(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Tìm và xóa tất cả products thuộc category này
                    var products = await _context.Products
                        .Where(p => p.CategoryId == id)
                        .ToListAsync();

                    if (products.Any())
                    {
                        _context.Products.RemoveRange(products);
                    }

                    // Xóa category
                    var category = await _context.Categories.FindAsync(id);
                    if (category != null)
                    {
                        _context.Categories.Remove(category);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        // Phương thức hỗ trợ: Lấy category kèm products
        public async Task<Category> GetCategoryWithProductsAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Phương thức hỗ trợ: Đếm số products trong category
        public async Task<int> GetProductCountByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .CountAsync(p => p.CategoryId == categoryId);
        }
    }
}