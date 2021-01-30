using System.Threading.Tasks;
using myTask.Domain.Models;

namespace myTask.Services.UserConfigManager
{
    public interface IUserConfigManager
    {
        Task<UserConfig> GetConfigAsync();
        Task<bool> SetConfigAsync(UserConfig config);
    }
}