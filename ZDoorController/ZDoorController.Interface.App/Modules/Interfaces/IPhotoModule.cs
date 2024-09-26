namespace ZDoorController.Interface.App.Modules.Interfaces
{
    public interface IPhotoModule
    {
        byte[] CapturePhoto();
        void CaptureAndSavePhoto(string filePath);
    }
}
