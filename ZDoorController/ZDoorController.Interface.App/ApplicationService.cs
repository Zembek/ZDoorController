﻿using Microsoft.Extensions.Hosting;
using System.Device.Gpio;
using ZDoorController.Interface.App.Modules.Buttons;
using ZDoorController.Interface.App.Modules.Interfaces;

namespace ZDoorController.Interface.App
{
    public class ApplicationService : IHostedService, IHostedLifecycleService
    {
        private readonly IPhotoModule _photoModule;
        private readonly IButtonModule _buttonModule;
        private readonly IRelayModule _relayModule;

        private bool RunApp { get; set; }

        public ApplicationService(IHostApplicationLifetime appLifetime, IPhotoModule photoModule, IButtonModule buttonModule, IRelayModule relayModule)
        {
            _photoModule = photoModule;
            _buttonModule = buttonModule;
            _relayModule = relayModule;

            appLifetime.ApplicationStopping.Register(OnStopping);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //RunApp = true;
            while (RunApp)
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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stop app");
            RunApp = false;
            return Task.CompletedTask;
        }

        public Task StartingAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting app");
            RunApp = true;
            return Task.CompletedTask;
        }

        public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppingAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("stopping app");
            RunApp = false;
            return Task.CompletedTask;
        }

        private void OnStopping()
        {
            Console.WriteLine("stopping app");
            RunApp = false;
        }

        public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
