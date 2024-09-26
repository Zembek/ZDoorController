using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Configuration;
using ZDoorController.Interface.App.Interfaces;

namespace ZDoorController.Interface.App.Services.FaceRecognition
{
    public class FaceRecognitionService : IFaceRecognitionService
    {
        private readonly FaceClient _faceClient;

        public FaceRecognitionService(IConfiguration configuration)
        {
            FaceRecognitionConfiguration faceConfig = configuration.GetSection("AzureCognitiveService:FaceService").Get<FaceRecognitionConfiguration>();
            _faceClient = new FaceClient(new ApiKeyServiceClientCredentials(faceConfig.Key)) { Endpoint = faceConfig.Endpoint };
        }

        public double VerifyFacesConfidence(byte[] imageOne, byte[] imageTwo)
        {
            var task = Task.Run(async () => await VerifyFacesConfidenceAsync(imageOne, imageTwo));
            return task.Result;
        }

        public async Task<double> VerifyFacesConfidenceAsync(byte[] imageOne, byte[] imageTwo)
        {

            IList<DetectedFace> imageOneFaces = await GetFaceFromByteArray(imageOne);
            IList<DetectedFace> imageTwoFaces = await GetFaceFromByteArray(imageTwo);

            if (imageOneFaces?.Count == 0 || imageTwoFaces.Count == 0)
                return double.MinValue;

            DetectedFace faceOne = imageOneFaces.FirstOrDefault();
            DetectedFace faceTwo = imageTwoFaces.FirstOrDefault();

            if (faceOne?.FaceId == null || faceTwo?.FaceId == null)
                return double.MinValue;

            VerifyResult result = await VerifyFaces(faceOne, faceTwo);

            return result?.Confidence ?? double.MinValue;
        }

        private async Task<IList<DetectedFace>> GetFaceFromByteArray(byte[] imageArray)
        {
            using (Stream ms = new MemoryStream(imageArray))
            {
                return await GetFaceFromStream(ms);
            }
        }

        private async Task<IList<DetectedFace>> GetFaceFromStream(Stream image)
        {
            return await _faceClient.Face.DetectWithStreamAsync(image,
                recognitionModel: RecognitionModel.Recognition03,
                returnFaceLandmarks: false);
        }

        private async Task<VerifyResult> VerifyFaces(DetectedFace faceOne, DetectedFace faceTwo)
        {
            return await _faceClient.Face.VerifyFaceToFaceAsync(faceOne.FaceId.Value, faceTwo.FaceId.Value);
        }
    }
}
