
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using LogicPlayground.ViewModels.LogicBlocks;
using LogicPlayground.ViewModels;
using LogicPlayground.Models;

namespace LogicPlayground.Behaviors;

public static class ConnecterBehavior
{
    public static readonly AttachedProperty<bool> EnableConnectProperty =
            AvaloniaProperty.RegisterAttached<AvaloniaObject, bool>("EnableConnect", typeof(ConnecterBehavior));

    public static bool GetEnableConnect(AvaloniaObject element) => element.GetValue(EnableConnectProperty);
    public static void SetEnableConnect(AvaloniaObject element, bool value) => element.SetValue(EnableConnectProperty, value);


    // Store the currently pressed connection point
    private static ConnectionPointViewModel? _pressedConnectionPoint = null;
    // Store the currently hovered connection point during drag
    private static ConnectionPointViewModel? _hoveredConnectionPoint = null;
    // Store the root/top-level for PointerMoved subscription
    private static TopLevel? _subscribedRoot = null;

    static ConnecterBehavior()
    {
        EnableConnectProperty.Changed.AddClassHandler<Control>((control, e) =>
        {
            if (e.NewValue as bool? ?? false)
            {
                control.PointerEntered += OnPointerEnter;
                control.PointerExited += OnPointerLeave;
                control.PointerPressed += OnPointerPressed;
                control.PointerReleased += OnPointerReleased;
            }
            else
            {
                control.PointerEntered -= OnPointerEnter;
                control.PointerExited -= OnPointerLeave;
                control.PointerPressed -= OnPointerPressed;
                control.PointerReleased -= OnPointerReleased;
            }
        });
    }

    private static void OnPointerEnter(object? sender, PointerEventArgs e)
    {
        if (sender is Control control && control.DataContext is ViewModels.LogicBlocks.ConnectionPointViewModel vm)
        {
            vm.BeHovered();
        }
    }

    private static void OnPointerLeave(object? sender, PointerEventArgs e)
    {
        if (sender is Control control && control.DataContext is ViewModels.LogicBlocks.ConnectionPointViewModel vm)
        {
            vm.BeUnhovered();
        }
    }

