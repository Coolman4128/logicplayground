using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LogicPlayground.ViewModels
{
    public class LogicCanvasViewModel : ViewModelBase
    {
        public List<LogicBlockViewModel> LogicBlocks { get; set; } = new List<LogicBlockViewModel>();
        public LogicBlockViewModel SelectedBlock { get; set; } = new LogicBlockViewModel();

        
       
        public LogicCanvasViewModel()
        {
            // Initialize with some default blocks
            LogicBlocks.Add(new LogicBlockViewModel { Caption = "Block 1" });
            LogicBlocks.Add(new LogicBlockViewModel { Caption = "Block 2" });
            LogicBlocks.Add(new LogicBlockViewModel { Caption = "Block 3" });

            _ = Task.Run(() => MoveText());
        }


        public async Task MoveText()
        {
            while (true)
            {
                if (SelectedBlock.BlockPositionX < 100)
                {
                    SelectedBlock.BlockPositionX++;
                }
                else
                {
                    SelectedBlock.BlockPositionX = 0;
                }
                SelectedBlock.BlockPositionY = (int)Math.Floor(50 * Math.Sin(SelectedBlock.BlockPositionX / 4)) + 50;
                await Task.Delay(10);
            }
        }
    }
}