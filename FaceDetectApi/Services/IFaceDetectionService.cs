using FaceDetectApi.Models;
using FaceRecognitionDotNet;

namespace FaceDetectApi.Services
{
    public interface IFaceDetectionService
    {
        Task<FaceDetection> DetectFaceEmguCV(IFormFile file);
        Task<string> DetectFaceHog(IFormFile file);
        Task<string> DetectFaceCNNMmod(IFormFile file);
        Task<Location> DetectFaceRecognition(IFormFile file);
    }
}
