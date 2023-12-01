using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.Catalog
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICatalogRepository _catalogRepository;

        public CatalogRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<int> AddCatalogAsync(Models.Catalog model)
        {
            _context.Catalogs.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task DeleteCatalogAsync(int id)
        {
            var postToDelete = await _context.Catalogs.FindAsync(id);

            if (postToDelete != null)
            {
                _context.Catalogs.Remove(postToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Models.Catalog>> GetAllCatalogAsync()
        {
           return await _context.Catalogs.ToListAsync();
        }

        public async Task<Models.Catalog> GetCatalogAsync(int id)
        {
            return await _context.Catalogs.FindAsync(id);
        }

        public async Task UpdateCatalogAsync(int id, Models.Catalog model)
        {
            var existingPost = await _context.Catalogs.FindAsync(id);

            if (existingPost != null)
            {
                existingPost.CatalogName = model.CatalogName; 

                await _context.SaveChangesAsync();
            }
        }
    }
}
