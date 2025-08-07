using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Contract;
using Shared.DTOs;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupController : ControllerBase
    {
        private readonly ILogger<BackupController> _logger;
        private readonly IBackupService _backupService;

        public BackupController(ILogger<BackupController> logger,
            IBackupService backupService)
        {
            _logger = logger;
            _backupService = backupService;
        }
        [HttpGet("{userId}", Name = "Get backup of user's data")]
        [Authorize]
        public async Task<IActionResult> GetBackupFromUserData(int userId)
        {
            var apiResponse = new ApiResponse<dynamic>(false);
            try
            {
                //var currentUserId = User.GetUserId();
                dynamic backupJsonFile = await _backupService.BackupUserDataAsync(userId);

                byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(backupJsonFile);
                string fileName = $"User_{userId}_Backup_{DateTime.Now:yyyyMMddHHmmss}.json";

                // Return as a downloadable file
                //return ;
                return File(fileBytes, "application/json", fileName);
                //apiResponse.Success = true;
                //apiResponse.Message = "عملیات با موفقیت انجام شد.";
                //apiResponse.Data = File(fileBytes, "application/json", fileName);
                //return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                apiResponse.Message = "خطای در سرور رخ داده است.";
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("restore/{userId}")]
        [Authorize]
        public async Task<IActionResult> RestoreUserData(int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using var stream = new StreamReader(file.OpenReadStream());
            var json = await stream.ReadToEndAsync();

            var result = await _backupService.RestoreUserDataAsync(userId, json);
            if (!result)
                return StatusCode(500, "Restore failed.");

            return Ok("User data restored successfully.");
        }

    }
}
