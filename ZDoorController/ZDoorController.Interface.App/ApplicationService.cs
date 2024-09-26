using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;
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
            Console.WriteLine("App is running");
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
            byte[] currentFace = _photoModule.CapturePhoto();

            Console.WriteLine("Grabbing photo");
            string[] validFaces = Directory.GetFiles(_settings.ValidFacesPath);
            Console.WriteLine($"Number of valid faces: {validFaces.Length}");
            foreach (string facePath in validFaces)
            {
                Console.WriteLine($"Checking face: {facePath}");
                byte[] correctFace = File.ReadAllBytes(facePath);
                double verificationConfidence = _fairRecognitionService.VerifyFacesConfidence(correctFace, currentFace);
                Console.WriteLine($"Face confidence: {verificationConfidence}/{_settings.MinConfidenceToOpenDoor}");
                if (verificationConfidence >= _settings.MinConfidenceToOpenDoor)
                {
                    Console.WriteLine("Opening door");
                    _relayModule.ActivateRelay(_settings.OpenDoorRelayName);
                    Thread.Sleep(5000);
                    Console.WriteLine("Close door");
                    _relayModule.DisableRelay(_settings.OpenDoorRelayName);
                    break;
                }
            }
        }

        private void SaveFace()
        {
            Console.WriteLine("Save face");

            string faceDirectoryPath = _settings.ValidFacesPath;
            string tmpFileName = Path.ChangeExtension(Path.GetRandomFileName(), ".jpeg");
            _photoModule.CaptureAndSavePhoto($"{faceDirectoryPath}{tmpFileName}");

            Console.WriteLine($"File saved: {faceDirectoryPath + tmpFileName}");
        }

        private void OnStopping()
        {
            Console.WriteLine("stopping app");
            RunApp = false;
        }
    }
}
