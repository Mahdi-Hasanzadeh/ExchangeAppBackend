using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Contract.Settings;
using Shared.DTOs;
using Shared.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsRepo _settingsRepo;

        public SettingsController(ISettingsRepo settingsRepo)
        {
            _settingsRepo = settingsRepo;
        }


        [HttpGet("OwnerInfo", Name = "GetOwnerInfoByOwnerId")]
        public async Task<ActionResult<ApiResponse<OwnerInfo>>> GetOwnerInfoByOwnerId()
        {
            var apiResponse = new ApiResponse<OwnerInfo>(false);
            try
            {
                var currentUserId = User.GetUserId();
                var result = await _settingsRepo.GetOwnerInfoByIdAsync(currentUserId);

                apiResponse.Success = true;
                apiResponse.Data = result;
                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }

        }

        [HttpPost("OwnerInfo", Name = "UpdateOwnerInfoByOwnerId")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateOwnerInfoByOwnerId(OwnerInfo ownerInfo)
        {
            var apiResponse = new ApiResponse<bool>(false);
            try
            {
                var currentUserId = User.GetUserId();
                var result = await _settingsRepo.UpdateOwnerInfoByIdAsync(currentUserId, ownerInfo);

                if (!result)
                {
                    apiResponse.Message = "خطا در ثبت اطلاعات";
                    return Ok(apiResponse);
                }

                apiResponse.Success = true;
                apiResponse.Data = result;
                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }

        }

        [HttpGet("OwnerAppName",Name = "GetOwnerAppName")]
        public async Task<ActionResult<ApiResponse<string>>> GetOwnerAppName()
        {
            var apiResponse = new ApiResponse<string>(false);
            try
            {
                var currentUserId = User.GetUserId();
                var result = await _settingsRepo.GetOwnerAppNameByIdAsync(currentUserId);
                apiResponse.Success = true;
                apiResponse.Data = result;
                return Ok(apiResponse);

            }
            catch (Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.Message = "خطای در سرور رخ داده است";
                return StatusCode(StatusCodes.Status500InternalServerError, apiResponse);
            }
        }
    }
}
