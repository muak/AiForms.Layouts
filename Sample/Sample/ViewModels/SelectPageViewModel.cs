using System;
using Prism.Navigation;
using Reactive.Bindings;
namespace Sample.ViewModels
{
    public class SelectPageViewModel
    {
        public ReactiveCommand<string> GoToPage {get;} = new ReactiveCommand<string>();

        public SelectPageViewModel(INavigationService navigationService)
        {
            GoToPage.Subscribe(async p=>{
                await navigationService.NavigateAsync(p);
            });
        }
    }
}
