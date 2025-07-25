using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.ReactiveUI;
using System.Net.Mail;
using LogicPlayground.ViewModels.LogicBlocks;
using LogicPlayground.ViewModels;
using System;

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
        if (sender is Control control)
        {
            // Handle both direct LogicBlock controls and ContentPresenter containers
            var viewModel = control.DataContext as LogicBlockViewModel;
            if (viewModel != null)
            {
                var parentCanvas = FindParentCanvas(control as Visual);
                if (parentCanvas != null)
                {
                    var point = e.GetPosition(parentCanvas);
                    viewModel.StartDrag(point);
                    
                    // Handle block selection
                    var canvasViewModel = FindLogicCanvasViewModel(control);
                    if (canvasViewModel != null)
                    {
                        canvasViewModel.SelectBlock(viewModel);
                    }
                    
                    e.Pointer.Capture(control);
                    e.Handled = true; // Mark the event as handled to prevent further processing
                }
            }
        }
    }


    private static void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is Control control)
        {
            var viewModel = control.DataContext as LogicBlockViewModel;
            if (viewModel != null && viewModel.IsDragging)
            {
                var canvas = FindParentCanvas(control as Visual);
                if (canvas != null)
                {
                    var point = e.GetPosition(canvas);
                    viewModel.DragTo(point);
                    e.Handled = true;
                }
            }
        }
    }

    private static void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Control control)
        {
            var viewModel = control.DataContext as LogicBlockViewModel;
            if (viewModel != null && viewModel.IsDragging)
            {
                viewModel.EndDrag();
                e.Pointer.Capture(null);
                e.Handled = true;
            }
        }
    }

    // Helper to find the parent Canvas (Panel) in the visual tree
    private static Panel? FindParentCanvas(Visual? visual)
    {
        while (visual != null)
        {
            if (visual is Panel panel)
                return panel;
            visual = visual.GetVisualParent() as Visual;
        }
        return null;
    }

    // Helper to find the LogicCanvasViewModel in the visual tree
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
}