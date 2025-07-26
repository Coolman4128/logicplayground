using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using LogicPlayground.ViewModels.LogicBlocks;
using LogicPlayground.Factories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LogicPlayground.Models;
using System.Linq;

namespace LogicPlayground.ViewModels;

public partial class LogicProgrammingViewModel : ViewModelBase
{
    [ObservableProperty]
    private LogicCanvasViewModel _logicCanvasViewModel = new LogicCanvasViewModel();

    public List<string> AvailableLogicBlocks => LogicBlockViewModel.BlockTypes;

    public ObservableCollection<UserDefinedFunctionViewModel> UserDefinedFunctions => UserFunctionManager.UserFunctions;

    [ObservableProperty]
    private string? _selectedLogicBlockType;

    [ObservableProperty]
    private UserDefinedFunctionViewModel? _currentUserFunction;

    [ObservableProperty]
    private UserDefinedFunctionViewModel? _selectedUserFunction;

    [RelayCommand]
    public void AddLogicBlock(string blockType)
    {
        if (!string.IsNullOrEmpty(blockType))
        {
            LogicCanvasViewModel.AddLogicBlock(blockType);
        }
    }

    [RelayCommand]
    public void AddUserDefinedFunctionBlock(string functionName)
    {
        if (!string.IsNullOrEmpty(functionName))
        {
            LogicCanvasViewModel.AddUserDefinedFunctionBlock(functionName);
        }
    }

    [RelayCommand]
    public void CreateNewUserFunction()
    {
        var newFunction = new UserDefinedFunctionViewModel
        {
            Name = $"Function_{UserDefinedFunctions.Count + 1}"
        };
        
        UserFunctionManager.AddUserFunction(newFunction);
        SwitchToUserFunction(newFunction);
    }

    [RelayCommand]
    public void SwitchToUserFunction(UserDefinedFunctionViewModel? userFunction)
    {
        if (userFunction != null)
        {
            CurrentUserFunction = userFunction;
            SelectedUserFunction = userFunction;
            LogicCanvasViewModel = new LogicCanvasViewModel(userFunction);
            OnPropertyChanged(nameof(IsInUserFunction));
        }
    }

    [RelayCommand]
    public void SwitchToMainCanvas()
    {
        CurrentUserFunction = null;
        SelectedUserFunction = null;
        LogicCanvasViewModel = new LogicCanvasViewModel();
        OnPropertyChanged(nameof(IsInUserFunction));
    }

    public bool IsInUserFunction => CurrentUserFunction != null;

    [ObservableProperty]
    private UserControl? _activeSettingsPanel;

    [ObservableProperty] 
    private LogicBlockViewModel? _activeSettingsViewModel;

    public void SetActiveSettingsPanel(LogicBlockViewModel viewModel)
    {
        ActiveSettingsViewModel = viewModel;
        ActiveSettingsPanel = SettingsPanelFactory.CreateSettingsView(viewModel);
    }

    [RelayCommand]
    public void CloseSettingsPanel()
    {
        ActiveSettingsPanel = null;
        ActiveSettingsViewModel = null;
    }
}
