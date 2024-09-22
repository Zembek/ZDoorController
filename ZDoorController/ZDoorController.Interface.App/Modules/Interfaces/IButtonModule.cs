using System.Device.Gpio;
using ZDoorController.Interface.App.Modules.Buttons;

namespace ZDoorController.Interface.App.Modules.Interfaces
{
    public interface IButtonModule
    {
        List<MatrixButton> Buttons { get; }
        bool IsPressed(MatrixButton buttonToCheck);
        List<MatrixButton> ArePressed();
        void Cleanup();
    }
}
