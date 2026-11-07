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
    public class ExtrasAPIController : Controller
    {
        private readonly IExtraRepo _extraRepo;
        private readonly CustomLogger _logger;

        public ExtrasAPIController(IExtraRepo extraRepo, CustomLogger logger)
        {
            _extraRepo = extraRepo;
            _logger = logger;
        }

        [Route("getExtras")]
        [HttpGet()]
        public async Task<IActionResult> GetExtras(int accountId)
        {
            try
            {
                _logger.Log("ExtrasAPIController.GetExtras Started.");
                return Ok(await _extraRepo.GetExtras(accountId));
                _logger.Log("ExtrasAPIController.GetExtras Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [Route("SaveExtra")]
        [HttpPost()]
        public async Task<IActionResult> SaveExtra(ExtraModel extraModel)
        {
            try
            {
                _logger.Log("ExtrasAPIController.SaveExtra Started.");
                return Ok(await _extraRepo.SaveExtra(extraModel));
                _logger.Log("ExtrasAPIController.SaveExtra Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

        [Route("DeleteExtra")]
        [HttpPost()]
        public async Task<IActionResult> DeleteExtra(int extrasId)
        {
            try
            {
                _logger.Log("ExtrasAPIController.DeleteExtra Started.");
                return Ok(await _extraRepo.DeleteExtra(extrasId));
                _logger.Log("ExtrasAPIController.DeleteExtra Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
