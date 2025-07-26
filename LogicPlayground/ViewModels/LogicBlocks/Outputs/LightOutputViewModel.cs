using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Outputs;

public partial class LightOutputViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private bool _value;

    [ObservableProperty]
    private Color _offColor = Colors.Red;

    [ObservableProperty]
    private Color _onColor = Colors.Green;

    public Color CurrentLightColor => Value ? OnColor : OffColor;

    public LightOutputViewModel()
    {
        Value = false;
        Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Digital));
    }

    public override void Process()
    {
        Value = Inputs[0].Value > 0;
        OnPropertyChanged(nameof(CurrentLightColor));
    }

    partial void OnValueChanged(bool value)
    {
        OnPropertyChanged(nameof(CurrentLightColor));
    }

    partial void OnOffColorChanged(Color value)
    {
        OnPropertyChanged(nameof(CurrentLightColor));
    }

    partial void OnOnColorChanged(Color value)
    {
        OnPropertyChanged(nameof(CurrentLightColor));
    }
}
