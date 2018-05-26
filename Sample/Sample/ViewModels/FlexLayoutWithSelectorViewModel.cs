using System;
using Prism.Navigation;
using Prism.Services;

namespace Sample.ViewModels
{
    public class FlexLayoutWithSelectorViewModel:RepeatableFlexPageViewModel
    {
        public FlexLayoutWithSelectorViewModel(INavigationService navigationService, IPageDialogService pageDlg)
            :base(navigationService,pageDlg)
        {
        }
    }
}
