using Microsoft.Extensions.Configuration;
using System.Device.Gpio;
using ZDoorController.Interface.App.Modules.Interfaces;

namespace ZDoorController.Interface.App.Modules.ReedSwitches
{
    public class ReedSwitchModule : IReedSwitchModule, IDisposable
    {
        private readonly int _pinId;
        private readonly GpioController _gpioController;

        public ReedSwitchModule(IConfiguration configuration, GpioController gpioController)
        {
            _pinId = configuration.GetValue<int>("Modules:ReedSwitch:Pin");
            _gpioController = gpioController;

            _gpioController.OpenPin(_pinId, PinMode.InputPullUp);
        }

        public bool IsClosed
        {
            get
            {
                PinValue pinValue = _gpioController.Read(_pinId);
                return pinValue == PinValue.High;
            }
        }

        public void Dispose()
        {
            _gpioController.ClosePin(_pinId);
        }
    }
}
