using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using LogicPlayground.ViewModels.LogicBlocks;
using LogicPlayground.ViewModels;
using System;

namespace LogicPlayground.Behaviors;

public static class DoubleClickBehavior
{
    public static readonly AttachedProperty<bool> EnableDoubleClickProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, bool>("EnableDoubleClick", typeof(DoubleClickBehavior));

    public static bool GetEnableDoubleClick(AvaloniaObject element) => element.GetValue(EnableDoubleClickProperty);
    public static void SetEnableDoubleClick(AvaloniaObject element, bool value) => element.SetValue(EnableDoubleClickProperty, value);

    private static DateTime _lastClickTime = DateTime.MinValue;
    private static readonly TimeSpan DoubleClickTimeout = TimeSpan.FromMilliseconds(500);

    static DoubleClickBehavior()
    {
        EnableDoubleClickProperty.Changed.AddClassHandler<Control>((control, e) =>
        {
            if (e.NewValue as bool? ?? false)
            {
                Console.WriteLine("DoubleClickBehavior enabled on control");
                control.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, handledEventsToo: true);
            }
            else
            {
                Console.WriteLine("DoubleClickBehavior disabled on control");
                control.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
            }
        });
    }

    private static void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Console.WriteLine("PointerPressed event triggered for DOUBLE CLICK");
        if (sender is not Control control) return;
        
        var currentTime = DateTime.Now;
        var timeSinceLastClick = currentTime - _lastClickTime;

        if (timeSinceLastClick <= DoubleClickTimeout)
        {
            // This is a double-click
            HandleDoubleClick(control);
            _lastClickTime = DateTime.MinValue; // Reset to prevent triple-click
        }
        else
        {
            _lastClickTime = currentTime;
        }
    }

    private static void HandleDoubleClick(Control control)
    {
        
        // Find the LogicBlockViewModel in the DataContext
        if (control.DataContext is LogicBlockViewModel logicBlockViewModel)
        {
            Console.WriteLine($"Double-clicked on LogicBlock: {logicBlockViewModel}");
            // Find the MainWindowViewModel to set the settings panel
            var mainWindow = control.FindLogicalAncestorOfType<Window>();
            if (mainWindow?.DataContext is MainWindowViewModel mainWindowViewModel)
            {

                mainWindowViewModel.SetActiveSettingsPanel(logicBlockViewModel);
            }
        }
    }
}
