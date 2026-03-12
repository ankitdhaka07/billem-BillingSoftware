using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;
using UI.Features.Home;

namespace UI.Features.About;

public class AboutViewModel : ViewModelBase
{
    public ICommand BackCommand { get; }

    public AboutViewModel(NavigationService navigationService)
    {
        BackCommand = new RelayCommand(() => navigationService.Navigate<HomeViewModel>());
    }
}
