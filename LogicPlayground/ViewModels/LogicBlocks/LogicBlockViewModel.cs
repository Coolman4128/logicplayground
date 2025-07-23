using System;
using System.Collections.ObjectModel;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using LogicPlayground.Models;


namespace LogicPlayground.ViewModels.LogicBlocks
{
    public partial class LogicBlockViewModel : ViewModelBase
    {
        [ObservableProperty]
        private Point _startPoint = new Point();

        [ObservableProperty]
        private bool _isDragging = false;

        [ObservableProperty]
        private Guid _guid = Guid.CreateVersion7();

        [ObservableProperty]
        private double _blockPositionX = 0;

        [ObservableProperty]
        private double _blockPositionY = 0;

        // Add to ViewModel:
        [ObservableProperty]
        private double _originalX;
        [ObservableProperty]
        private double _originalY;


        public ObservableCollection<ConnectionPointInputViewModel> Inputs { get; } = new();
        public ObservableCollection<ConnectionPointOutputViewModel> Outputs { get; } = new();

        public virtual void Process()
        {
            throw new NotImplementedException();
        }




        public void StartDrag(Point position)
        {
            StartPoint = position;
            OriginalX = BlockPositionX;
            OriginalY = BlockPositionY;
            IsDragging = true;
            Console.WriteLine($"Started dragging block at position: {BlockPositionX}, {BlockPositionY}");
        }

        public void DragTo(Point position)
        {
            if (IsDragging)
            {
                var deltaX = position.X - StartPoint.X;
                var deltaY = position.Y - StartPoint.Y;
                BlockPositionX = OriginalX + deltaX;
                BlockPositionY = OriginalY + deltaY;
                
                // Update connection lines for this block when it moves
                ConnectionLineManager.Instance.UpdateLinesForBlock(this);
            }
            Console.WriteLine($"Dragging block to position: {BlockPositionX}, {BlockPositionY}");
        }

        public void EndDrag()
        {
            IsDragging = false;
        }
    }
}