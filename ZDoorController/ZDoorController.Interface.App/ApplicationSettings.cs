namespace ZDoorController.Interface.App
{
    public class ApplicationSettings
    {
        public string SavePhotoButtonName { get; set; }
        public string ValidateFaceButtonName { get; set; }
        public string ValidFacesPath { get; set; }
        public string OpenDoorRelayName {  get; set; }
        public double MinConfidenceToOpenDoor { get; set; }
    }
}
