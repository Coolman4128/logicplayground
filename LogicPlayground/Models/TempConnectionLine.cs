using System.Collections.ObjectModel;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using LogicPlayground.ViewModels.LogicBlocks;

namespace LogicPlayground.Models;

public partial class TempConnectionLine : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Point> _points = new();

    [ObservableProperty]
    private Point _startPoint;

    [ObservableProperty]
    private Point _endPoint;

    [ObservableProperty]
    private string _strokeColor = "#00ff00"; // Green color for temporary line

    [ObservableProperty]
    private double _strokeThickness = 2.0;

    [ObservableProperty]
    private bool _isVisible = false;

    public TempConnectionLine()
    {
        Points = new ObservableCollection<Point>();
    }

    public void UpdateLine(Point start, Point end)
    {
        StartPoint = start;
        EndPoint = end;
        
        Points.Clear();
        Points.Add(start);
        Points.Add(end);
        
        IsVisible = true;
    }

    public void Hide()
    {
        IsVisible = false;
        Points.Clear();
    }
}
