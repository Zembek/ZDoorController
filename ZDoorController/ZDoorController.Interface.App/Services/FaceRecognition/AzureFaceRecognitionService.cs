using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Configuration;
using ZDoorController.Interface.App.Interfaces;

namespace ZDoorController.Interface.App.Services.FaceRecognition
{
    public class AzureFaceRecognitionService : IFaceRecognitionService
    {
        private readonly FaceClient _faceClient;
        private readonly FaceRecognitionConfiguration _faceConfig;
        private readonly float _similarityThreshold;

        public AzureFaceRecognitionService(IConfiguration configuration)
        {
            _faceConfig = configuration.GetSection("AzureCognitiveService:FaceService").Get<FaceRecognitionConfiguration>();
            _faceClient = new FaceClient(new ApiKeyServiceClientCredentials(_faceConfig.Key)) { Endpoint = _faceConfig.Endpoint };
            _similarityThreshold = configuration.GetValue<float>("ApplicationConfigs:SimilarityThreshold");
        }

        public bool VerifyFacesConfidence(byte[] imageOne, byte[] imageTwo)
        {
            var task = Task.Run(async () => await VerifyFacesConfidenceAsync(imageOne, imageTwo));
            return task.Result;
        }

        public async Task<bool> VerifyFacesConfidenceAsync(byte[] imageOne, byte[] imageTwo)
        {

            IList<DetectedFace> imageOneFaces = await GetFaceFromByteArray(imageOne);
            IList<DetectedFace> imageTwoFaces = await GetFaceFromByteArray(imageTwo);

            if (imageOneFaces?.Count == 0 || imageTwoFaces.Count == 0)
                return false;

            DetectedFace faceOne = imageOneFaces.FirstOrDefault();
            DetectedFace faceTwo = imageTwoFaces.FirstOrDefault();

            if (faceOne?.FaceId == null || faceTwo?.FaceId == null)
                return false;

            VerifyResult result = await VerifyFaces(faceOne, faceTwo);

            return result?.Confidence >= _similarityThreshold;
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
