using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Inputs;

public partial class VariableDigitalInputViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private bool _value;

    [ObservableProperty]
    private string _variableName = "NewVariable";

    public Dictionary<string, double> Variables { get; } = null!;

    public VariableDigitalInputViewModel(LogicCanvasViewModel canvasViewModel) : base(canvasViewModel)
    {
        Variables = canvasViewModel.Variables;
        Value = false;
        Outputs.Add(new ConnectionPointOutputViewModel(Enums.ConnectionTypeEnum.Digital));
    }

    public override void Process()
    {
        if (!Variables.TryGetValue(VariableName, out var varValue))
        {
            // If the variable does not exist, create it with the current value
            Variables[VariableName] = Value ? 1 : 0;
            varValue = Value ? 1 : 0;
        }

        Value = varValue > 0;
        Outputs[0].Value = Value ? 1 : 0;
    }
}