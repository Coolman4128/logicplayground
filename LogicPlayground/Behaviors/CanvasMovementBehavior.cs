using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using LogicPlayground.ViewModels;

namespace LogicPlayground.Behaviors
{
public static class CanvasMovementBehavior
{
    // Getter and setter for the attached property so Avalonia can resolve it in XAML
    public static bool GetEnablePan(AvaloniaObject obj) => obj.GetValue(EnablePanProperty);
    public static void SetEnablePan(AvaloniaObject obj, bool value) => obj.SetValue(EnablePanProperty, value);
        public static readonly AttachedProperty<bool> EnablePanProperty =
            AvaloniaProperty.RegisterAttached<AvaloniaObject, bool>("EnablePan", typeof(CanvasMovementBehavior));


        static CanvasMovementBehavior()
        {
            EnablePanProperty.Changed.AddClassHandler<Control>((control, e) =>
            {
                var enable = e.NewValue as bool? ?? false;
                if (enable)
                {
                    control.PointerWheelChanged += OnPointerWheelChanged;
                    control.PointerPressed += OnPointerPressed;
                    control.PointerMoved += OnPointerMoved;
                    control.PointerReleased += OnPointerReleased;
                }
                else
                {
                    control.PointerWheelChanged -= OnPointerWheelChanged;
                    control.PointerPressed -= OnPointerPressed;
                    control.PointerMoved -= OnPointerMoved;
                    control.PointerReleased -= OnPointerReleased;
                }
            });
        }

        private static void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            Console.WriteLine("PointerWheelChanged event triggered");
            if (sender is Control control && control.DataContext is LogicCanvasViewModel vm)
            {
                Console.WriteLine("Zooming in/out");
                double zoomDelta = e.Delta.Y > 0 ? 1.1 : 0.9;
                vm.ZoomLevel *= zoomDelta;
            }
        }

        private static void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is Control control && control.DataContext is LogicCanvasViewModel vm)
            {
                if (e.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
                {
                    // Deselect all blocks when clicking on empty canvas area
                    vm.DeselectAllBlocks();
                    vm.StartDrag(e.GetPosition(control));
                }
            }
        }

        private static void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (sender is Control control && control.DataContext is LogicCanvasViewModel vm)
            {
                if (e.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
                {
                    vm.DragTo(e.GetPosition(control));
                }
            }
        }


        private static void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (sender is Control control && control.DataContext is LogicCanvasViewModel vm)
            {
                vm.StopDrag();
            }
        }
    }
}