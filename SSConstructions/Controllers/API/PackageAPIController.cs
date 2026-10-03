using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SSConstructions.Repository.Interface;
using SSConstructions.Repository.Models;
using SSConstructions.Repository.Utility;
using System;
using System.Threading.Tasks;

namespace SSConstructions.Controllers.API
{
    [ApiController]
    [EnableCors("addCors")]
    [Route("api/[controller]")]
    public class PackageAPIController : Controller
    {
        private readonly IPackageRepo _packageRepo;
        private readonly CustomLogger _logger;

        public PackageAPIController(IPackageRepo packageRepo, CustomLogger logger)
        {
            _packageRepo = packageRepo;
            _logger = logger;
        }

        [Route("getPackages")]
        [HttpGet()]
        public async Task<IActionResult> GetPackages(int accountId)
        {
            try
            {
                _logger.Log("PackageAPIController.GetPackages Started.");
                return Ok(await _packageRepo.GetPackages(accountId));
                _logger.Log("PackageAPIController.GetPackages Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [Route("getConstructionTypes")]
        [HttpGet()]
        public async Task<IActionResult> GetConstructionTypes()
        {
            try
            {
                _logger.Log("PackageAPIController.GetConstructionTypes Started.");
                return Ok(await _packageRepo.GetConstructionTypes());
                _logger.Log("PackageAPIController.GetConstructionTypes Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [Route("getPackageSpecTypes")]
        [HttpGet()]
        public async Task<IActionResult> GetPackageSpecTypes()
        {
            try
            {
                _logger.Log("PackageAPIController.GetPackageSpecTypes Started.");
                return Ok(await _packageRepo.GetPackageSpecTypes());
                _logger.Log("PackageAPIController.GetPackageSpecTypes Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [Route("SavePackage")]
        [HttpPost()]
        public async Task<IActionResult> SavePackage(PackageModel packageModel)
        {
            try
            {
                _logger.Log("PackageAPIController.SavePackage Started.");
                return Ok(await _packageRepo.SavePackage(packageModel));
                _logger.Log("PackageAPIController.SavePackage Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [Route("DeletePackage")]
        [HttpPost()]
        public async Task<IActionResult> DeletePackage(int packageId)
        {
            try
            {
                _logger.Log("PackageAPIController.DeletePackage Started.");
                return Ok(await _packageRepo.DeletePackage(packageId));
                _logger.Log("PackageAPIController.DeletePackage Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }
    }
}