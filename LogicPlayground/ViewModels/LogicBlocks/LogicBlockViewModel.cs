using System;
using System.Collections.Generic;
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
        private double _blockPositionX = LogicCanvasViewModel.CANVASSIZE_WIDTH / 2;

        [ObservableProperty]
        private double _blockPositionY = LogicCanvasViewModel.CANVASSIZE_HEIGHT / 2;

        // Add to ViewModel:
        [ObservableProperty]
        private double _originalX;
        [ObservableProperty]
        private double _originalY;

        [ObservableProperty]
        private bool _isSelected = false;

        public ObservableCollection<ConnectionPointInputViewModel> Inputs { get; } = new();
        public ObservableCollection<ConnectionPointOutputViewModel> Outputs { get; } = new();


        // PUT EVERY BLOCK TYPE HERE
        public static List<string> BlockTypes { get; } = new()
        {
            "LogicGate",
        "LogDigitalOutput",
        "ConstDigitalInput",
        "ConstAnalogInput",
        "LogAnalogOutput",
        "MathFunction",
        "CompareFunction",

        };

        public virtual void Process()
        {
            throw new NotImplementedException();
        }


        public void DisconnectAll()
        {
            foreach (var input in Inputs)
            {
                input.Disconnect();
            }

            foreach (var output in Outputs)
            {
                foreach (var connection in output.ConnectedInputs)
                {
                    connection.Disconnect();
                }
            }
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

        public void Select()
        {
            IsSelected = true;
        }

        public void Deselect()
        {
            IsSelected = false;
        }
    }
}