using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using LogicPlayground.Enums;

namespace LogicPlayground.ViewModels.LogicBlocks.Functions;



public partial class LogicGateFunctionViewModel : LogicBlockViewModel
{


    [ObservableProperty]
    private bool _output;

    public LogicGateTypeEnum[] GateTypeOptions { get; } = Enum.GetValues<LogicGateTypeEnum>();

   



    private int _numberOfInputs = 2;

    public int NumberOfInputs
    {
        get => _numberOfInputs;
        set
        {
            SetProperty(ref _numberOfInputs, value);
            ChangeInputNumber(value);
        }
    }

    [ObservableProperty]
    private LogicGateTypeEnum _gateType;

    public LogicGateFunctionViewModel()
    {
        NumberOfInputs = 2;
        GateType = LogicGateTypeEnum.NAND;
        Outputs.Add(new ConnectionPointOutputViewModel(Enums.ConnectionTypeEnum.Digital));
    }

    public override void Process()
    {
        switch (GateType)
        {
            case LogicGateTypeEnum.AND:
                Output = Inputs.All(input => input.Value > 0);
                break;
            case LogicGateTypeEnum.OR:
                Output = Inputs.Any(input => input.Value > 0);
                break;
            case LogicGateTypeEnum.NOT:
                ChangeInputNumber(1);
                Output = Inputs[0].Value == 0;
                break;
            case LogicGateTypeEnum.NAND:
                Output = !Inputs.All(input => input.Value > 0);
                break;
            case LogicGateTypeEnum.NOR:
                Output = !Inputs.Any(input => input.Value > 0);
                break;
            case LogicGateTypeEnum.XOR:
                Output = Inputs.Count(input => input.Value > 0) == 1;
                break;
            case LogicGateTypeEnum.XNOR:
                Output = !(Inputs.Count(input => input.Value > 0) == 1);
                break;
            default:
                Output = false;
                break;
        }
        Outputs[0].Value = Output ? 1 : 0;
    }

    public void ChangeInputNumber(int inputs)
    {
        if (inputs < 1 || inputs > 16)
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