namespace Backend.Repository.Package
{
    public interface IPackageRepository
    {
        public Task<List<Models.Package>> GetAllPackageAsync();
        public Task<Models.Package> GetPackageAsync(int id);
        public Task<int> AddPackageAsync(Models.Package model);
        public Task UpdatePackageAsync(int id, Models.Package model);
        public Task DeletePackageAsync(int id);
    }
}
