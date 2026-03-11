using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Services;

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
        // Here you would typically set the current view model in your application
        // For example, if you have a MainWindow with a ContentControl, you would set its Content to the new view model
        // MainWindow.Content = viewModel;
        OnNavigate.Invoke(viewModel);
    
    }
}
