using CommunityToolkit.Mvvm.Input;

namespace LogicPlayground.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    public LogicCanvasViewModel LogicCanvasViewModel { get; } = new LogicCanvasViewModel();

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
    public void AddConstAnalogInputBlock()
    {
        LogicCanvasViewModel.AddLogicBlock("ConstAnalogInput");
    }
}
