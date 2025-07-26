using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels;

public partial class NavigationItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _icon = string.Empty;

    [ObservableProperty]
    private ViewModelBase? _viewModel;

    [ObservableProperty]
    private bool _isSelected;

    public NavigationItemViewModel(string title, string icon, ViewModelBase? viewModel = null)
    {
        Title = title;
        Icon = icon;
        ViewModel = viewModel;
    }
}
