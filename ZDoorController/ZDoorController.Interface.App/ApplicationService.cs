using Microsoft.Extensions.Hosting;
using System.Device.Gpio;
using ZDoorController.Interface.App.Modules.Buttons;
using ZDoorController.Interface.App.Modules.Interfaces;

namespace ZDoorController.Interface.App
{
    public class ApplicationService : IHostedService, IHostedLifecycleService, IDisposable
    {
        private readonly IPhotoModule _photoModule;
        private readonly IButtonModule _buttonModule;
        private readonly IRelayModule _relayModule;

        public ApplicationService(IHostApplicationLifetime appLifetime, IPhotoModule photoModule, IButtonModule buttonModule, IRelayModule relayModule)
        {
            _photoModule = photoModule;
            _buttonModule = buttonModule;
            _relayModule = relayModule;


            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);
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

        public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private void OnStarted()
        {
            Console.WriteLine("OnStarted has been called.");
        }

        private void OnStopping()
        {
            Console.WriteLine("OnStopping has been called.");
        }
        private void OnStopped()
        {
            Console.WriteLine("OnStopped has been called.");
        }
    }
}
