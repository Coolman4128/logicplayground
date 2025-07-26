using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Inputs;

public partial class VariableAnalogInputViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private double _value;

    [ObservableProperty]
    private string _variableName = "NewVariable";

    public Dictionary<string, double> Variables { get; set; } = null!;

    public VariableAnalogInputViewModel(LogicCanvasViewModel canvasViewModel) : base(canvasViewModel)
    {
        Variables = canvasViewModel.Variables;
        Value = 0.0;
        Outputs.Add(new ConnectionPointOutputViewModel(Enums.ConnectionTypeEnum.Analog));
    }

    public override void Process()
    {
        if (!Variables.TryGetValue(VariableName, out var varValue))
        {
            // If the variable does not exist, create it with the current value
            Variables[VariableName] = Value;
            varValue = Value;
        }
        Value = varValue;
        Outputs[0].Value = Value;
    }
}