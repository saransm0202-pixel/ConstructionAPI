using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSConstructions.Repository.Interface;
using SSConstructions.Repository.Models;
using SSConstructions.Repository.Utility;

namespace SSConstructions.Controllers.API
{
    [ApiController]
    [EnableCors("addCors")]
    [Route("api/[controller]")]
    public class UserAPIController : Controller
    {
        private readonly IUsers _usersRepo;
        private readonly CustomLogger _logger;

        public UserAPIController(IUsers userRepo, CustomLogger logger)
        {
            _usersRepo = userRepo;
            _logger = logger;
        }
        [Route("getRoles")]
        [HttpGet()]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                _logger.Log("UserAPIController.GetRoles Started.");
                return Ok(await _usersRepo.GetRoles());
                _logger.Log("UserAPIController.GetRoles Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }
        [Route("getUsers")]
        [HttpGet()]
        public async Task<IActionResult> GetUsers(int accountId)
        {
            try
            {
                _logger.Log("UserAPIController.GetUsers Started.");
                return Ok(await _usersRepo.GetUsers(accountId));
                _logger.Log("UserAPIController.GetUsers Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }
        [Route("InsertUpdateUser")]
        [HttpPost()]
        public async Task<IActionResult> InsertUpdateUser(UserModel userModel)
        {
            try
            {
                _logger.Log("UserAPIController.InsertUpdateUser Started.");
                return Ok(await _usersRepo.InsertUpdateUser(userModel));
                _logger.Log("UserAPIController.InsertUpdateUser Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
