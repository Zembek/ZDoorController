using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RPiButtons.SSD1306;
using System.Runtime.ConstrainedExecution;
using UnitsNet;
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
        private readonly SSD1306Manager _displayManager;
        private readonly IReedSwitchModule _reedSwitchModule;

        private bool RunApp { get; set; }

        public ApplicationService(IHostApplicationLifetime appLifetime,
            IPhotoModule photoModule,
            IButtonModule buttonModule,
            IRelayModule relayModule,
            IFaceRecognitionService faceRecognitionService,
            ITemperatureModule temperatureModule,
            SSD1306Manager displayManager,
            IReedSwitchModule reedSwitchModule,
            IConfiguration configuration)
        {
            _photoModule = photoModule;
            _buttonModule = buttonModule;
            _relayModule = relayModule;
            _fairRecognitionService = faceRecognitionService;
            _temperatureModule = temperatureModule;
            _reedSwitchModule = reedSwitchModule;
            _settings = configuration.GetSection("ApplicationConfigs").Get<ApplicationSettings>();
            _displayManager = displayManager;
            _displayManager.TurnOn();

            appLifetime.ApplicationStopping.Register(OnStopping);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("App is running");
            RunApp = true;
            while (RunApp)
            {
                _displayManager.DrawPikachu(0,0);
                //await CheckButtons();

                //_displayManager.Clear();

                //GetTemperature();
                //GetReedSwitch();

                _displayManager.Update();

                Thread.Sleep(500);
            }
        }

        private void GetReedSwitch()
        {
            bool isReedSwithClosed = _reedSwitchModule.IsClosed;
            _displayManager.WriteMessage(3, 0, $"Switch closed: {isReedSwithClosed}");
            Console.WriteLine($"Reed Switch is closed: {isReedSwithClosed}");
        }

        private void GetTemperature()
        {
            Console.WriteLine($"Number of temperature sensors: {_temperatureModule.Configuration.Sensors.Length}");
            for (uint i = 0; i < _temperatureModule.Configuration.Sensors.Length; i++)
            {
                string sensor = _temperatureModule.Configuration.Sensors[i];
                double temperature = _temperatureModule.GetTemperature(sensor);
                Console.WriteLine($"Sensor: {sensor}, temperature: {temperature}°C");
                _displayManager.WriteMessage(i, 0, $"Sensor {i + 1}: {temperature}°C");
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
            _displayManager.TurnOff();
        }
    }
}
