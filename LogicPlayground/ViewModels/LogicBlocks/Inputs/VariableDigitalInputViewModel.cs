using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Inputs;

public partial class VariableDigitalInputViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private bool _value;

    [ObservableProperty]
    private string _variableName = "NewVariable";

    public VariableDigitalInputViewModel(LogicCanvasViewModel canvasViewModel) : base(canvasViewModel)
    {
        Value = false;
        Outputs.Add(new ConnectionPointOutputViewModel(Enums.ConnectionTypeEnum.Digital));
    }

    public override void Process()
    {
        var varValue = CanvasViewModel.GetVarValue(VariableName);
        Value = varValue > 0;
        Outputs[0].Value = Value ? 1 : 0;
    }
}