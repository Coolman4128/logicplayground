using Avalonia.Controls;
using LogicPlayground.ViewModels;

namespace LogicPlayground.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void LogicBlockSelector_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is string selectedBlockType)
        {
            // Call the command on the ViewModel
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.AddLogicBlockCommand.Execute(selectedBlockType);
            }
            
            // Reset the selection to allow selecting the same item again
            comboBox.SelectedIndex = -1;
        }
    }
}