using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Outputs;

public partial class VariableDigitalOutputViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private bool _value;

    [ObservableProperty]
    private string _name = "NewVariable";

    public Dictionary<string, double> Variables { get; set; } = null!;

    public VariableDigitalOutputViewModel(LogicCanvasViewModel canvasViewModel) : base(canvasViewModel)
    {
        Variables = canvasViewModel.Variables;
        Value = false;
        Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Digital));
    }

    public override void Process()
    {
        Value = Inputs[0].Value > 0;
        Variables[Name] = Value ? 1 : 0;
    }
}