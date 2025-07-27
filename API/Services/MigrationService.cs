using API.DataContext;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class MigrationService
    {
        //private readonly UserManagementDbContext _userManagement;

        //public MigrationService(UserManagementDbContext userManagement)
        //{
        //    _userManagement = userManagement;
        //}

        //public async Task ApplyMigrationsToAllCustomersAsync()
        //{
        //    var connections = await _userManagement.Users.AsNoTracking().Where(u => u.ParentUserId == null)
        //        .Select(u => u.ConnectionString)
        //        .ToListAsync();

        //    foreach (var conn in connections)
        //    {
        //        var userDbService = new UserDatabaseService();
        //        userDbService.SetConnectionString(conn);
        //        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        //        using var context = new AppDbContext(optionsBuilder.Options, userDbService);
        //        context.Database.Migrate(); // Apply the latest migration
        //    }
        //}
    }

}
