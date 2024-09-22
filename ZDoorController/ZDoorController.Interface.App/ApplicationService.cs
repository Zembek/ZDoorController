using Microsoft.Extensions.Hosting;
using System.Device.Gpio;
using ZDoorController.Interface.App.Modules.Buttons;
using ZDoorController.Interface.App.Modules.Interfaces;

namespace ZDoorController.Interface.App
{
    public class ApplicationService : IHostedService
    {
        private readonly IPhotoModule _photoModule;
        private readonly IButtonModule _buttonModule;

        public ApplicationService(IPhotoModule photoModule, IButtonModule buttonModule)
        {
            _photoModule = photoModule;
            _buttonModule = buttonModule;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            bool executeApp = true;
            while (executeApp)
            {
                List<MatrixButton> pressedButtons = _buttonModule.ArePressed();
                foreach (MatrixButton button in pressedButtons)
                {
                    int buttonIndex = _buttonModule.Buttons.IndexOf(button);
                    if (buttonIndex < 0)
                        continue;

                    Console.WriteLine($"Button pressed: {button.Name}");
                    if (button.Name == "Four")
                    {
                        Console.WriteLine($"Clossing app");
                        executeApp = false;
                    }

                }

                Thread.Sleep(500);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
