using API.DataContext;
using Microsoft.EntityFrameworkCore;
using Shared.Contract.User;
using Shared.Models;

namespace API.Repositories.User
{
    public class UserRepository : IUserRepository
    {

        //private readonly UserManagementDbContext _context;

        //public UserRepository(UserManagementDbContext context)
        //{
        //    _context = context;
        //}

        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddUserAsync(UserEntity user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task BeginTransaction()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransaction()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task<bool> DeleteUserById(int id)
        {
            int affectedRows = await _context.Users.Where(u => u.Id == id).ExecuteDeleteAsync();
            return affectedRows > 0;
        }

        public async Task<IEnumerable<UserEntity>> GetAllUsersByParentUserIdAsync(int parentUserId)
        {
            var subUsers = await _context.Users
                .AsNoTracking()
                .Where(user => user.ParentUserId == parentUserId).ToListAsync();
            if (subUsers.Count == 0)
            {
                return [];
            }
            return subUsers;
        }

        public async Task<UserEntity?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<UserEntity?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

        public async Task<UserEntity?> GetUserByUsernameAndParentIdAsync(string username, int parentUserId)
        {
            return await _context.Users
                .Where(u => u.Username == username && u.ParentUserId == parentUserId)
                .FirstOrDefaultAsync();
        }

        public async Task<UserEntity?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }
            //var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            var user = await _context.Users
                    .FirstOrDefaultAsync(u => EF.Functions.Collate(u.Username, "Latin1_General_100_CS_AS_SC_UTF8") == username);


            return user;
        }

        public async Task RollbackTransaction()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
