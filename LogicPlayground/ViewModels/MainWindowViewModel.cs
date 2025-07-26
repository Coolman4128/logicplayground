using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;

namespace LogicPlayground.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<NavigationItemViewModel> _navigationItems;

    [ObservableProperty]
    private NavigationItemViewModel? _selectedNavigationItem;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    [ObservableProperty]
    private bool _isSidebarCollapsed = false;

    public GridLength SidebarWidth => IsSidebarCollapsed ? new GridLength(50) : new GridLength(200);

    public MainWindowViewModel()
    {
        // Initialize navigation items
        var logicProgrammingViewModel = new LogicProgrammingViewModel();
        
        NavigationItems = new ObservableCollection<NavigationItemViewModel>
        {
            new NavigationItemViewModel("Logic Programming", "🔧", logicProgrammingViewModel),
            new NavigationItemViewModel("Circuit Design", "⚡", null), // Placeholder for future views
            new NavigationItemViewModel("Simulation", "🎮", null), // Placeholder for future views
            new NavigationItemViewModel("Settings", "⚙️", null) // Placeholder for future views
        };

        // Set Logic Programming as the default selected item
        SelectedNavigationItem = NavigationItems.First();
        CurrentViewModel = SelectedNavigationItem.ViewModel;
        SelectedNavigationItem.IsSelected = true;
    }

    partial void OnSelectedNavigationItemChanged(NavigationItemViewModel? value)
    {
        // Deselect all items
        foreach (var item in NavigationItems)
        {
            item.IsSelected = false;
        }

        // Select the current item and set the view model
        if (value != null)
        {
            value.IsSelected = true;
            CurrentViewModel = value.ViewModel;
        }
    }

    [RelayCommand]
    public void SelectNavigationItem(NavigationItemViewModel navigationItem)
    {
        SelectedNavigationItem = navigationItem;
    }

    [RelayCommand]
    public void ToggleSidebar()
    {
        IsSidebarCollapsed = !IsSidebarCollapsed;
        OnPropertyChanged(nameof(SidebarWidth));
    }

    partial void OnIsSidebarCollapsedChanged(bool value)
    {
        OnPropertyChanged(nameof(SidebarWidth));
    }
}
