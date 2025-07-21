using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LogicPlayground.Models;
using LogicPlayground.ViewModels.LogicBlocks;
using LogicPlayground.ViewModels.LogicBlocks.Functions;
using LogicPlayground.ViewModels.LogicBlocks.Inputs;
using LogicPlayground.ViewModels.LogicBlocks.Outputs;

namespace LogicPlayground.ViewModels
{
    public class LogicCanvasViewModel : ViewModelBase
    {
        public ObservableCollection<LogicBlockViewModel> Blocks => LogicProcessor.Instance.Blocks;
        public LogicBlockViewModel? SelectedBlock { get; set; } = null;

        
       
        public LogicCanvasViewModel()
        {
           

           
        }

        public void AddLogicBlock(string blockType)
        {
            LogicBlockViewModel block = blockType switch
            {
                "LogicGate" => new LogicGateFunctionViewModel(),
                "LogDigitalOutput" => new LogDigitalOutputViewModel(),
                "ConstAnalogInput" => new ConstAnalogInputViewModel(),
                _ => throw new ArgumentException("Unknown block type", nameof(blockType))
            };

            LogicProcessor.Instance.AddBlock(block);
            SelectedBlock = block;
        }
       

       
    }
}