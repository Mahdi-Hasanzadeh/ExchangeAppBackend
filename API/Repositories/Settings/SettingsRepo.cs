using API.DataContext;
using Microsoft.EntityFrameworkCore;
using Shared.Contract.Settings;
using Shared.Models;

namespace API.Repositories.Settings
{
    public class SettingsRepo : ISettingsRepo
    {
        private readonly AppDbContext _context;

        public SettingsRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string?> GetOwnerAppNameByIdAsync(int ownerId)
        {
            return await _context.OwnerInfos.AsNoTracking().
                Where(owner => owner.UserId == ownerId).
                Select(owner=> owner.OwnerName).
                FirstOrDefaultAsync();
        }

        public async Task<OwnerInfo> GetOwnerInfoByIdAsync(int ownerId)
        {
            var ownerInfo = await _context.OwnerInfos.AsNoTracking().Where(owner => owner.UserId == ownerId).FirstOrDefaultAsync();
            if (ownerInfo == null)
            {
                return null;
            }
            return ownerInfo;
        }

        public async Task<bool> UpdateOwnerInfoByIdAsync(int ownerId, OwnerInfo ownerInfo)
        {
            var existing = await _context.OwnerInfos
               .FirstOrDefaultAsync(o => o.UserId == ownerId);

            if (existing == null)
            {
                // INSERT
                ownerInfo.UserId = ownerId;
                await _context.OwnerInfos.AddAsync(ownerInfo);
            }
            else
            {
                // UPDATE (only relevant properties)
                existing.OwnerName = ownerInfo.OwnerName;
                existing.Logo = ownerInfo.Logo;
                existing.LogoContentType = ownerInfo.LogoContentType;

                _context.OwnerInfos.Update(existing); // Optional; EF tracks changes automatically
            }

            await _context.SaveChangesAsync();
            return true;

        }
    }
}
