namespace ZDoorController.Interface.App.Interfaces
{
    public interface IFaceRecognitionService
    {
        bool VerifyFacesConfidence(byte[] imageOne, byte[] imageTwo);
        Task<bool> VerifyFacesConfidenceAsync(byte[] imageOne, byte[] imageTwo);
    }
}
