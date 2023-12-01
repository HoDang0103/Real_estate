using Backend.Repository.Catalog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogRepository catalogRepository;

        public CatalogController(ICatalogRepository catalogRepository)
        {
            this.catalogRepository = catalogRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCatalogs()
        {
            var catalogs = await catalogRepository.GetAllCatalogAsync();
            return Ok(catalogs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCatalogById(int id)
        {
            var catalog = await catalogRepository.GetCatalogAsync(id);

            if (catalog == null)
            {
                return NotFound();
            }

            return Ok(catalog);
        }

        [HttpPost]
        public async Task<IActionResult> AddCatalog(Models.Catalog catalog)
        {
            if (catalog == null)
            {
                return BadRequest();
            }

            var newCatalogId = await catalogRepository.AddCatalogAsync(catalog);

            return CreatedAtAction(nameof(GetCatalogById), new { id = newCatalogId }, catalog);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCatalog(int id, Models.Catalog catalog)
        {
            if (catalog == null)
            {
                return BadRequest();
            }

            await catalogRepository.UpdateCatalogAsync(id, catalog);

            return StatusCode(201);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatalog(int id)
        {
            await catalogRepository.DeleteCatalogAsync(id);

            return StatusCode(201);
        }
    }
}
