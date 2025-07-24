using System;
using System.Collections;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using LogicPlayground.Models;

namespace LogicPlayground.Behaviors;

public static class ConnectionLineSelectionBehavior
{
    public static readonly AttachedProperty<bool> EnableSelectionProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, bool>("EnableSelection", typeof(ConnectionLineSelectionBehavior));

    public static bool GetEnableSelection(AvaloniaObject element) => element.GetValue(EnableSelectionProperty);
    public static void SetEnableSelection(AvaloniaObject element, bool value) => element.SetValue(EnableSelectionProperty, value);

    static ConnectionLineSelectionBehavior()
    {
        EnableSelectionProperty.Changed.AddClassHandler<Control>((control, e) =>
        {
            if (e.NewValue as bool? ?? false)
            {
                control.PointerPressed += OnPointerPressed;
                control.KeyDown += OnKeyDown;
                control.Focusable = true; // Allow the control to receive keyboard focus
            }
            else
            {
                control.PointerPressed -= OnPointerPressed;
                control.KeyDown -= OnKeyDown;
            }
        });
    }

    private static void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control control) return;

        var position = e.GetPosition(control);
        var hitConnectionLine = FindConnectionLineAtPoint(control, position);

        if (hitConnectionLine != null)
        {
            // Select the clicked connection line
            ConnectionLineManager.Instance.SelectConnectionLine(hitConnectionLine);
            e.Handled = true; // Only handle the event if we actually hit a connection line

            // Ensure the control has keyboard focus for key events
            control.Focus();
        }
        else
        {
            // Clicked on empty space, deselect all connection lines but don't handle the event
            // This allows the event to bubble up for other behaviors like drag
            ConnectionLineManager.Instance.DeselectAllConnectionLines();
            // Don't set e.Handled = true here - let other behaviors handle the event
        }
        
        e.Handled = true; // Prevent further processing of this event
    }

    private static void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete || e.Key == Key.Back)
        {
            ConnectionLineManager.Instance.DeleteSelectedConnectionLine();
            e.Handled = true;
        }
    }

    private static ConnectionLine? FindConnectionLineAtPoint(Control control, Point point)
    {
        // Find the connection lines ItemsControl in the visual tree
        var connectionLinesControl = control.GetVisualDescendants()
            .OfType<ItemsControl>()
            .Where(ic => ic.ItemsSource != null)
            .FirstOrDefault(ic => 
            {
                try 
                {
                    var items = ic.ItemsSource!.Cast<object>();
                    return items.Any() && items.First() is ConnectionLine;
                }
                catch
                {
                    return false;
                }
            });

        if (connectionLinesControl == null) return null;

        // Get the Canvas that contains the polylines
        var canvas = connectionLinesControl.GetVisualDescendants().OfType<Canvas>().FirstOrDefault();
        if (canvas == null) return null;

        // Transform the point to the canvas coordinate system
        var canvasTransform = canvas.TransformToVisual(control);
        if (canvasTransform == null) return null;

        Point canvasPoint;
        try
        {
            canvasPoint = canvasTransform.Value.Invert().Transform(point);
        }
        catch
        {
            return null;
        }

        // Find all Polyline elements in the canvas
        var polylines = canvas.GetVisualDescendants().OfType<Polyline>().ToList();

        foreach (var polyline in polylines)
        {
            if (polyline.DataContext is ConnectionLine connectionLine && IsPointNearPolyline(polyline, canvasPoint, canvas))
            {
                return connectionLine;
            }
        }

        return null;
    }

    private static bool IsPointNearPolyline(Polyline polyline, Point point, Control container)
    {
        // Transform the point to the polyline's coordinate system
        var transform = polyline.TransformToVisual(container);
        if (transform == null) return false;

        try
        {
            var transformedPoint = transform.Value.Invert().Transform(point);
            const double hitTolerance = 10.0; // Pixels

            // Check if point is near any line segment
            if (polyline.Points == null || polyline.Points.Count < 2) return false;

            for (int i = 0; i < polyline.Points.Count - 1; i++)
            {
                var start = polyline.Points[i];
                var end = polyline.Points[i + 1];

                var distance = DistancePointToLineSegment(transformedPoint, start, end);
                if (distance <= hitTolerance)
                {
                    return true;
                }
            }
        }
        catch
        {
            // Transformation failed, ignore this polyline
            return false;
        }

        return false;
    }

    private static double DistancePointToLineSegment(Point point, Point lineStart, Point lineEnd)
    {
        var dx = lineEnd.X - lineStart.X;
        var dy = lineEnd.Y - lineStart.Y;

        if (Math.Abs(dx) < 0.001 && Math.Abs(dy) < 0.001)
        {
            // Line segment is actually a point
            return Distance(point, lineStart);
        }

        var t = ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / (dx * dx + dy * dy);
        
        if (t < 0)
        {
            // Closest point is lineStart
            return Distance(point, lineStart);
        }
        else if (t > 1)
        {
            // Closest point is lineEnd
            return Distance(point, lineEnd);
        }
        else
        {
            // Closest point is on the line segment
            var closestPoint = new Point(lineStart.X + t * dx, lineStart.Y + t * dy);
            return Distance(point, closestPoint);
        }
    }

    private static double Distance(Point p1, Point p2)
    {
        var dx = p1.X - p2.X;
        var dy = p1.Y - p2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
