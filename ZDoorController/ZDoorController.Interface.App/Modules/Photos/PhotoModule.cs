using Iot.Device.Media;
using Microsoft.Extensions.Configuration;
using ZDoorController.Interface.App.Modules.Interfaces;
using ZDoorController.Interface.App.Modules.Photo;

namespace ZDoorController.Interface.App.Modules.Photos
{
    public class PhotoModule : IPhotoModule
    {
        private readonly VideoConnectionSettings _settings;
        private readonly PhotoConfiguration _photoConfiguration;

        public PhotoModule(IConfiguration configuration)
        {
            _photoConfiguration = configuration.GetSection("PhotoModule").Get<PhotoConfiguration>() ?? new PhotoConfiguration();
            _settings = new VideoConnectionSettings(busId: 0, captureSize: (_photoConfiguration.width, _photoConfiguration.height), pixelFormat: VideoPixelFormat.JPEG);
        }

        public byte[] CapturePhoto()
        {
            using VideoDevice device = VideoDevice.Create(_settings);
            return device.Capture();
        }
    }
}
