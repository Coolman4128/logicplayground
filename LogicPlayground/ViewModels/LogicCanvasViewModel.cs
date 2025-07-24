using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using LogicPlayground.Models;
using LogicPlayground.ViewModels.LogicBlocks;
using LogicPlayground.ViewModels.LogicBlocks.Functions;
using LogicPlayground.ViewModels.LogicBlocks.Inputs;
using LogicPlayground.ViewModels.LogicBlocks.Outputs;

namespace LogicPlayground.ViewModels
{
    public partial class LogicCanvasViewModel : ViewModelBase
    {
        public ObservableCollection<LogicBlockViewModel> Blocks => LogicProcessor.Instance.Blocks;
        public ObservableCollection<ConnectionLine> ConnectionLines => ConnectionLineManager.Instance.ConnectionLines;
        public LogicBlockViewModel? SelectedBlock { get; set; } = null;

        [ObservableProperty]
        private TempConnectionLine _tempConnectionLine = new();

        [ObservableProperty]
        private double _zoomLevel = 1.0;

        [ObservableProperty]
        private double _cameraOffsetX = 0;
        [ObservableProperty]
        private double _cameraOffsetY = 0;

        private Point? _lastPanPoint = null;

        private bool _isDragging = false;




        public LogicCanvasViewModel()
        {



        }

        public void AddLogicBlock(string blockType)
        {
            LogicBlockViewModel block = blockType switch
            {
                "LogicGate" => new LogicGateFunctionViewModel(),
                "LogDigitalOutput" => new LogDigitalOutputViewModel(),
                "ConstDigitalInput" => new ConstDigitalInputViewModel(),
                _ => throw new ArgumentException("Unknown block type", nameof(blockType))
            };

            LogicProcessor.Instance.AddBlock(block);
            SelectedBlock = block;
        }

        public void StartDrag(Point point)
        {
            _isDragging = true;
            _lastPanPoint = point;
        }

        public void DragTo(Point point)
        {
            if (!_isDragging || _lastPanPoint == null)
                return;

            double deltaX = point.X - _lastPanPoint.Value.X;
            double deltaY = point.Y - _lastPanPoint.Value.Y;

            _lastPanPoint = point;

            // Update camera offset to pan the canvas
            CameraOffsetX += deltaX;
            CameraOffsetY += deltaY;
            
            // Update all connection lines when canvas is panned
            ConnectionLineManager.Instance.UpdateAllLines();
        }
        
        
        public void StopDrag()
        {
            _isDragging = false;
            _lastPanPoint = null;
        }
       

       
    }
}