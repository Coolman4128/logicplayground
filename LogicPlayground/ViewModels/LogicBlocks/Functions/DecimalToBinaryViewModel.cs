using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels.LogicBlocks.Functions;

public partial class DecimalToBinaryViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private int _decimalValue;

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



    public DecimalToBinaryViewModel()
    {
        DecimalValue = 0;
        Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Analog));
        for (int i = 0; i < Bits; i++)
        {
            Outputs.Add(new ConnectionPointOutputViewModel(Enums.ConnectionTypeEnum.Analog));
        }
        
    }

    public override void Process()
    {
        DecimalValue = (int)Math.Floor(Inputs[0].Value);
        for (int i = 0; i < Bits; i++)
        {
            if (i < Outputs.Count)
            {
                Outputs[i].Value = (DecimalValue & (1 << i)) != 0 ? 1.0 : 0.0;
            }
        }
    }

    public void ChangeInputNumber(int outputs)
    {
        if (outputs < 1 || outputs > 64)
        {
            throw new ArgumentOutOfRangeException(nameof(outputs), "Number of outputs must be between 1 and 64.");
        }

        if (Outputs.Count > outputs)
        {
            for (int i = Outputs.Count - 1; i >= outputs; i--)
            {
                Outputs[i].DisconnectAll();
                Outputs.RemoveAt(i);
            }
        }
        else if (Outputs.Count == outputs)
        {
            // No change needed
        }
        else if (Outputs.Count < outputs)
        {
            for (int i = Outputs.Count; i < outputs; i++)
            {
                Outputs.Add(new ConnectionPointOutputViewModel(Enums.ConnectionTypeEnum.Digital));
            }
        }
    }
}