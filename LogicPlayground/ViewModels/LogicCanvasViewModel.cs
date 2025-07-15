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
           

           
        }


        public async Task MoveText()
        {
            while (true)
            {
                if (SelectedBlock.BlockPositionX < 1000)
                {
                    SelectedBlock.BlockPositionX++;
                }
                else
                {
                    SelectedBlock.BlockPositionX = 0;
                }
                SelectedBlock.BlockPositionY = (int)(250 * Math.Sin((double)SelectedBlock.BlockPositionX / 100.00) + 300);
                await Task.Delay(10);
            }
        }
    }
}