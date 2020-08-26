﻿using System;
using System.Threading.Tasks;
using myTask.Services.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using myTask.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace myTask
{
    public partial class App : Application
    {

        private INavigationService _navigationService;
        public App()
        {
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);
            
        }

        protected override async void OnStart()
        {
            // Handle when your app starts
            await InitNavigation();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private async Task InitNavigation()
        {
            _navigationService = SuperContainer.Resolve<INavigationService>();
            await _navigationService.InitMainNavigation();
        }
    }
}