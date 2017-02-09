using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Sample.Resources;
using Xamarin.Forms;
using System.Net.Http;
using Prism.Services;
using System.IO;

namespace Sample.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigationAware
    {

        private IPageDialogService _pageDialog;
        private INavigationService _navi;
        public MainPageViewModel(INavigationService navigationService,IPageDialogService pageDlg)
        {
            _navi = navigationService;
            _pageDialog = pageDlg;
        }


        public void OnNavigatedFrom(NavigationParameters parameters){

        }

        public void OnNavigatedTo(NavigationParameters parameters){

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }
    }
}

