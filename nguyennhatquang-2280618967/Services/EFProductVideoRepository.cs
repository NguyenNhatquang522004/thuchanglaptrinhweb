using Microsoft.EntityFrameworkCore;
using nguyennhatquang_2280618967.Models;
using nguyennhatquang_2280618967.Repository;

namespace nguyennhatquang_2280618967.Services
{
    public class EFProductVideoRepository : IProductVideoRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductVideoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductVideo>> GetAllAsync()
        {
            return await _context.ProductVideos.ToListAsync();
        }

        public async Task<ProductVideo?> GetByIdAsync(int id)
        {
            return await _context.ProductVideos.FindAsync(id);
        }

        public async Task<IEnumerable<ProductVideo>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductVideos
                .Where(v => v.ProductId == productId)
                .ToListAsync();
        }

        public async Task AddAsync(ProductVideo productVideo)
        {
            await _context.ProductVideos.AddAsync(productVideo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductVideo productVideo)
        {
            _context.ProductVideos.Update(productVideo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var video = await GetByIdAsync(id);
            if (video != null)
            {
                _context.ProductVideos.Remove(video);
                await _context.SaveChangesAsync();
            }
        }
    }
}

