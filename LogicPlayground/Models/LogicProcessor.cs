using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LogicPlayground.ViewModels.LogicBlocks;

namespace LogicPlayground.Models;

public partial class LogicProcessor : ObservableObject
{
    public ObservableCollection<LogicBlockViewModel> Blocks { get; } = new ObservableCollection<LogicBlockViewModel>();

    public static LogicProcessor Instance { get; private set; } = new LogicProcessor();

    [ObservableProperty]
    private int _cycleTime = 100; // Default cycle time in milliseconds

    [ObservableProperty]
    private bool _paused = true;

    public void AddBlock(LogicBlockViewModel block)
    {
        Blocks.Add(block);
    }

    public void RemoveBlock(LogicBlockViewModel block)
    {
        Blocks.Remove(block);
    }

    public void RemoveBlock(Guid id)
    {
        var block = Blocks.FirstOrDefault(b => b.Guid == id);
        if (block != null)
        {
            Blocks.Remove(block);
        }
    }

    public void StartProcessing()
    {
        _ = Task.Run(async () =>
        {
            while (true)
            {
                if (Paused)
                {
                    await Task.Delay(CycleTime); // Wait a bit before checking again
                    continue;
                }
                var startTime = System.Diagnostics.Stopwatch.StartNew();
                if (!Paused)
                {
                    ProcessSingleCycle();
                }
                startTime.Stop();
                var elapsed = startTime.ElapsedMilliseconds;
                if (elapsed > CycleTime)
                {
                    Console.WriteLine($"Cycle took too long: {elapsed} ms, expected: {CycleTime} ms");
                }
                await Task.Delay(Math.Max((int)(CycleTime - elapsed), 0));
            }
        });
    }

    public void ProcessSingleCycle()
    {
        var SortedBlocks = Blocks.OrderBy(b => b.BlockPositionX).ToList();
        foreach (var block in SortedBlocks)
        {
            block.Process();
        }
    }
}
