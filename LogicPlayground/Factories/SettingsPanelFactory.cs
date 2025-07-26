using Avalonia.Controls;
using LogicPlayground.ViewModels.LogicBlocks;
using LogicPlayground.ViewModels.LogicBlocks.Functions;
using LogicPlayground.ViewModels.LogicBlocks.Inputs;
using LogicPlayground.ViewModels.LogicBlocks.Outputs;
using LogicPlayground.Views.Settings;

namespace LogicPlayground.Factories;

public static class SettingsPanelFactory
{
    public static UserControl? CreateSettingsView(LogicBlockViewModel viewModel)
    {
        return viewModel switch
        {
            LogicGateFunctionViewModel lgfvm => new LogicGateFunctionSettingsView { DataContext = lgfvm },
            CompareFunctionViewModel cfvm => new CompareFunctionSettingsView { DataContext = cfvm },
            MathFunctionViewModel mfvm => new MathFunctionSettingsView { DataContext = mfvm },
            ConstAnalogInputViewModel caivm => new ConstAnalogInputSettingsView { DataContext = caivm },
            ConstDigitalInputViewModel cdivm => new ConstDigitalInputSettingsView { DataContext = cdivm },
            LogAnalogOutputViewModel laovm => new LogAnalogOutputSettingsView { DataContext = laovm },
            LogDigitalOutputViewModel ldovm => new LogDigitalOutputSettingsView { DataContext = ldovm },
            LightOutputViewModel lovm => new LightOutputSettingsView { DataContext = lovm },
            DigitalToAnalogViewModel dtavm => new DigitalToAnalogSettingsView { DataContext = dtavm },
            DecimalToBinaryViewModel dtbvm => new DecimalToBinarySettingsView { DataContext = dtbvm },
            BinaryToDecimalViewModel btdvm => new BinaryToDecimalSettingsView { DataContext = btdvm },
            _ => null
        };
    }
}
