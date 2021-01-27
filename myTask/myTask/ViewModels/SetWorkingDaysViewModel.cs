using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using myTask.Helpers;
using myTask.Models;
using myTask.Services.Navigation;
using myTask.ViewModels.Base;
using myTask.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace myTask.ViewModels
{
    public class SetWorkingDaysViewModel : BaseViewModel
       {
           public override Type WiredPageType => typeof(InitWorkingDaysPage);

           public List<string> WorkingDaysStrings { get; set; } = new List<string>()
           {
               "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
           };
           
           public List<int> DaysIndices { get; set; }
           public ICommand ProceedCommand { get; set; }
           public ICommand SelectedIndicesChangedCommand { get; set; }
           public SetWorkingDaysViewModel(INavigationService navigationService) : base(navigationService)
           {
               ProceedCommand = new Command(OnProceed);
               SelectedIndicesChangedCommand = new Command<int[]>(SelectedIndicesChanges);
               DaysIndices = new List<int>();
           }

           public override async Task Init(object param)
           {
               await base.Init(param);
           }

           private void SelectedIndicesChanges(int[] indices)
           {
               DaysIndices.Clear();
               DaysIndices.AddRange(indices);
           }

           private async void OnProceed()
           {
               await _navigationService.NavigateToAsync<SetWorkingHoursViewModel>(DaysIndices.ToList());
               await _navigationService.ClearTheStackAsync();
           }
       }
    }