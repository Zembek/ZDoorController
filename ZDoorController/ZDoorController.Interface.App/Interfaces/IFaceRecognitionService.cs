namespace ZDoorController.Interface.App.Interfaces
{
    public interface IFaceRecognitionService
    {
        double VerifyFacesConfidence(byte[] imageOne, byte[] imageTwo);
        Task<double> VerifyFacesConfidenceAsync(byte[] imageOne, byte[] imageTwo);
    }
}
