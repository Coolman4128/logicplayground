using Avalonia.Controls;
using LogicPlayground.ViewModels;

namespace LogicPlayground.Views;

public partial class LogicProgrammingView : UserControl
{
    public LogicProgrammingView()
    {
        InitializeComponent();
    }

    private void LogicBlockSelector_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is string selectedBlockType)
        {
            // Call the command on the ViewModel
            if (DataContext is LogicProgrammingViewModel viewModel)
            {
                viewModel.AddLogicBlockCommand.Execute(selectedBlockType);
            }
            
            // Reset the selection to allow selecting the same item again
            comboBox.SelectedIndex = -1;
        }
    }

    private void UserFunctionSelector_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is UserDefinedFunctionViewModel selectedFunction)
        {
            // Call the command on the ViewModel
            if (DataContext is LogicProgrammingViewModel viewModel)
            {
                viewModel.AddUserDefinedFunctionBlockCommand.Execute(selectedFunction.Name);
            }
            
            // Reset the selection to allow selecting the same item again
            comboBox.SelectedIndex = -1;
        }
    }

    private void SwitchFunctionSelector_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is UserDefinedFunctionViewModel selectedFunction)
        {
            // Call the command on the ViewModel
            if (DataContext is LogicProgrammingViewModel viewModel)
            {
                viewModel.SwitchToUserFunctionCommand.Execute(selectedFunction);
            }
        }
    }
}
