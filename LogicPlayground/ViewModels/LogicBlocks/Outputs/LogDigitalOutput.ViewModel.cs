using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Outputs;


public partial class LogDigitalOutputViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private bool _value;

    public LogDigitalOutputViewModel()
    {
        Value = false;
        Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Digital));
    }

    public override void Process()
    {
        Value = Inputs[0].Value > 0;
    }
}