using System;
using System.Collections.ObjectModel;
using System.Drawing;
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
    private string _name = string.Empty;

    [ObservableProperty]
    private double _value;


    public string PointColor => _selected || _isHovered ? "#ff00ffff" : "#ffffffff";

    public int Size => _selected || _isHovered ?  18 : 15;
    private bool _selected = false;

    public bool Selected
    {
        get => _selected;
        set
        {
            SetProperty(ref _selected, value);
            OnPropertyChanged(nameof(PointColor));
            OnPropertyChanged(nameof(Size));
        }
    }

    private bool _isHovered = false;

    public bool IsHovered
    {
        get => _isHovered;
        set
        {
            SetProperty(ref _isHovered, value);
            OnPropertyChanged(nameof(PointColor));
            OnPropertyChanged(nameof(Size));
        }
    }


    public ConnectionTypeEnum ConnectionType { get; set; }

    public static ObservableCollection<(ConnectionPointInputViewModel, ConnectionPointOutputViewModel)> Connections { get; } = new();

    public ConnectionPointViewModel(ConnectionTypeEnum connectionType, string name = "conn")
    {
        Name = name;
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

    public void BeSelected()
    {
        Selected = true;
    }

    public void BeUnselected()
    {
        Selected = false;
    }

    public void BeHovered()
    {
        IsHovered = true;
    }

    public void BeUnhovered()
    {
        IsHovered = false;
    }

    public void AttemptConnection(ConnectionPointViewModel vm)
    {
        Console.WriteLine($"Attempting connection from {Id} to {vm.Id}");
        if (this is ConnectionPointInputViewModel input && vm is ConnectionPointOutputViewModel output)
        {
            Connect(input, output);
        }
        else if (this is ConnectionPointOutputViewModel outputVm && vm is ConnectionPointInputViewModel inputVm)
        {
            Connect(inputVm, outputVm);
        }
        else
        {
            // Points are not compatible, handle it however
        }
    }

    public static void PropagateConnections()
    {
        foreach (var connection in Connections)
        {
            var input = connection.Item1;
            var output = connection.Item2;

            if (input.ConnectionType != output.ConnectionType)
            {
                throw new InvalidOperationException("Cannot connect points of different types.");
            }

            input.Value = output.Value;
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

