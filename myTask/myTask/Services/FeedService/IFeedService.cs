using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTask.Domain.Models;

namespace myTask.Services.FeedService
{
    public interface IFeedService
    {
        Task<UserUpdate> RegisterUpdate(string title, string message, Assignment assignment = null);
        Task<UserUpdate> DeleteUpdate(Guid id);

        Task<List<UserUpdate>> GetUpdatesByPage(int numberOfUpdates = 5, int pageNumber = 1);
    }
}