using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using LogicPlayground.ViewModels.LogicBlocks.Inputs;
using LogicPlayground.ViewModels.LogicBlocks.Outputs;
using Tmds.DBus.Protocol;

namespace LogicPlayground.ViewModels.LogicBlocks.Functions;

public partial class UserDefinedFunctionBlockViewModel : LogicBlockViewModel
{

    [ObservableProperty]
    private string _functionName = string.Empty;

    [ObservableProperty]
    private string _functionDescription = string.Empty;

    public ObservableCollection<LogicBlockViewModel> FunctionBlocks { get; set; } = new ObservableCollection<LogicBlockViewModel>();

    [ObservableProperty]
    private int _numberOfInputs;

    [ObservableProperty]
    private int _numberOfOutputs;

    public Dictionary<string, double> LogicSpaceVariables { get; set; } = new Dictionary<string, double>();

    public UserDefinedFunctionBlockViewModel(LogicCanvasViewModel canvasViewModel, UserDefinedFunctionViewModel userDefinedFunctionViewModel)
        : base(canvasViewModel)
    {
        LogicSpaceVariables = new Dictionary<string, double>(userDefinedFunctionViewModel.Variables);
        FunctionBlocks = new ObservableCollection<LogicBlockViewModel>(userDefinedFunctionViewModel.LogicBlocks);
        NumberOfInputs = userDefinedFunctionViewModel.InputCount;
        NumberOfOutputs = userDefinedFunctionViewModel.OutputCount;
        FunctionName = userDefinedFunctionViewModel.Name;
        FunctionDescription = userDefinedFunctionViewModel.Description;

        foreach (var block in FunctionBlocks)
        {
            if (block is VariableAnalogInputViewModel varai)
            {
                varai.Variables = LogicSpaceVariables;
            }
            if (block is VariableDigitalInputViewModel vardi)
            {
                vardi.Variables = LogicSpaceVariables;
            }
            if (block is VariableAnalogOutputViewModel varaout)
            {
                varaout.Variables = LogicSpaceVariables;
            }
            if (block is VariableDigitalOutputViewModel vardout)
            {
                vardout.Variables = LogicSpaceVariables;
            }

            foreach (var input in block.Inputs)
            {
                input.Id = System.Guid.CreateVersion7();
            }

            foreach (var output in block.Outputs)
            {
                output.Id = System.Guid.CreateVersion7();
            }
        }

        for (int i = 0; i < NumberOfInputs; i++)
        {
            Inputs.Add(new ConnectionPointInputViewModel(Enums.ConnectionTypeEnum.Analog));
        }
        
        for (int i = 0; i < NumberOfOutputs; i++)
        {
            Outputs.Add(new ConnectionPointOutputViewModel(Enums.ConnectionTypeEnum.Analog));
        }
    }

    public override void Process()
    {
        // The general idea is to do a single process of all the blocks inside the user function

        var sortedBlocks = FunctionBlocks.OrderBy(b => b.BlockPositionX).ToList();

        for (int i = 0; i < Inputs.Count; i++)
        {
            var input = Inputs[i];
            LogicSpaceVariables[$"Input_{i}"] = input.Value;
        }


        foreach (var block in sortedBlocks)
        {
            block.Process();
            ConnectionPointViewModel.PropagateConnections();
        }
        
        for (int i = 0; i < Outputs.Count; i++)
        {
            var output = LogicSpaceVariables[$"Output_{i}"];
            Outputs[i].Value = output;
        }

    }
}