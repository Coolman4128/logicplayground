using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using LogicPlayground.Enums;

namespace LogicPlayground.ViewModels.LogicBlocks;

public partial class ConnectionPointOutputViewModel : ConnectionPointViewModel
{
    // Additional properties or methods specific to output connection points can be added here.

    public ObservableCollection<ConnectionPointInputViewModel> ConnectedInputs { get; } = new();

    public ConnectionPointOutputViewModel(ConnectionTypeEnum connectionType) : base(connectionType)
    {
        // Initialize any specific properties for output connection points if needed.
    }

    public void Connect(ConnectionPointInputViewModel input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input), "Input connection point cannot be null.");
        }

        if (ConnectedInputs.Contains(input))
        {
            throw new InvalidOperationException("This input connection point is already connected.");
        }

        ConnectedInputs.Add(input);
        input.ConnectedOutput = this; // Set the output reference in the input
        ConnectionPointViewModel.Connections.Add((input, this)); // Add the connection to the static collection

    }

    public void Disconnect(ConnectionPointInputViewModel input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input), "Input connection point cannot be null.");
        }

        if (ConnectedInputs.Contains(input))
        {
            ConnectedInputs.Remove(input);
            input.ConnectedOutput = null; // Clear the connected output reference
            var connection = ConnectionPointViewModel.Connections.FirstOrDefault(c => c.Item1 == input && c.Item2 == this);
            if (connection != default)
            {
                ConnectionPointViewModel.Connections.Remove(connection);
            }
        }
    }

    public void Disconnect(Guid guid)
    {
        var input = ConnectedInputs.FirstOrDefault(i => i.Id == guid);
        if (input != null)
        {
            Disconnect(input);
        }
        else
        {
            throw new ArgumentException("No input connection point found with the specified GUID.", nameof(guid));
        }
    }
}