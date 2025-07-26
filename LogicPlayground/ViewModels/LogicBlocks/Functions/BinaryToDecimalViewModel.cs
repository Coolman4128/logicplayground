using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Functions;

public partial class BinaryToDecimalViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private double _outputValue = 0.0;

    private int _bits = 8;

    public int Bits
    {
        get => _bits;
        set
        {
            SetProperty(ref _bits, value);
            ChangeInputNumber(value);
        }
    }

    public BinaryToDecimalViewModel(LogicCanvasViewModel canvas) : base(canvas)
    {
        for (int i = 0; i < Bits; i++)
        {
            Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Digital));
        }
        Outputs.Add(new ConnectionPointOutputViewModel(Enums.ConnectionTypeEnum.Analog));
    }

    public override void Process()
    {
        double value = 0.0;
        for (int i = 0; i < Bits; i++)
        {
            if (Inputs[i].Value > 0)
            {
                value += Math.Pow(2, i);
            }
        }
        OutputValue = value;
        Outputs[0].Value = OutputValue;
    }

    public void ChangeInputNumber(int inputs)
    {
        if (inputs < 1 || inputs > 64)
        {
            throw new ArgumentOutOfRangeException(nameof(inputs), "Number of inputs must be between 1 and 16.");
        }

        if (Inputs.Count > inputs)
        {
            for (int i = Inputs.Count - 1; i >= inputs; i--)
            {
                Inputs[i].Disconnect();
                Inputs.RemoveAt(i);
            }
        }
        else if (Inputs.Count == inputs)
        {
            // No change needed
        }
        else if (Inputs.Count < inputs)
        {
            for (int i = Inputs.Count; i < inputs; i++)
            {
                Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Digital));
            }
        }
    }
}