using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using LogicPlayground.ViewModels;
using Avalonia.ReactiveUI;
using System.Net.Mail;

namespace LogicPlayground.Behaviors;

public static class DragBehavior
{
    public static readonly AttachedProperty<bool> EnableDragProperty =
            AvaloniaProperty.RegisterAttached<AvaloniaObject, bool>("EnableDrag", typeof(DragBehavior));

    public static bool GetEnableDrag(AvaloniaObject element) => element.GetValue(EnableDragProperty);
    public static void SetEnableDrag(AvaloniaObject element, bool value) => element.SetValue(EnableDragProperty, value);

    static DragBehavior()
    {
        EnableDragProperty.Changed.AddClassHandler<Control>((control, e) =>
        {
            if (e.NewValue as bool? ?? false)
            {
                control.PointerPressed += OnPointerPressed;
                control.PointerMoved += OnPointerMoved;
                control.PointerReleased += OnPointerReleased;
            }
        });
    }

    private static void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control && control.DataContext is LogicBlockViewModel vm)
        {
            var point = e.GetPosition(control);
            vm.StartDrag(point);

        }
    }

    private static void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is Control control && control.DataContext is LogicBlockViewModel vm)
        {
            var point = e.GetPosition(control);
            vm.DragTo(point);
        }
    }

    private static void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Control control && control.DataContext is LogicBlockViewModel vm)
        {
            vm.EndDrag();

        }
    }
}