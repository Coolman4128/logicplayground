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
        public ObservableCollection<LogicBlockViewModel> Blocks { get; private set; } = null!;
        public ObservableCollection<ConnectionLine> ConnectionLines => ConnectionLineManager.Instance.ConnectionLines;

        public Dictionary<string, double> Variables { get; private set; } = null!;
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

        [ObservableProperty]
        private bool _isUserFunction = false;

        public static double CANVASSIZE_WIDTH { get; } = 20000;
        public static double CANVASSIZE_HEIGHT { get; } = 10000;





        public LogicCanvasViewModel(bool isUserFunction = false)
        {
            if (isUserFunction)
            {
                IsUserFunction = true;
                var userFunction = new UserDefinedFunctionViewModel();
                UserFunctionManager.AddUserFunction(userFunction);
                Blocks = userFunction.LogicBlocks;
                Variables = userFunction.Variables;
            }
            else
            {
                Blocks = LogicProcessor.Instance.Blocks;
                Variables = LogicProcessor.Instance.Variables;
            }



        }

        public void AddLogicBlock(string blockType)
        {
            LogicBlockViewModel block = blockType switch
            {
                "LogicGate" => new LogicGateFunctionViewModel(this),
                "LogDigitalOutput" => new LogDigitalOutputViewModel(this),
                "ConstDigitalInput" => new ConstDigitalInputViewModel(this),
                "ConstAnalogInput" => new ConstAnalogInputViewModel(this),
                "LogAnalogOutput" => new LogAnalogOutputViewModel(this),
                "MathFunction" => new MathFunctionViewModel(this),
                "CompareFunction" => new CompareFunctionViewModel(this),
                "DigitalToAnalog" => new DigitalToAnalogViewModel(this),
                "LightOutput" => new LightOutputViewModel(this),
                "DecimalToBinary" => new DecimalToBinaryViewModel(this),
                "BinaryToDecimal" => new BinaryToDecimalViewModel(this),
                "VariableDigitalOutput" => new VariableDigitalOutputViewModel(this),
                "VariableAnalogOutput" => new VariableAnalogOutputViewModel(this),
                "VariableDigitalInput" => new VariableDigitalInputViewModel(this),
                "VariableAnalogInput" => new VariableAnalogInputViewModel(this),
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

        public void SelectBlock(LogicBlockViewModel block)
        {
            // Deselect all other blocks
            foreach (var b in Blocks)
            {
                b.Deselect();
            }

            // Select the clicked block
            block.Select();
            SelectedBlock = block;
        }

        public void DeselectAllBlocks()
        {
            foreach (var block in Blocks)
            {
                block.Deselect();
            }
            SelectedBlock = null;
        }

        public void DeleteSelectedBlock()
        {
            if (SelectedBlock != null)
            {
                SelectedBlock.DisconnectAll();
                LogicProcessor.Instance.RemoveBlock(SelectedBlock);
                SelectedBlock = null;
            }
        }

        public double GetVarValue(string variableName)
        {
            if (Variables.TryGetValue(variableName, out double value))
            {
                return value;
            }
            else
            {
                Variables[variableName] = 0.0; // Initialize if not found
                return 0.0;
            }
        }
        
        public void SetVarValue(string variableName, double value)
        {
            if (Variables.ContainsKey(variableName))
            {
                Variables[variableName] = value;
            }
            else
            {
                Variables.Add(variableName, value);
            }
        }
       

       
    }
}