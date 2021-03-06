﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using XFCovidTrack.Interfaces;
using XFCovidTrack.ThemeResources;
using XFCovidTrack.Views;

namespace XFCovidTrack.ViewModels
{
   public class MainPageViewModel : BaseViewModel
    {
        public Command ChangeThemeAppCommand { get; }
        private readonly IRestService _service;
        public ICommand CommandAvoidCovid { get; }
        public ICommand SelectedCommand { get; }
        public MainPageViewModel(IRestService restService)
        {
            _service = restService;
            ChangeThemeAppCommand = new Command(ExecuteChangeThemeAppCommand);
            SelectedCommand = new Command(async()=> await ExecuteSelectedCommand());
            CommandAvoidCovid = new Command(CoronaVirusVoid);
        }
        public bool AppDarkTheme
        {
            get => Preferences.Get("appDarkTheme", false);
            set => Preferences.Set("appDarkTheme", value);
        }
        public async Task GetAll()
        {
            SetDarkTheme(AppDarkTheme);
        }
        public void SubscribeChangeLanguage()
        {
         
            MessagingCenter.Subscribe<TesteViewModel, bool>(this, "changeAppTheme", (s, param) =>
            {
                AppDarkTheme = param;
                SetDarkTheme(param);
            });
         
        }

        public void CoronaVirusVoid()
        {
            App.Current.MainPage.Navigation.PushAsync(new AvoidVirus());
        }

        public async Task ExecuteSelectedCommand()
        {
            if (CheckConnectivity())
                await Navigation.PushAsync(new ResultCases());
            else
                App.Current.MainPage.DisplayAlert("Warning", "Check your Connection", "OK").Wait();
        }
        public void UnsubscribeEvents()
        {
           
            MessagingCenter.Unsubscribe<TesteViewModel>(this, "changeAppTheme");
           
        }
        private void ExecuteChangeThemeAppCommand()
        {
            SetDarkTheme(true);
        }

      

        void SetDarkTheme(bool darkTheme)
        {
            if (Device.RuntimePlatform == Device.iOS)
                DependencyService.Get<IStatusBarStyle>().ChangeTextColor(darkTheme);

            if (darkTheme)
            {             
                Application.Current.Resources = new DarkTheme();
            }
            else
            {         

                if (Application.Current.Resources.Source != null &&
                    Application.Current.Resources.Source.OriginalString.ToLower().Contains("light"))
                    return;

                Application.Current.Resources = new LightTheme();
            }
        }

        public override void OnDisappearing()
        {
            UnsubscribeEvents();
        }
    }
}
