using System;
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

    public static UserDefinedFunctionViewModel? GetUserFunctionById(Guid id)
    {
        foreach (var function in UserFunctions)
        {
            if (function.Id == id)
            {
                return function;
            }
        }
        return null;
    }

    public static UserDefinedFunctionViewModel? GetUserFunctionByName(string name)
    {
        foreach (var function in UserFunctions)
        {
            if (function.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return function;
            }
        }
        return null;
    }


}