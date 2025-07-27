using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contract
{
    public interface IBackupService
    {
        Task<dynamic> BackupUserDataAsync(int userId);

        Task RestoreUserDataAsync(int userId, string backupFilePath);
    }
}
