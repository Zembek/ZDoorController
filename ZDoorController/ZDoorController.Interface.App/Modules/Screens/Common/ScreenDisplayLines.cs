namespace ZDoorController.Interface.App.Modules.Screens.Common
{
    public class ScreenDisplayLines
    {
        private readonly DisplayLine[] _screenLineArray = new DisplayLine[4];
        public bool IsUpdateRequired { get; private set; }

        public void UpdatePerformed()
        {
            IsUpdateRequired = false;
        }

        public void UpdateScreenLine(uint line, uint column, string message)
        {
            UpdateScreenLine(new DisplayLine
            {
                Column = column,
                Line = line,
                Message = message
            });
        }

        public void UpdateScreenLine(DisplayLine line)
        {
            DisplayLine currentLine = _screenLineArray[line.Line];
            if (currentLine == null)
            {
                _screenLineArray[line.Line] = line;
                return;
            }

            if (currentLine.Column != line.Column || currentLine.Message != line.Message)
            {
                _screenLineArray[line.Line] = line;
                IsUpdateRequired = true;
            }

        }
    }
}
