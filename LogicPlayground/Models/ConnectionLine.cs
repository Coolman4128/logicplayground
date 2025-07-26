using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using LogicPlayground.ViewModels.LogicBlocks;

namespace LogicPlayground.Models;

public partial class ConnectionLine : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Point> _points = new();

    [ObservableProperty]
    private ConnectionPointInputViewModel _inputConnection;

    [ObservableProperty]
    private ConnectionPointOutputViewModel _outputConnection;

    [ObservableProperty]
    private string _strokeColor = "#ffffff";

    [ObservableProperty]
    private double _strokeThickness = 2.0;

    [ObservableProperty]
    private bool _isSelected = false;

    partial void OnIsSelectedChanged(bool value)
    {
        StrokeColor = value ? "#ffff00" : "#ffffff"; // Yellow when selected, white when not
        StrokeThickness = value ? 3.0 : 2.0; // Thicker when selected
    }

    public ConnectionLine(ConnectionPointInputViewModel input, ConnectionPointOutputViewModel output)
    {
        InputConnection = input;
        OutputConnection = output;
        UpdatePoints();
    }

    public void UpdatePoints()
    {
        var startPos = GetConnectionPointPosition(OutputConnection);
        var endPos = GetConnectionPointPosition(InputConnection);
        
        var pointsList = CalculatePolylinePoints(startPos, endPos);
        Points.Clear();
        foreach (var point in pointsList)
        {
            Points.Add(point);
        }
    }

    private Point GetConnectionPointPosition(ConnectionPointViewModel connectionPoint)
    {
        // Find the parent block and calculate the absolute position of the connection point
        var parentBlock = FindParentBlock(connectionPoint);
        if (parentBlock == null)
            return new Point(0, 0);

        // Calculate relative position within the block based on connection point type and index
        var relativePos = GetRelativeConnectionPointPosition(connectionPoint, parentBlock);
        
        return new Point(
            parentBlock.BlockPositionX + relativePos.X,
            parentBlock.BlockPositionY + relativePos.Y
        );
    }

    private LogicBlockViewModel? FindParentBlock(ConnectionPointViewModel connectionPoint)
    {
        // Search through all blocks to find the one that contains this connection point
        foreach (var block in LogicProcessor.Instance.Blocks)
        {
            if (connectionPoint is ConnectionPointInputViewModel input && block.Inputs.Contains(input))
                return block;
            if (connectionPoint is ConnectionPointOutputViewModel output && block.Outputs.Contains(output))
                return block;
        }
        return null;
    }

    private Point GetRelativeConnectionPointPosition(ConnectionPointViewModel connectionPoint, LogicBlockViewModel parentBlock)
    {
        var blockWidth = parentBlock.BlockWidth;
        var blockHeight = parentBlock.BlockHeight;
        const double connectionPointSpacing = 18.7; // Approximate spacing between connection points
        
        if (connectionPoint is ConnectionPointInputViewModel input)
        {
            // Inputs are on the left side
            var index = parentBlock.Inputs.IndexOf(input);
            var totalInputs = parentBlock.Inputs.Count;
            var verticalOffset = (blockHeight / 2) + (index - (totalInputs - 1) / 2.0) * connectionPointSpacing;
            return new Point(0, verticalOffset);
        }
        else if (connectionPoint is ConnectionPointOutputViewModel output)
        {
            // Outputs are on the right side
            var index = parentBlock.Outputs.IndexOf(output);
            var totalOutputs = parentBlock.Outputs.Count;
            var verticalOffset = (blockHeight / 2) + (index - (totalOutputs - 1) / 2.0) * connectionPointSpacing;
            return new Point(blockWidth, verticalOffset);
        }
        
        return new Point(0, 0);
    }

    private List<Point> CalculatePolylinePoints(Point start, Point end)
    {
        var points = new List<Point>();
        points.Add(start);

        // Calculate intermediate points using only 45-degree angles
        var deltaX = end.X - start.X;
        var deltaY = end.Y - start.Y;

        // Minimum distance to move horizontally before turning
        const double minHorizontalDistance = 5;

        if (Math.Abs(deltaX) < minHorizontalDistance * 2)
        {
            // If the horizontal distance is too small, move vertically
            points.Add(end);
        }
        else
        {
            var horizontalDistance = Math.Abs(deltaX) - (minHorizontalDistance * 2);
            if (Math.Abs(deltaY) <= Math.Abs(horizontalDistance))
            {
                var distanceToGo = Math.Sign(deltaX) * (Math.Abs(deltaX) - Math.Abs(deltaY));
                points.Add(new Point(start.X  + (Math.Sign(deltaX) * minHorizontalDistance) + (distanceToGo / 2), start.Y));
                points.Add(new Point(start.X + (Math.Sign(deltaX) * minHorizontalDistance) + (distanceToGo / 2) + (Math.Sign(deltaX) * Math.Abs(deltaY)), start.Y + deltaY));
                points.Add(end);
            }
            else
            {
                var horizontalSplit = horizontalDistance / 2;
                var verticalDistance = Math.Sign(deltaY) * (Math.Abs(deltaY) - horizontalSplit);
                points.Add(new Point(start.X + Math.Sign(horizontalSplit) * (Math.Abs(horizontalSplit / 2) + minHorizontalDistance), start.Y));
                points.Add(new Point(start.X + Math.Sign(horizontalSplit) * (Math.Abs(horizontalSplit) + minHorizontalDistance), start.Y + Math.Sign(verticalDistance) * Math.Abs(horizontalSplit / 2)));
                points.Add(new Point(start.X + Math.Sign(horizontalSplit) * (Math.Abs(horizontalSplit) + minHorizontalDistance), start.Y + (Math.Sign(verticalDistance) * Math.Abs(horizontalSplit / 2)) + verticalDistance));
                points.Add(new Point(start.X + Math.Sign(horizontalSplit) * (Math.Abs(3 * horizontalSplit / 2) + minHorizontalDistance), start.Y + (Math.Sign(verticalDistance) * Math.Abs(horizontalSplit)) + verticalDistance));
                points.Add(end);

            }
            
        }

        return points;
    }

    public void DeleteConnection()
    {
        // Remove the connection from the connection points
        if (InputConnection != null && OutputConnection != null)
        {
            // Find and remove this connection from the connections collection
            var connectionToRemove = ConnectionPointViewModel.Connections
                .FirstOrDefault(c => c.Item1 == InputConnection && c.Item2 == OutputConnection);
            
            if (connectionToRemove != default)
            {
                ConnectionPointViewModel.Disconnect(InputConnection, OutputConnection);
            }
        }
    }
}
