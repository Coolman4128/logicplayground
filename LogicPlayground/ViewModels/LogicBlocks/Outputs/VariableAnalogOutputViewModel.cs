using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Outputs;

public partial class VariableAnalogOutputViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private double _value;

    [ObservableProperty]
    private string _name = "NewVariable";

    public Dictionary<string, double> Variables { get; set; } = null!;

    public VariableAnalogOutputViewModel(LogicCanvasViewModel canvasViewModel) : base(canvasViewModel)
    {
        Variables = canvasViewModel.Variables;
        Value = 0.0;
        Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Analog));
    }

    public override void Process()
    {
        
        Value = Inputs[0].Value;
        Variables[Name] = Value;
    }
}