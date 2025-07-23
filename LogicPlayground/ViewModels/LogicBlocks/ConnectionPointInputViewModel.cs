using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using LogicPlayground.Enums;

namespace LogicPlayground.ViewModels.LogicBlocks;

public partial class ConnectionPointInputViewModel : ConnectionPointViewModel
{
    [ObservableProperty]
    private ConnectionPointOutputViewModel? _connectedOutput;

    public ConnectionPointInputViewModel(ConnectionTypeEnum connectionType) : base(connectionType)
    {
        // Initialize any specific properties for input connection points if needed.
    }

    public void Connect(ConnectionPointOutputViewModel output)
    {
        if (output == null)
        {
            throw new ArgumentNullException(nameof(output), "Output connection point cannot be null.");
        }

        if (ConnectedOutput != null)
        {
            throw new InvalidOperationException("This input connection point is already connected to an output.");
        }

        ConnectedOutput = output; // Set the output reference in the input
        output.ConnectedInputs.Add(this); // Add this input to the output's connected inputs
        ConnectionPointViewModel.Connections.Add((this, output)); // Add the connection to the static collection
    }

    public void Disconnect()
    {
        Value = 0.0; // Reset value when disconnected
        if (ConnectedOutput != null)
        {
            ConnectedOutput.ConnectedInputs.Remove(this); // Remove this input from the output's connected inputs
            ConnectedOutput = null; // Clear the connected output reference
            var connection = ConnectionPointViewModel.Connections.FirstOrDefault(c => c.Item1 == this);
            if (connection != default)
            {
                ConnectionPointViewModel.Connections.Remove(connection);
            }
        }
    }
   
}