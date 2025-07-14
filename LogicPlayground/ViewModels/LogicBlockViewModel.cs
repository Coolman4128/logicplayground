namespace LogicPlayground.ViewModels
{
    public class LogicBlockViewModel : ViewModelBase
    {
        public string Caption { get; set; } = "Test";

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
    }
}