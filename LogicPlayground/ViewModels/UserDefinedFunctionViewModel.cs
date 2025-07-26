using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LogicPlayground.ViewModels.LogicBlocks;

namespace LogicPlayground.ViewModels;

public partial class UserDefinedFunctionViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private Guid _id = Guid.NewGuid();

    [ObservableProperty]
    private string _description = string.Empty;

    public ObservableCollection<LogicBlockViewModel> LogicBlocks { get; } = new();

    public Dictionary<string, double> Variables { get; } = new();

    [ObservableProperty]
    private int _inputCount = 1;

    [ObservableProperty]
    private int _outputCount = 1;

    public UserDefinedFunctionViewModel()
    {
        for (int i = 0; i < InputCount; i++)
        {
            Variables[$"Input_{i + 1}"] = 0.0;
        }
        for (int i = 0; i < OutputCount; i++)
        {
            Variables[$"Output_{i + 1}"] = 0.0;
        }
    }
}