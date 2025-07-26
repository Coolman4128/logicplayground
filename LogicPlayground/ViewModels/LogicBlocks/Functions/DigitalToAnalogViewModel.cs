using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Functions;

public partial class DigitalToAnalogViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private double _outputValue;

    [ObservableProperty]
    private double _truthValue = 5.0; // Example truth value for digital to analog conversion

    [ObservableProperty]
    private double _falseValue = 0.0; // Example false value for digital to analog conversion

    public DigitalToAnalogViewModel(LogicCanvasViewModel canvas) : base(canvas)

    {
        Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Digital));
        Outputs.Add(new ConnectionPointOutputViewModel(Enums.ConnectionTypeEnum.Analog));
    }

    public override void Process()
    {
        // Assuming the input is a digital value (0 or 1)
        OutputValue = Inputs.Count > 0 && Inputs[0].Value > 0 ? TruthValue : FalseValue;
        
        // Update the output connection point
        if (Outputs.Count > 0)
        {
            Outputs[0].Value = OutputValue;
        }
    }
}