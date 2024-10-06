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
        private readonly ITemperatureModule _temperatureModule;

        private bool RunApp { get; set; }

        public ApplicationService(IHostApplicationLifetime appLifetime,
            IPhotoModule photoModule,
            IButtonModule buttonModule,
            IRelayModule relayModule,
            IFaceRecognitionService faceRecognitionService,
            ITemperatureModule temperatureModule,
            IConfiguration configuration)
        {
            _photoModule = photoModule;
            _buttonModule = buttonModule;
            _relayModule = relayModule;
            _fairRecognitionService = faceRecognitionService;
            _temperatureModule = temperatureModule;
            _settings = configuration.GetSection("ApplicationConfigs").Get<ApplicationSettings>();

            appLifetime.ApplicationStopping.Register(OnStopping);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("App is running");
            RunApp = true;
            while (RunApp)
            {
                await CheckButtons();

                foreach (string sensor in _temperatureModule.Configuration.Sensors)
                {
                    double temperature = _temperatureModule.GetTemperature(sensor);
                    Console.WriteLine($"Sensor ID:{sensor}, temperature: {temperature}");
                }

                Thread.Sleep(500);
            }
        }

        private async Task CheckButtons()
        {
            List<MatrixButton> pressedButtons = _buttonModule.ArePressed();
            foreach (MatrixButton button in pressedButtons)
            {
                int buttonIndex = _buttonModule.Buttons.IndexOf(button);
                if (buttonIndex < 0)
                    continue;

                Console.WriteLine($"Button pressed: {button.Name}");
                await ProcessButton(button.Name);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task ProcessButton(string buttonName)
        {
            if (buttonName == _settings.ValidateFaceButtonName)
                await ValidateFace();
            else if (buttonName == _settings.SavePhotoButtonName)
                SaveFace();
        }

        private async Task ValidateFace()
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
                try
                {
                    bool isValidFace = await _fairRecognitionService.VerifyFacesConfidenceAsync(correctFace, currentFace);
                    Console.WriteLine($"Face is valid: {isValidFace}");
                    if (isValidFace)
                    {
                        Console.WriteLine("Opening door");
                        _relayModule.ActivateRelay(_settings.OpenDoorRelayName);
                        Thread.Sleep(5000);
                        Console.WriteLine("Close door");
                        _relayModule.DisableRelay(_settings.OpenDoorRelayName);
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Face: {facePath} throwed exteption: {e.Message}");

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
