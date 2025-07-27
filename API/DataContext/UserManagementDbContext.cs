using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace API.DataContext
{
    //public class UserManagementDbContext : DbContext
    //{
    //    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options) { }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        // Configure the self-referencing relationship for ParentUserId
    //        modelBuilder.Entity<UserEntity>()
    //            .HasOne(u => u.ParentUser)
    //            .WithMany(u => u.SubUsers)
    //            .HasForeignKey(u => u.ParentUserId)
    //            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete/update

   
    //    }



    //    public DbSet<UserEntity> Users { get; set; }
    //}
}
