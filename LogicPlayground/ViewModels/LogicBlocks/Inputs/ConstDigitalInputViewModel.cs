using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Inputs;

public partial class ConstDigitalInputViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private bool _value;

    public ConstDigitalInputViewModel()
    {
        Value = false;
        Outputs.Add(new ConnectionPointOutputViewModel(Enums.ConnectionTypeEnum.Digital));
    }

    public override void Process()
    {
        Outputs[0].Value = Value ? 1 : 0;
    }
}