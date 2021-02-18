using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using myTask.Domain.Models;
using myTask.Services.FeedService;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Forms;

namespace myTask.ViewModels
{
    public class FeedViewModel : BaseViewModel
    {
        public override Type WiredPageType => typeof(FeedPage);

        private readonly IFeedService _feedService;
        private int _page = 1;
        private int _numberOfUpdates = 10;
        
        public ObservableCollection<UserUpdate> RecentUpdates { get; set; }
        public ICommand LoadMoreCommand { get; set; }

        public FeedViewModel(INavigationService navigationService, IFeedService feedService) : base(navigationService)
        {
            _feedService = feedService;
            RecentUpdates = new ObservableCollection<UserUpdate>();
            LoadMoreCommand = new Command(LoadMore);
            MessagingCenter.Subscribe<FeedService>(this, "New Update", (sender) => LoadNewUpdate());
        }

        private async void LoadMore()
        {
            _page++;
            var moreUpdates = await _feedService.GetUpdatesByPage(_numberOfUpdates, _page);
            if (RecentUpdates != null)
            {
                foreach (var update in moreUpdates)
                {
                    RecentUpdates.Add(update);
                }
            }
            else
            {
                RecentUpdates = new ObservableCollection<UserUpdate>(moreUpdates);
            }
        }

        private async void LoadNewUpdate()
        {
            var newUpdates = await _feedService.GetUpdatesByPage(1, 1);
            RecentUpdates.Insert(0, newUpdates.First());
        }

        public override async Task Init(object param)
        {
            var firstUpdates = await _feedService.GetUpdatesByPage(_numberOfUpdates, _page);
            RecentUpdates = new ObservableCollection<UserUpdate>(firstUpdates);
        }
    }
}