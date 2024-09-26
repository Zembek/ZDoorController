using Microsoft.Extensions.Configuration;
using System.Device.Gpio;
using ZDoorController.Interface.App.Modules.Interfaces;

namespace ZDoorController.Interface.App.Modules.Buttons
{
    public class ButtonModule : IButtonModule, IDisposable
    {
        private string CONFIGURATION_NAME = "Modules:ButtonModule";

        private readonly GpioController _gpioController;
        public List<MatrixButton> Buttons { get; private set; }

        private List<int> OutputPins => Buttons.Select(b => b.RowPin).Distinct().ToList();
        private List<int> InputPins => Buttons.Select(b => b.ColumnPin).Distinct().ToList();

        public ButtonModule(GpioController gpioController, IConfiguration configuration)
        {
            ButtonConfiguration buttonConfiguration = configuration.GetSection(CONFIGURATION_NAME).Get<ButtonConfiguration>();
            Buttons = buttonConfiguration.Buttons.ToList();
            _gpioController = gpioController;

            foreach (int pinNo in OutputPins)
            {
                _gpioController.OpenPin(pinNo, PinMode.Output);
                _gpioController.Write(pinNo, PinValue.Low);
            }

            foreach (int pinNo in InputPins)
            {
                _gpioController.OpenPin(pinNo, PinMode.InputPullDown);
            }
        }

        public bool IsPressed(MatrixButton buttonToCheck)
        {
            if (!Buttons.Contains(buttonToCheck))
                return false;

            foreach (int pinNo in OutputPins)
                _gpioController.Write(pinNo, PinValue.Low);

            _gpioController.Write(buttonToCheck.RowPin, PinValue.High);
            var state = _gpioController.Read(buttonToCheck.ColumnPin);
            _gpioController.Write(buttonToCheck.RowPin, PinValue.Low);

            return state == PinValue.High;
        }

        public List<MatrixButton> ArePressed()
        {
            List<MatrixButton> result = new List<MatrixButton>();

            foreach (int pinNo in OutputPins)
                _gpioController.Write(pinNo, PinValue.Low);

            foreach (int pinNo in OutputPins)
            {
                _gpioController.Write(pinNo, PinValue.High);

                List<MatrixButton> buttonsToCheck = Buttons
                    .Where(q => q.RowPin == pinNo)
                    .ToList();

                foreach (MatrixButton button in buttonsToCheck)
                {
                    var status = _gpioController.Read(button.ColumnPin);
                    if (status == PinValue.High)
                    {
                        result.Add(button);
                    }
                }
                _gpioController.Write(pinNo, PinValue.Low);
            }

            foreach (int pinNo in OutputPins)
                _gpioController.Write(pinNo, PinValue.Low);

            return result;
        }

        public void Dispose()
        {
            foreach (int pinNo in OutputPins)
            {
                _gpioController.ClosePin(pinNo);
            }

            foreach (int pinNo in InputPins)
            {
                _gpioController.ClosePin(pinNo);
            }
        }
    }
}
