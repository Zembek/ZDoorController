using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Microsoft.Extensions.Configuration;
using ZDoorController.Interface.App.Interfaces;

namespace ZDoorController.Interface.App.Services.FaceRecognition
{
    internal class AwsFaceRecognitionService : IFaceRecognitionService
    {
        private readonly AmazonRekognitionClient _rekognitionClient;
        private readonly float _similarityThreshold;

        public AwsFaceRecognitionService(IConfiguration configuration)
        {
            _rekognitionClient = new AmazonRekognitionClient();
            _similarityThreshold = configuration.GetValue<float>("ApplicationConfigs:SimilarityThreshold");
        }

        public bool VerifyFacesConfidence(byte[] imageOne, byte[] imageTwo)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> VerifyFacesConfidenceAsync(byte[] validImage, byte[] imageToCheck)
        {
            Image imageSource = CreateImageObj(validImage);
            Image newImage = CreateImageObj(imageToCheck);

            CompareFacesRequest compareFacesRequest = new CompareFacesRequest()
            {
                SourceImage = imageSource,
                TargetImage = newImage,
                SimilarityThreshold = _similarityThreshold
            };

            CompareFacesResponse compareFacesResponse = await _rekognitionClient.CompareFacesAsync(compareFacesRequest);


            return compareFacesResponse.FaceMatches.Count == 0;
        }

        private Image CreateImageObj(byte[] image) => new Image
        {
            Bytes = new MemoryStream(image)
        };
    }
}
