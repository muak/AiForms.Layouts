using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;

namespace Sample.ViewModels
{
    public class TestPageViewModel : BindableBase
    {
        private INavigationService _navi;
        public TestPageViewModel(INavigationService navigationService)
        {
            _navi = navigationService;
        }

        private DelegateCommand _BackCommand;
        public DelegateCommand BackCommand {
            get { return _BackCommand = _BackCommand ?? new DelegateCommand(async() => {
                await _navi.GoBackAsync(new NavigationParameters { {"hoge",true } });

            }); }
        }
    }
}
