using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myTask.Domain.Models;
using myTask.Helpers;
using myTask.Services.Database.Repositories;
using Xamarin.Forms;

namespace myTask.Services.FeedService
{
    public class FeedService : IFeedService
    {

        private readonly IExtendedRepository<UserUpdate> _updateRepository;

        public FeedService(IExtendedRepository<UserUpdate> updateRepository)
        {
            _updateRepository = updateRepository;
        }
        
        public async Task<UserUpdate> RegisterUpdate(string title, string message, Assignment assignment = null)
        {
            var userUpdate = new UserUpdate()
            {
                Id = Guid.NewGuid(),
                DateTime = DateTime.Now,
                Title = title,
                Message = message
            };
            if (assignment != null)
            {
                userUpdate.Assignment = assignment;
            }
            var item = await _updateRepository.CreateItemAsync(userUpdate) == true ? userUpdate : null;
            MessagingCenter.Send(this, "New Update");
            return item;
        }

        public async Task<UserUpdate> DeleteUpdate(Guid id)
        {
            var update = await _updateRepository.GetItemByIdAsync(id);
            if (update != null)
            {
                await _updateRepository.DeleteItemAsync(update);
                return update;
            }
            
            return null;
        }

        public async Task<List<UserUpdate>> GetUpdatesByPage(int numberOfUpdates = 5, int pageNumber = 1)
        {
            var result = await _updateRepository.GetRecentItemsByPageAsync(numberOfUpdates, pageNumber);
            if (result != null)
            {
                return result.ToList();
            }
            return new List<UserUpdate>();
        }
    }
}