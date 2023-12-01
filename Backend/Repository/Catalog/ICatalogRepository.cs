namespace Backend.Repository.Catalog
{
    public interface ICatalogRepository
    {
        public Task<List<Models.Catalog>> GetAllCatalogAsync();
        public Task<Models.Catalog> GetCatalogAsync(int id);
        public Task<int> AddCatalogAsync(Models.Catalog model);
        public Task UpdateCatalogAsync(int id, Models.Catalog model);
        public Task DeleteCatalogAsync(int id);
    }
}
