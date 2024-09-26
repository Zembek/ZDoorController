using Microsoft.Extensions.Configuration;
using System.Device.Gpio;
using ZDoorController.Interface.App.Modules.Interfaces;

namespace ZDoorController.Interface.App.Modules.Relays
{
    public class RelayModule : IRelayModule, IDisposable
    {
        private string CONFIGURATION_NAME = "Modules:RelayModule";

        private readonly GpioController _gpioController;
        public RelayConfiguration Configuration { get; private set; }

        public RelayModule(GpioController gpioController, IConfiguration configuration)
        {
            _gpioController = gpioController;
            Configuration = configuration.GetSection(CONFIGURATION_NAME).Get<RelayConfiguration>();
            InitializeRelays();
        }

        private void InitializeRelays()
        {
            foreach (var relay in Configuration.Relays)
            {
                _gpioController.OpenPin(relay.Pin, PinMode.Output);
                _gpioController.Write(relay.Pin, relay.DisabledState());
            }
        }

        public void ActivateRelay(string relayName)
        {
            SetRelayState(relayName, true);
        }

        public void DisableRelay(string relayName)
        {
            SetRelayState(relayName, false);
        }

        public bool SwitchRelay(string relayName)
        {
            RelayItem relay = GetRelay(relayName);
            SetRelayState(relay, !relay.IsActive);
            return relay.IsActive;
        }

        public void SetRelayState(string relayName, bool isActive)
        {
            RelayItem relay = GetRelay(relayName);
            SetRelayState(relay, isActive);
        }

        private RelayItem GetRelay(string relayName)
        {
            RelayItem? relay = Configuration.Relays.FirstOrDefault(q => q.Name == relayName);
            if (relay == null)
                throw new ArgumentOutOfRangeException("relayName", $"Relay with name {relayName} isn't initialized. Please check appsettings.json");
            return relay;
        }

        private void SetRelayState(RelayItem relay, bool isActive)
        {
            _gpioController.Write(relay.Pin, isActive ? relay.ActiveState() : relay.DisabledState());
            relay.IsActive = isActive;
        }

        public void Dispose()
        {
            foreach (var relay in Configuration.Relays)
            {
                relay.IsActive = false;
                _gpioController.OpenPin(relay.Pin, PinMode.Output);
                _gpioController.Write(relay.Pin, relay.DisabledState());
            }
        }
    }
}
