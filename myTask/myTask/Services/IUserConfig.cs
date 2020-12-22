using System.Threading.Tasks;
using myTask.Models;

namespace myTask.Services
{
    public interface IUserConfig
    {
        Task<UserConfig> GetConfig();
        Task<bool> SetConfig();
    }
}