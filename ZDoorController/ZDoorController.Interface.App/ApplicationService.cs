﻿using Microsoft.Extensions.Hosting;
using System.Device.Gpio;
using ZDoorController.Interface.App.Modules.Buttons;
using ZDoorController.Interface.App.Modules.Interfaces;

namespace ZDoorController.Interface.App
{
    public class ApplicationService : IHostedService, IDisposable
    {
        private readonly IPhotoModule _photoModule;
        private readonly IButtonModule _buttonModule;
        private readonly IRelayModule _relayModule;

        public ApplicationService(IPhotoModule photoModule, IButtonModule buttonModule, IRelayModule relayModule)
        {
            _photoModule = photoModule;
            _buttonModule = buttonModule;
            _relayModule = relayModule;
        }

        public void Dispose()
        {
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
                    bool relayActive = _relayModule.SwitchRelay(button.Name);

                    Console.WriteLine($"Relay: {button.Name} is {(relayActive ? "Active" : "Disabled")}");
                }

                Thread.Sleep(500);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
