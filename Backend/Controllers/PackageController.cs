using Backend.Repository.Catalog;
using Backend.Repository.Package;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageRepository packageRepository;

        public PackageController(IPackageRepository packageRepository)
        {
            this.packageRepository = packageRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPackages()
        {
            var catalogs = await packageRepository.GetAllPackageAsync();
            return Ok(catalogs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackageById(int id)
        {
            var catalog = await packageRepository.GetPackageAsync(id);

            if (catalog == null)
            {
                return NotFound();
            }

            return Ok(catalog);
        }

        [HttpPost]
        public async Task<IActionResult> AddPackage(Models.Package catalog)
        {
            if (catalog == null)
            {
                return BadRequest();
            }

            var newCatalogId = await packageRepository.AddPackageAsync(catalog);

            return CreatedAtAction(nameof(GetPackageById), new { id = newCatalogId }, catalog);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePackage(int id, Models.Package catalog)
        {
            if (catalog == null)
            {
                return BadRequest();
            }

            await packageRepository.UpdatePackageAsync(id, catalog);

            return StatusCode(201);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            await packageRepository.DeletePackageAsync(id);

            return StatusCode(201);
        }
    }
}
