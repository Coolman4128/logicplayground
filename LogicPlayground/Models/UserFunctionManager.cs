using System.Collections.ObjectModel;
using LogicPlayground.ViewModels;

namespace LogicPlayground.Models;

public static class UserFunctionManager
{
    public static ObservableCollection<UserDefinedFunctionViewModel> UserFunctions { get; } = new();

    public static void AddUserFunction(UserDefinedFunctionViewModel function)
    {
        UserFunctions.Add(function);
    }

    public static void RemoveUserFunction(UserDefinedFunctionViewModel function)
    {
        UserFunctions.Remove(function);
    }

    

}