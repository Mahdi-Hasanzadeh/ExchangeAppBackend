using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contract.User
{
    public interface IUserRepository
    {
        public Task<UserEntity?> GetUserByIdAsync(int id);

        public Task<IEnumerable<UserEntity>> GetAllUsersByParentUserIdAsync(int parentUserId);

        public Task<UserEntity?> GetUserByUsernameAsync(string username);

        public Task<UserEntity?> GetUserByUsernameAndParentIdAsync(string username, int parentUserId);

        public Task<UserEntity?> GetUserByEmailAsync(string email);

        public Task BeginTransaction();
        public Task CommitTransaction();
        public Task RollbackTransaction();

        public Task<bool> AddUserAsync(UserEntity user);

        public Task<bool> DeleteUserById(int id);

        public Task SaveAsync();

    }
}
