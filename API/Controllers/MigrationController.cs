using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MigrationController : ControllerBase
    {
        //private readonly MigrationService _migrationService;
        //public MigrationController(MigrationService migrationService)
        //{
        //    _migrationService = migrationService;
        //}

        //[HttpPost("apply-migrations")]
        //public async Task<IActionResult> ApplyMigrations()
        //{
        //    try
        //    {
        //        await _migrationService.ApplyMigrationsToAllCustomersAsync();
        //        return Ok(new { message = "Migrations applied successfully to all customer databases." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = "An error occurred while applying migrations.", error = ex.Message });
        //    }
        //}
    }
}
