using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Inputs;

public partial class VariableAnalogInputViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private double _value;

    [ObservableProperty]
    private string _variableName = "NewVariable";

    public VariableAnalogInputViewModel(LogicCanvasViewModel canvasViewModel) : base(canvasViewModel)
    {
        Value = 0.0;
        Outputs.Add(new ConnectionPointOutputViewModel(Enums.ConnectionTypeEnum.Analog));
    }

    public override void Process()
    {
        var varValue = CanvasViewModel.GetVarValue(VariableName);
        Value = varValue;
        Outputs[0].Value = Value;
    }
}