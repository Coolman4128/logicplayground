using System;
using Avalonia;


namespace LogicPlayground.ViewModels
{
    public class LogicBlockViewModel : ViewModelBase
    {
        private Point _startPoint = new Point();
        public Point StartPoint
        {
            get => _startPoint;
            set => SetProperty(ref _startPoint, value);
        }

        private bool _isDragging = false;
        public bool IsDragging
        {
            get => _isDragging;
            set => SetProperty(ref _isDragging, value);
        }
        private Guid _guid = Guid.CreateVersion7();
        public Guid Guid
        {
            get => _guid;
            set => SetProperty(ref _guid, value);
        }

        private int _blockPositionX = 0;
        public int BlockPositionX
        {
            get => _blockPositionX;
            set => SetProperty(ref _blockPositionX, value);
        }

        private int _blockPositionY = 0;

        public int BlockPositionY
        {
            get => _blockPositionY;
            set => SetProperty(ref _blockPositionY, value);
        }

        public virtual void Process()
        {
            throw new NotImplementedException();
        }
        

        public void StartDrag(Point position)
    {
        _startPoint = position;
        _isDragging = true;
    }

    public void DragTo(Point position)
    {
        if (_isDragging)
        {
            BlockPositionX += (int) (position.X - _startPoint.X);
            BlockPositionY += (int) (position.Y - _startPoint.Y);
            _startPoint = position;
        }
    }

    public void EndDrag()
    {
        _isDragging = false;
    }
    }
}