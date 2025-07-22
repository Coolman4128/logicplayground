
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using LogicPlayground.ViewModels.LogicBlocks;

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
            }

            e.Handled = true; // Prevent event bubbling to parent drag handler
        }
    }

    private static void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is not Control control)
            return;

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
                _pressedConnectionPoint!.AttemptConnection(releasedVm);
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

    
}