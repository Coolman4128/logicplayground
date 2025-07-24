using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using LogicPlayground.ViewModels.LogicBlocks;
using LogicPlayground.Factories;

namespace LogicPlayground.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    public LogicCanvasViewModel LogicCanvasViewModel { get; } = new LogicCanvasViewModel();

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

    [RelayCommand]
    public void AddLogicGateBlock()
    {
        LogicCanvasViewModel.AddLogicBlock("LogicGate");
    }

    [RelayCommand]
    public void AddLogDigitalOutputBlock()
    {
        LogicCanvasViewModel.AddLogicBlock("LogDigitalOutput");
    }

    [RelayCommand]
    public void AddConstDigitalInputBlock()
    {
        LogicCanvasViewModel.AddLogicBlock("ConstDigitalInput");
    }
}
