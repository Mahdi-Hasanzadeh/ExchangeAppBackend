using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contract.Settings
{
    public interface ISettingsRepo
    {
        public Task<OwnerInfo> GetOwnerInfoByIdAsync(int ownerId);

        public Task<bool> UpdateOwnerInfoByIdAsync(int ownerId, OwnerInfo ownerInfo);

        public Task<string?> GetOwnerAppNameByIdAsync(int ownerId);

    }
}
