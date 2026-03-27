using UI.Core.Base;

namespace UI.Core.Navigation;

public class NavigationService
{
    private readonly Func<Type, ViewModelBase> _viewModelFactory;
    public Action<ViewModelBase>? OnNavigate;

    public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public void Navigate<TViewModel>() where TViewModel : ViewModelBase
    {
        var viewModel = _viewModelFactory(typeof(TViewModel));
        OnNavigate.Invoke(viewModel);
    }

    public void Navigate<TViewModel>(Action<TViewModel> configure) where TViewModel : ViewModelBase
    {
        var viewModel = (TViewModel)_viewModelFactory(typeof(TViewModel));
        configure(viewModel);
        OnNavigate!.Invoke(viewModel);
    }
}
