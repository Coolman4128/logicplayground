using System;
using CommunityToolkit.Mvvm.ComponentModel;
using LogicPlayground.Enums;

namespace LogicPlayground.ViewModels.LogicBlocks.Functions;

public partial class MathFunctionViewModel : LogicBlockViewModel
{

    [ObservableProperty]
    private double _result;

    [ObservableProperty]
    private MathFunctionTypeEnum _functionType;

    public MathFunctionTypeEnum[] FunctionTypeOptions { get; } = Enum.GetValues<MathFunctionTypeEnum>();

    public MathFunctionViewModel()
    {
        Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Analog));
        Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Analog));
        Outputs.Add(new ConnectionPointOutputViewModel(Enums.ConnectionTypeEnum.Analog));
    }



    // Example of a method that could be overridden
    public override void Process()
    {
        switch (FunctionType)
        {
            case MathFunctionTypeEnum.Add:
                Result = Inputs[0].Value + Inputs[1].Value;
                break;
            case MathFunctionTypeEnum.Subtract:
                Result = Inputs[0].Value - Inputs[1].Value;
                break;
            case MathFunctionTypeEnum.Multiply:
                Result = Inputs[0].Value * Inputs[1].Value;
                break;
            case MathFunctionTypeEnum.Divide:
                Result = Inputs[0].Value / Inputs[1].Value;
                break;
            case MathFunctionTypeEnum.Modulus:
                Result = Inputs[0].Value % Inputs[1].Value;
                break;
            case MathFunctionTypeEnum.Power:
                Result = Math.Pow(Inputs[0].Value, Inputs[1].Value);
                break;
            case MathFunctionTypeEnum.Root:
                Result = Math.Sqrt(Inputs[0].Value);
                break;
            case MathFunctionTypeEnum.Log:
                Result = Math.Log(Inputs[0].Value, Inputs[1].Value);
                break;
            default:
                throw new NotSupportedException($"Function type {FunctionType} is not supported.");
        }
        Outputs[0].Value = Result;
    }
}