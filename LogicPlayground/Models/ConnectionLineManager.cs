using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using LogicPlayground.ViewModels.LogicBlocks;

namespace LogicPlayground.Models;

public partial class ConnectionLineManager : ObservableObject
{
    public static ConnectionLineManager Instance { get; } = new ConnectionLineManager();

    [ObservableProperty]
    private ObservableCollection<ConnectionLine> _connectionLines = new();

    private ConnectionLineManager()
    {
        // Subscribe to changes in the connections collection
        ConnectionPointViewModel.Connections.CollectionChanged += OnConnectionsChanged;
    }

    private void OnConnectionsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems.Cast<(ConnectionPointInputViewModel, ConnectionPointOutputViewModel)>())
                    {
                        AddConnectionLine(item.Item1, item.Item2);
                    }
                }
                break;
                
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems.Cast<(ConnectionPointInputViewModel, ConnectionPointOutputViewModel)>())
                    {
                        RemoveConnectionLine(item.Item1, item.Item2);
                    }
                }
                break;
                
            case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                ConnectionLines.Clear();
                break;
        }
    }

    private void AddConnectionLine(ConnectionPointInputViewModel input, ConnectionPointOutputViewModel output)
    {
        // Check if line already exists
        if (ConnectionLines.Any(line => line.InputConnection == input && line.OutputConnection == output))
            return;

        var connectionLine = new ConnectionLine(input, output);
        ConnectionLines.Add(connectionLine);
        
        Console.WriteLine($"Added connection line between {input.Id} and {output.Id}");
    }

    private void RemoveConnectionLine(ConnectionPointInputViewModel input, ConnectionPointOutputViewModel output)
    {
        var lineToRemove = ConnectionLines.FirstOrDefault(line => 
            line.InputConnection == input && line.OutputConnection == output);
        
        if (lineToRemove != null)
        {
            ConnectionLines.Remove(lineToRemove);
            Console.WriteLine($"Removed connection line between {input.Id} and {output.Id}");
        }
    }

    public void UpdateAllLines()
    {
        foreach (var line in ConnectionLines)
        {
            line.UpdatePoints();
        }
    }

    public void UpdateLinesForBlock(LogicBlockViewModel block)
    {
        var linesToUpdate = ConnectionLines.Where(line =>
        {
            var inputParent = FindParentBlock(line.InputConnection);
            var outputParent = FindParentBlock(line.OutputConnection);
            return inputParent == block || outputParent == block;
        });

        foreach (var line in linesToUpdate)
        {
            line.UpdatePoints();
        }
    }

    private LogicBlockViewModel? FindParentBlock(ConnectionPointViewModel connectionPoint)
    {
        foreach (var block in LogicProcessor.Instance.Blocks)
        {
            if (connectionPoint is ConnectionPointInputViewModel input && block.Inputs.Contains(input))
                return block;
            if (connectionPoint is ConnectionPointOutputViewModel output && block.Outputs.Contains(output))
                return block;
        }
        return null;
    }

    public void SelectConnectionLine(ConnectionLine line)
    {
        // First, deselect all other lines
        foreach (var existingLine in ConnectionLines)
        {
            existingLine.IsSelected = false;
        }
        
        // Then select the specified line
        line.IsSelected = true;
    }

    public void DeselectAllConnectionLines()
    {
        foreach (var line in ConnectionLines)
        {
            line.IsSelected = false;
        }
    }

    public ConnectionLine? GetSelectedConnectionLine()
    {
        return ConnectionLines.FirstOrDefault(line => line.IsSelected);
    }

    public void DeleteSelectedConnectionLine()
    {
        var selectedLine = GetSelectedConnectionLine();
        if (selectedLine != null)
        {
            selectedLine.DeleteConnection();
        }
    }
}
