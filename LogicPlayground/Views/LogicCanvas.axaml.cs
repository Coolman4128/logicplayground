using System;
using Avalonia.Controls;
using Avalonia.Input;
using LogicPlayground.ViewModels;

namespace LogicPlayground.Views
{
    public partial class LogicCanvas : UserControl
    {
        public LogicCanvas()
        {
            InitializeComponent();
            Focusable = true; // Allow the control to receive focus
            KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (DataContext is LogicCanvasViewModel viewModel)
            {
                Console.WriteLine("Key pressed: " + e.Key);
                if (e.Key == Key.Delete || e.Key == Key.Back)
                {
                    Console.WriteLine("Deleting selected block");
                    viewModel.DeleteSelectedBlock();
                    e.Handled = true;
                }
            }
        }
    }
}