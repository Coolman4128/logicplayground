namespace LogicPlayground.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    public LogicCanvasViewModel LogicCanvasViewModel { get; } = new LogicCanvasViewModel();
}
