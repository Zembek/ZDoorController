namespace ZDoorController.Interface.App.Modules.Interfaces
{
    public interface IRelayModule
    {
        void ActivateRelay(string relayName);
        void DisableRelay(string relayName);
        bool SwitchRelay(string relayName);
        void SetRelayState(string relayName, bool isActive);
    }
}
