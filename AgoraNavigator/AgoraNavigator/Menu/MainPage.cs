﻿using System;
using Xamarin.Forms;
using AgoraNavigator.Schedule;
using AgoraNavigator.Tasks;
using Plugin.Permissions.Abstractions;
using AgoraNavigator.Login;
using AgoraNavigator.Contact;
using AgoraNavigator.Info;
using AgoraNavigator.GoogleMap;
using AgoraNavigator.Badge;
using AgoraNavigator.Downloads;

namespace AgoraNavigator.Menu
{
    public class MainPage : MasterDetailPage
    {  
        MasterPage masterPage;
        public static WelcomePage welcomePage;
        public static MapPage mapPage;
        public static SchedulePage schedulePage;
        public static TasksPage tasksPage;
        public static ContactPage contactPage;
        public static DownloadsPage downloadsPage;
        public static BonusInfoPage bonusInfoPage;
        public static BadgePage badgePage;

        public MainPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetTitleIcon(this, "Hamburger_Icon.png");
            BackgroundColor = Color.FromHex("061d3f");
            masterPage = new MasterPage();
            welcomePage = new WelcomePage();
            mapPage = new MapPage(50.0656911, 19.9083581);
            schedulePage = new SchedulePage();
            contactPage = new ContactPage();
            downloadsPage = new DownloadsPage();
            bonusInfoPage = new BonusInfoPage();
            
            masterPage.listView.ItemSelected += OnItemSelected;
            Master = masterPage;
            Detail = welcomePage;

            SchedulePage.scheduleDaysPage.FetchScheduleAsync();
            DownloadsPage.downloadsMasterPage.FetchDownloadFilesAsync();
        }

        public void SetStartedPage(string pageName)
        {
            switch(pageName)
            {
                case "WelcomePage":
                    Detail = welcomePage;
                    break;
                case "MapPage":
                    Detail = mapPage;
                    break;
                case "SchedulePage":
                    Detail = schedulePage;
                    break;
                case "GameLoginNavPage":
                    Detail = new GameLoginNavPage(typeof(WelcomePage));
                    break;
                case "ContactPage":
                    Detail = contactPage;
                    break;
                case "DownloadsPage":
                    Detail = downloadsPage;
                    break;
                case "BonusInfoPage":
                    Detail = bonusInfoPage;
                    break;
            }
        }

        public void ShowLoginScreen(Type navigateTo)
        {
            Detail = new GameLoginNavPage(navigateTo);
        }

        public void NavigateTo(Type navigateTo)
        {
            Detail = Activator.CreateInstance(navigateTo) as Page;
        }

        public void OpenMapAt(double latitude, double longitude)
        {
            Detail = new MapPage(latitude, longitude);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is MasterPageItem item)
            {
                switch (item.Title)
                {
                    case "Map":
                        GoogleMapPage.map.MyLocationEnabled = await Permissions.GetRuntimePermission(Permission.Location);
                        Detail = mapPage;
                        break;
                    case "Schedule":
                        Detail = schedulePage;
                        break;
                    case "Tasks":
                        if (Users.isUserLogged)
                        {
                            Detail = new TasksPage();
                        }
                        else
                        {
                            Detail = new GameLoginNavPage(typeof(TasksPage));
                        }
                        break;
                    case "Badge":
                        if (Users.isUserLogged)
                        {
                            Detail = new BadgePage();
                        }
                        else
                        {
                            Detail = new GameLoginNavPage(typeof(BadgePage));
                        }
                        break;
                    case "Contact":
                        Detail = contactPage;
                        break;
                    case "Downloads":
                        Detail = downloadsPage;
                        break;
                    case "Bonus info":
                        Detail = bonusInfoPage;
                        break;
                    default:
                        break;
                }
                masterPage.listView.SelectedItem = null;
                IsPresented = false;
            }
        }
    }
}
