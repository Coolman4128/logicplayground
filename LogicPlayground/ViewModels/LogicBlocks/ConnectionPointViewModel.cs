using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using LogicPlayground.Enums;

namespace LogicPlayground.ViewModels.LogicBlocks;

public partial class ConnectionPointViewModel : ViewModelBase
{
    [ObservableProperty]
    private Guid _id = Guid.NewGuid();

    [ObservableProperty]
    private double _value;

    public ConnectionTypeEnum ConnectionType { get; set; }

    public static ObservableCollection<(ConnectionPointInputViewModel, ConnectionPointOutputViewModel)> Connections { get; } = new();

    public ConnectionPointViewModel(ConnectionTypeEnum connectionType)
    {
        ConnectionType = connectionType;
        _value = 0.0; // Initialize value to a default
    }

    public static void Connect(ConnectionPointInputViewModel input, ConnectionPointOutputViewModel output)
    {
        if (input == null || output == null)
        {
            throw new ArgumentNullException("Input and output connection points cannot be null.");
        }
        if (Connections.Any(c => c.Item1 == input && c.Item2 == output))
        {
            throw new InvalidOperationException("This connection already exists.");
        }
        input.Connect(output);
    }

    public static void Disconnect(ConnectionPointInputViewModel input, ConnectionPointOutputViewModel output)
    {
        if (input == null || output == null)
        {
            throw new ArgumentNullException("Input and output connection points cannot be null.");
        }
        var connection = Connections.FirstOrDefault(c => c.Item1 == input && c.Item2 == output);
        if (connection != default)
        {
            input.Disconnect();
        }
    }

    public bool GetValueAsBool()
    {
        if (Value != 0.0)
        {
            return true;
        }
        return false;
    }

    public double GetValueAsDouble()
    {
        return Value;
    }

    public object GetValue()
    {
        if (ConnectionType == ConnectionTypeEnum.Digital)
        {
            return GetValueAsBool();
        }
        else if (ConnectionType == ConnectionTypeEnum.Analog)
        {
            return GetValueAsDouble();
        }
        else
        {
            throw new InvalidOperationException("Unsupported connection type.");
        }
    }

   
}

