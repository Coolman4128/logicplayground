using System;
using CommunityToolkit.Mvvm.ComponentModel;
using LogicPlayground.Enums;

namespace LogicPlayground.ViewModels.LogicBlocks.Functions;

public partial class CompareFunctionViewModel : LogicBlockViewModel
{
    [ObservableProperty]
    private bool _result;

    [ObservableProperty]
    private CompareFunctionTypeEnum _functionType;

    public CompareFunctionTypeEnum[] FunctionTypeOptions { get; } = Enum.GetValues<CompareFunctionTypeEnum>();

    [ObservableProperty]
    private double _threshold = 0.0001; // Small threshold for floating-point comparison

    public CompareFunctionViewModel()
    {
        Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Analog));
        Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Analog));
        Outputs.Add(new ConnectionPointOutputViewModel(Enums.ConnectionTypeEnum.Digital));
    }

    public override void Process()
    {
        switch (FunctionType)
        {
            case CompareFunctionTypeEnum.Equal:
                Result = Math.Abs(Inputs[0].Value - Inputs[1].Value) < Threshold;
                break;
            case CompareFunctionTypeEnum.NotEqual:
                Result = Math.Abs(Inputs[0].Value - Inputs[1].Value) >= Threshold;
                break;
            case CompareFunctionTypeEnum.GreaterThan:
                Result = Inputs[0].Value > Inputs[1].Value + Threshold;
                break;
            case CompareFunctionTypeEnum.LessThan:
                Result = Inputs[0].Value < Inputs[1].Value - Threshold;
                break;
            case CompareFunctionTypeEnum.GreaterThanOrEqual:
                Result = Inputs[0].Value >= Inputs[1].Value - Threshold;
                break;
            case CompareFunctionTypeEnum.LessThanOrEqual:
                Result = Inputs[0].Value <= Inputs[1].Value + Threshold;
                break;
            default:
                throw new NotSupportedException($"Function type {FunctionType} is not supported.");
        }
        Outputs[0].Value = Result ? 1 : 0;
    }
}