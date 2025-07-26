using Avalonia.Controls;
using Avalonia.Controls.Templates;
using LogicPlayground.ViewModels;

namespace LogicPlayground.Views;

public class ViewModelToViewConverter : IDataTemplate
{
    public Control? Build(object? param)
    {
        return param switch
        {
            LogicProgrammingViewModel => new LogicProgrammingView(),
            _ => new TextBlock { Text = "View not implemented yet" }
        };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
