using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using LogicPlayground.ViewModels.LogicBlocks;
using LogicPlayground.Factories;
using System.Collections.Generic;

namespace LogicPlayground.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    public LogicCanvasViewModel LogicCanvasViewModel { get; } = new LogicCanvasViewModel();

    public List<string> AvailableLogicBlocks { get; } = new List<string>
    {
        "LogicGate",
        "LogDigitalOutput", 
        "ConstDigitalInput",
        "ConstAnalogInput",
        "LogAnalogOutput"
    };

    [ObservableProperty]
    private string? _selectedLogicBlockType;

    [RelayCommand]
    public void AddLogicBlock(string blockType)
    {
        if (!string.IsNullOrEmpty(blockType))
        {
            LogicCanvasViewModel.AddLogicBlock(blockType);
        }
    }

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
