using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Inputs;

public partial class ConstAnalogInputViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private double _value;

    public ConstAnalogInputViewModel()
    {
        Value = 0.0;
        Outputs.Add(new ConnectionPointOutputViewModel(Enums.ConnectionTypeEnum.Analog));
    }

    public override void Process()
    {
        Outputs[0].Value = Value;
    }
}