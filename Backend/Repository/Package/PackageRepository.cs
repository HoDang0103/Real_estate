using Backend.Models;
using Backend.Repository.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.Package
{
    public class PackageRepository : IPackageRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IPackageRepository _packageRepository;

        public PackageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddPackageAsync(Models.Package model)
        {
            _context.Packages.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task DeletePackageAsync(int id)
        {
            var postToDelete = await _context.Packages.FindAsync(id);

            if (postToDelete != null)
            {
                _context.Packages.Remove(postToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Models.Package>> GetAllPackageAsync()
        {
            return await _context.Packages.ToListAsync();
        }

        public async Task<Models.Package> GetPackageAsync(int id)
        {
            return await _context.Packages.FindAsync(id);
        }

        public async Task UpdatePackageAsync(int id, Models.Package model)
        {
            var existingPost = await _context.Packages.FindAsync(id);

            if (existingPost != null)
            {
                existingPost.Name = model.Name;
                existingPost.PricePerDay = model.PricePerDay;

                await _context.SaveChangesAsync();
            }
        }
    }
}
