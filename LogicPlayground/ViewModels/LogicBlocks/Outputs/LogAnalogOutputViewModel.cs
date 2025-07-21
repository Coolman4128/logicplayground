using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Outputs;

public partial class LogAnalogOutputViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private double _value;

    public LogAnalogOutputViewModel()
    {
        Value = 0.0;
        Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Analog));
    }

    public override void Process()
    {
        Value = Inputs[0].Value;
    }
}