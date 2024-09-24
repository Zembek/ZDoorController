using System.Device.Gpio;

namespace ZDoorController.Interface.App.Modules.Relays
{
    public static class RelayItemExtensions
    {
        public static PinValue ActiveState(this RelayItem ths) => ths.IsActiveLow ? PinValue.Low : PinValue.High;
        public static PinValue DisabledState(this RelayItem ths) => !ths.ActiveState();
    }
}