    private static void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control && control.DataContext is ConnectionPointViewModel vm)
        {
            Console.WriteLine($"PointerPressed on ConnectionPoint: {vm.Id}");
            _pressedConnectionPoint = vm;
            vm.BeSelected();
            // Capture the pointer so we can track release anywhere
            e.Pointer.Capture(control);

            // Subscribe to PointerMoved on the root/top-level
            var root = TopLevel.GetTopLevel(control);
            if (root != null)
            {
                root.PointerMoved += OnRootPointerMoved;
                _subscribedRoot = root;
                
                // Start temporary connection line
                StartTempConnection(control, e.GetPosition(root));
            }

            e.Handled = true; // Prevent event bubbling to parent drag handler
        }
    }

    private static void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is not Control control)
            return;

        // Hide temporary connection line
        HideTempConnection();

        // Unsubscribe from PointerMoved
        if (_subscribedRoot != null)
        {
            _subscribedRoot.PointerMoved -= OnRootPointerMoved;
            _subscribedRoot = null;
        }

        // Hit test at the pointer position to find the ConnectionPointViewModel under the pointer
        var root = TopLevel.GetTopLevel(control);
        if (root == null)
            return;

        var pointerPos = e.GetPosition(root);
        ConnectionPointViewModel? releasedVm = null;
        var hit = root.InputHitTest(pointerPos) as Control;
        while (hit != null)
        {
            if (hit.DataContext is ConnectionPointViewModel vm)
            {
                releasedVm = vm;
                break;
            }
            hit = hit.Parent as Control;
        }

        if (releasedVm != null)
        {
            Console.WriteLine($"PointerReleased on ConnectionPoint: {releasedVm.Id}");
            if (_pressedConnectionPoint != null && !ReferenceEquals(_pressedConnectionPoint, releasedVm))
            {
                // Try to connect the two points
                try
                {
                    _pressedConnectionPoint!.AttemptConnection(releasedVm);
                    Console.WriteLine("Connection successful - connection lines will be updated automatically");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection failed: {ex.Message}");
                }
            }
        }
        if (_pressedConnectionPoint != null)
        {
            _pressedConnectionPoint!.BeUnselected();
        }
        // Unhover any hovered connection point
        if (_hoveredConnectionPoint != null)
        {
            _hoveredConnectionPoint.BeUnhovered();
            _hoveredConnectionPoint = null;
        }
        _pressedConnectionPoint = null;
        // Release pointer capture
        e.Pointer.Capture(null);
        e.Handled = true; // Prevent event bubbling to parent drag handler
    }

    // Manual hover tracking during drag
    private static void OnRootPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_pressedConnectionPoint == null || sender is not TopLevel root)
            return;

        var pointerPos = e.GetPosition(root);
        
        // Update temporary connection line
        UpdateTempConnection(root, pointerPos);
        
        ConnectionPointViewModel? hoveredVm = null;
        var hit = root.InputHitTest(pointerPos) as Control;
        while (hit != null)
        {
            if (hit.DataContext is ConnectionPointViewModel vm)
            {
                hoveredVm = vm;
                break;
            }
            hit = hit.Parent as Control;
        }

        if (!ReferenceEquals(_hoveredConnectionPoint, hoveredVm))
        {
            if (_hoveredConnectionPoint != null)
                _hoveredConnectionPoint.BeUnhovered();
            if (hoveredVm != null && !ReferenceEquals(_pressedConnectionPoint, hoveredVm))
                hoveredVm.BeHovered();
            _hoveredConnectionPoint = hoveredVm;
        }
    }

    private static void StartTempConnection(Control connectionPointControl, Point mousePosition)
    {
        var canvasViewModel = FindLogicCanvasViewModel(connectionPointControl);
        if (canvasViewModel == null || _pressedConnectionPoint == null)
            return;

        var startPos = GetConnectionPointCanvasPosition(_pressedConnectionPoint, connectionPointControl, canvasViewModel);
        var mouseCanvasPos = ConvertToCanvasPosition(mousePosition, canvasViewModel);
        canvasViewModel.TempConnectionLine.UpdateLine(startPos, mouseCanvasPos);
    }

    private static void UpdateTempConnection(TopLevel root, Point mousePosition)
    {
        var canvasViewModel = FindLogicCanvasViewModelFromRoot(root);
        if (canvasViewModel == null || _pressedConnectionPoint == null)
            return;

        var connectionPointControl = FindConnectionPointControl(root, _pressedConnectionPoint);
        if (connectionPointControl == null)
            return;

        var startPos = GetConnectionPointCanvasPosition(_pressedConnectionPoint, connectionPointControl, canvasViewModel);
        var mouseCanvasPos = ConvertToCanvasPosition(mousePosition, canvasViewModel);
        canvasViewModel.TempConnectionLine.UpdateLine(startPos, mouseCanvasPos);
    }

    private static void HideTempConnection()
    {
        if (_subscribedRoot != null)
        {
            var canvasViewModel = FindLogicCanvasViewModelFromRoot(_subscribedRoot);
            canvasViewModel?.TempConnectionLine.Hide();
        }
    }

    private static LogicCanvasViewModel? FindLogicCanvasViewModel(Control control)
    {
        var current = control.Parent;
        while (current != null)
        {
            if (current.DataContext is LogicCanvasViewModel canvasVm)
                return canvasVm;
            current = current.Parent;
        }
        return null;
    }

    private static LogicCanvasViewModel? FindLogicCanvasViewModelFromRoot(TopLevel root)
    {
        // Look for the LogicCanvas control in the visual tree
        return FindLogicCanvasInChildren(root);
    }

    private static LogicCanvasViewModel? FindLogicCanvasInChildren(Visual visual)
    {
        if (visual is Control control && control.DataContext is LogicCanvasViewModel canvasVm)
            return canvasVm;

        foreach (var child in visual.GetVisualChildren())
        {
            var result = FindLogicCanvasInChildren(child);
            if (result != null)
                return result;
        }
        return null;
    }

    private static Point GetConnectionPointWorldPosition(ConnectionPointViewModel connectionPoint, Control connectionPointControl)
    {
        // Get the center position of the connection point control
        var bounds = connectionPointControl.Bounds;
        var centerPoint = new Point(bounds.Width / 2, bounds.Height / 2);
        
        // Transform to world coordinates
        var topLevel = TopLevel.GetTopLevel(connectionPointControl);
        if (topLevel != null)
        {
            var worldPoint = connectionPointControl.TransformToVisual(topLevel)?.Transform(centerPoint) ?? centerPoint;
            return worldPoint;
        }
        
        return centerPoint;
    }

    private static Control? FindConnectionPointControl(TopLevel root, ConnectionPointViewModel connectionPoint)
    {
        return FindControlInChildren(root, connectionPoint);
    }

    private static Control? FindControlInChildren(Visual visual, ConnectionPointViewModel targetVm)
    {
        if (visual is Control control && ReferenceEquals(control.DataContext, targetVm))
            return control;

        foreach (var child in visual.GetVisualChildren())
        {
            var result = FindControlInChildren(child, targetVm);
            if (result != null)
                return result;
        }
        return null;
    }

    private static Point GetConnectionPointCanvasPosition(ConnectionPointViewModel connectionPoint, Control connectionPointControl, LogicCanvasViewModel canvasViewModel)
    {
        // Get the center position of the connection point control in world coordinates
        var worldPos = GetConnectionPointWorldPosition(connectionPoint, connectionPointControl);
        
        // Convert world position to canvas position by accounting for camera transforms
        return ConvertToCanvasPosition(worldPos, canvasViewModel);
    }

    private static Point ConvertToCanvasPosition(Point worldPosition, LogicCanvasViewModel canvasViewModel)
    {
        // Convert from world coordinates to canvas coordinates
        // This reverses the transforms applied to the canvas (zoom and pan)
        // Add 50 pixel offset adjustment to fix the positioning issue
        var canvasX = (worldPosition.X - canvasViewModel.CameraOffsetX) / canvasViewModel.ZoomLevel;
        var canvasY = (worldPosition.Y - canvasViewModel.CameraOffsetY - 50) / canvasViewModel.ZoomLevel; // Subtract 50 to fix offset
        
        return new Point(canvasX, canvasY);
    }

    private static Canvas? FindCanvasControl(Control control)
    {
        var current = control.Parent;
        while (current != null)
        {
            if (current is Canvas canvas)
                return canvas;
            current = current.Parent;
        }
        return null;
    }

    private static Canvas? FindCanvasControlFromRoot(TopLevel root)
    {
        return FindCanvasInChildren(root);
    }

    private static Canvas? FindCanvasInChildren(Visual visual)
    {
        if (visual is Canvas canvas)
            return canvas;

        foreach (var child in visual.GetVisualChildren())
        {
            var result = FindCanvasInChildren(child);
            if (result != null)
                return result;
        }
        return null;
    }

    private static LogicBlockViewModel? FindParentBlock(ConnectionPointViewModel connectionPoint)
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

    private static Point GetRelativeConnectionPointPosition(ConnectionPointViewModel connectionPoint, LogicBlockViewModel parentBlock)
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

    
}