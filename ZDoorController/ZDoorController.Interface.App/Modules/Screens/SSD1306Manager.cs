using RPiButtons.SSD1306.Core;
using ZDoorController.Interface.App.Modules.Screens.Common;

namespace RPiButtons.SSD1306
{
    /// <summary>
    /// Pretty old code that I adjusted for Raspberry Windows IoT. Source code came from MS github. I didn't want to change original structure - I added this "Manager" class only.
    /// </summary>
    public class SSD1306Manager
    {
        private readonly Display _display = new Display();
        private readonly ScreenDisplayLines _sceenLines = new ScreenDisplayLines();


        public void TurnOn()
        {
            _display.Init(new Common.DisplayConfiguration());
        }

        public void TurnOff()
        {
            _display.TurnOffDisplay();
        }

        public void WriteMessageAndUpdate(uint line, uint column, string message)
        {
            WriteMessage(line, column, message);
            Update();
        }

        public void WriteMessage(uint line, uint column, string message)
        {
            _display.WriteLineDisplayBuf(message, column, line);
            _sceenLines.UpdateScreenLine(line, column, message);
        }

        public void Update(bool forceUpdate = false)
        {
            if(forceUpdate || _sceenLines.IsUpdateRequired)
            {
                _display.DisplayUpdate();
                _sceenLines.UpdatePerformed();
            }
        }

        public void DrawPikachu(uint line, uint column)
        {
            _display.WriteImageDisplayBuf(DisplayImages.Pikachu, column, line);
            Update();
        }

        public void Clear()
        {
            _display.ClearDisplayBuf();
            Update();
        }


    }
}
