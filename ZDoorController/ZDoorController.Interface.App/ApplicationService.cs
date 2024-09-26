using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ZDoorController.Interface.App.Interfaces;
using ZDoorController.Interface.App.Modules.Buttons;
using ZDoorController.Interface.App.Modules.Interfaces;

namespace ZDoorController.Interface.App
{
    public class ApplicationService : IHostedService
    {
        private readonly ApplicationSettings _settings;
        private readonly IPhotoModule _photoModule;
        private readonly IButtonModule _buttonModule;
        private readonly IRelayModule _relayModule;
        private readonly IFaceRecognitionService _fairRecognitionService;

        private bool RunApp { get; set; }

        public ApplicationService(IHostApplicationLifetime appLifetime, IPhotoModule photoModule, IButtonModule buttonModule, IRelayModule relayModule, IFaceRecognitionService faceRecognitionService, IConfiguration configuration)
        {
            _photoModule = photoModule;
            _buttonModule = buttonModule;
            _relayModule = relayModule;
            _fairRecognitionService = faceRecognitionService;
            _settings = configuration.GetSection("ApplicationConfigs").Get<ApplicationSettings>();

            appLifetime.ApplicationStopping.Register(OnStopping);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            RunApp = true;
            while (RunApp)
            {
                List<MatrixButton> pressedButtons = _buttonModule.ArePressed();
                foreach (MatrixButton button in pressedButtons)
                {
                    int buttonIndex = _buttonModule.Buttons.IndexOf(button);
                    if (buttonIndex < 0)
                        continue;

                    Console.WriteLine($"Button pressed: {button.Name}");
                    ProcessButton(button.Name);

                    //Console.WriteLine($"Button pressed: {button.Name}");
                    //bool relayActive = _relayModule.SwitchRelay(button.Name);

                    //Console.WriteLine($"Relay: {button.Name} is {(relayActive ? "Active" : "Disabled")}");
                }

                Thread.Sleep(500);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void ProcessButton(string buttonName)
        {
            if (buttonName == _settings.ValidateFaceButtonName)
                ValidateFace();
            else if (buttonName == _settings.SavePhotoButtonName)
                SaveFace();
        }

        private void ValidateFace()
        {
            Console.WriteLine("Face validation");
        }

        private void SaveFace()
        {
            Console.WriteLine("Save face");

            string faceDirectoryPath = _settings.ValidFacesPath;
            string tmpFileName = Path.ChangeExtension(Path.GetRandomFileName(), ".jpeg");
            _photoModule.CaptureAndSavePhoto($"{faceDirectoryPath}{tmpFileName}");
        }

        private void OnStopping()
        {
            Console.WriteLine("stopping app");
            RunApp = false;
        }
    }
}
