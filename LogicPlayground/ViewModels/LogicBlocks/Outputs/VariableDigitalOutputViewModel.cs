using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Outputs;

public partial class VariableDigitalOutputViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private bool _value;

    [ObservableProperty]
    private string _name = "NewVariable";

    public VariableDigitalOutputViewModel(LogicCanvasViewModel canvasViewModel) : base(canvasViewModel)
    {
        Value = false;
        Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Digital));
    }

    public override void Process()
    {
        Value = Inputs[0].Value > 0;
        CanvasViewModel.SetVarValue(Name, Value ? 1 : 0);
    }
}