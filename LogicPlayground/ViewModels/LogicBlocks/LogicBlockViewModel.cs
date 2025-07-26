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

        [ObservableProperty]
        private double _blockWidth = 200;

        [ObservableProperty]
        private double _blockHeight = 200;

        public ObservableCollection<ConnectionPointInputViewModel> Inputs { get; } = new();
        public ObservableCollection<ConnectionPointOutputViewModel> Outputs { get; } = new();

        public LogicBlockViewModel()
        {
            // Set up event handlers to update block size when collections change
            Inputs.CollectionChanged += (_, _) => UpdateBlockSize();
            Outputs.CollectionChanged += (_, _) => UpdateBlockSize();
            
            // Initial size calculation
            UpdateBlockSize();
        }

        // PUT EVERY BLOCK TYPE HERE
        public static List<string> BlockTypes { get; } = new()
        {
            
        "LogDigitalOutput",
        "LogAnalogOutput",
        "ConstDigitalInput",
        "ConstAnalogInput",
        "LogicGate",
        "MathFunction",
        "CompareFunction",
        "DigitalToAnalog",

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
                var copyOutputs = new List<ConnectionPointInputViewModel>(output.ConnectedInputs);
                foreach (var connection in copyOutputs)
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

        protected void UpdateBlockSize()
        {
            const double minSize = 200;
            const double connectionSpacing = 20;
            
            var inputHeight = Math.Max(minSize, Inputs.Count * connectionSpacing);
            var outputHeight = Math.Max(minSize, Outputs.Count * connectionSpacing);
            
            BlockHeight = Math.Max(inputHeight, outputHeight);
            BlockWidth = minSize; // Keep width constant for now
            
            // Update connection lines when size changes
            ConnectionLineManager.Instance.UpdateLinesForBlock(this);
        }
    }
}