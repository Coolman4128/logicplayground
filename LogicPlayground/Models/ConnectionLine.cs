using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
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
        const double blockWidth = 200;
        const double blockHeight = 200;
        const double connectionPointSpacing = 24; // Approximate spacing between connection points
        
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
        const double minHorizontalDistance = 10;

        if (System.Math.Abs(deltaX) <= minHorizontalDistance && System.Math.Abs(deltaY) <= minHorizontalDistance)
        {
            // Simple direct connection for close points
            points.Add(end);
        }
        else
        {
            // Create path with horizontal segments and 45-degree angles
            // Ensure at least minHorizontalDistance on both sides
            var totalHorizontalDistance = System.Math.Abs(deltaX);
            var availableForMiddle = totalHorizontalDistance - (2 * minHorizontalDistance);
            
            double horizontalDistance;
            if (availableForMiddle > 0)
            {
                // We have enough space for minimum distances on both sides
                horizontalDistance = minHorizontalDistance + (availableForMiddle * 0.6);
            }
            else
            {
                // Not enough total distance, use half of available distance
                horizontalDistance = totalHorizontalDistance * 0.5;
            }
            
            // Move horizontally from start
            var midPoint1 = new Point(start.X + horizontalDistance, start.Y);
            points.Add(midPoint1);
            
            // Calculate remaining deltas
            var remainingDeltaX = end.X - midPoint1.X;
            var remainingDeltaY = end.Y - midPoint1.Y;
            
            if (System.Math.Abs(remainingDeltaY) > 0)
            {
                // Pattern: horizontal -> diagonal -> vertical -> diagonal -> horizontal
                
                // First diagonal segment
                var firstDiagonalDistance = System.Math.Min(System.Math.Abs(remainingDeltaX) * 0.3, System.Math.Abs(remainingDeltaY) * 0.3);
                firstDiagonalDistance = System.Math.Max(firstDiagonalDistance, 20); // Minimum diagonal length
                firstDiagonalDistance = System.Math.Min(firstDiagonalDistance, System.Math.Min(System.Math.Abs(remainingDeltaX), System.Math.Abs(remainingDeltaY)));
                
                var firstDiagonalDirection = new Point(
                    System.Math.Sign(remainingDeltaX) * firstDiagonalDistance,
                    System.Math.Sign(remainingDeltaY) * firstDiagonalDistance
                );
                
                var midPoint2 = new Point(midPoint1.X + firstDiagonalDirection.X, midPoint1.Y + firstDiagonalDirection.Y);
                points.Add(midPoint2);
                
                // Calculate remaining distances after first diagonal
                var remainingAfterFirstDiagonal = new Point(end.X - midPoint2.X, end.Y - midPoint2.Y);
                
                // Vertical segment (middle section)
                if (System.Math.Abs(remainingAfterFirstDiagonal.Y) > firstDiagonalDistance * 2)
                {
                    var verticalDistance = System.Math.Abs(remainingAfterFirstDiagonal.Y) - firstDiagonalDistance;
                    var midPoint3 = new Point(midPoint2.X, midPoint2.Y + System.Math.Sign(remainingAfterFirstDiagonal.Y) * verticalDistance);
                    points.Add(midPoint3);
                    
                    // Second diagonal segment
                    var secondDiagonalDistance = System.Math.Min(System.Math.Abs(end.X - midPoint3.X), System.Math.Abs(end.Y - midPoint3.Y));
                    var secondDiagonalDirection = new Point(
                        System.Math.Sign(end.X - midPoint3.X) * secondDiagonalDistance,
                        System.Math.Sign(end.Y - midPoint3.Y) * secondDiagonalDistance
                    );
                    
                    var midPoint4 = new Point(midPoint3.X + secondDiagonalDirection.X, midPoint3.Y + secondDiagonalDirection.Y);
                    points.Add(midPoint4);
                    
                    // Final horizontal segment if needed
                    if (System.Math.Abs(end.X - midPoint4.X) > 1)
                    {
                        points.Add(end);
                    }
                    else
                    {
                        points.Add(end);
                    }
                }
                else
                {
                    // Not enough vertical distance for the full pattern, use simpler routing
                    var finalDiagonalDistance = System.Math.Min(System.Math.Abs(remainingAfterFirstDiagonal.X), System.Math.Abs(remainingAfterFirstDiagonal.Y));
                    if (finalDiagonalDistance > 1)
                    {
                        var finalDiagonalDirection = new Point(
                            System.Math.Sign(remainingAfterFirstDiagonal.X) * finalDiagonalDistance,
                            System.Math.Sign(remainingAfterFirstDiagonal.Y) * finalDiagonalDistance
                        );
                        
                        var finalMidPoint = new Point(midPoint2.X + finalDiagonalDirection.X, midPoint2.Y + finalDiagonalDirection.Y);
                        points.Add(finalMidPoint);
                    }
                    
                    points.Add(end);
                }
            }
            else
            {
                // Only horizontal movement needed
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
