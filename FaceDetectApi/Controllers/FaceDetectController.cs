using FaceDetectApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FaceDetectApi.Controllers
{
    [Route("api/faces")]
    [ApiController]
    public class FaceDetectController : ControllerBase
    {
        [HttpPost("detect-face-recognition")]
        public async Task<IActionResult> DetectFaceRecognition(IFormFile file, [FromServices] IFaceDetectionService service)
        {
            var result = await service.DetectFaceRecognition(file);
            if (result == null) return NotFound();            
            return Ok(new { location = result });
        }

        [HttpPost("detect-dnn")]
        public async Task<IActionResult> DetectFaceCNNMmod(IFormFile file, [FromServices] IFaceDetectionService service)
        {
            var result = await service.DetectFaceCNNMmod(file);
            if (string.IsNullOrEmpty(result)) return NotFound();
            var collection = result.Split('-');
            return Ok(new { faceFileName = collection[0], quantidade = collection[1] });
        }

        [HttpPost("detect-hog")]
        public async Task<IActionResult> DetectFaceHog(IFormFile file, [FromServices] IFaceDetectionService service)
        {
            var result = await service.DetectFaceHog(file);
            if (string.IsNullOrEmpty(result)) return NotFound();
            var collection = result.Split('-');
            return Ok(new { faceFileName = collection[0], quantidade = collection[1] });
        }


        [HttpPost("detect-emgucv")]
        public async Task<IActionResult> DetectFaceEmguCV(IFormFile file, [FromServices] IFaceDetectionService service)
        {
            var result = await service.DetectFaceEmguCV(file);
            if(result == null) return NotFound();
            return Ok(new { faceFileName = result.FileName, quantidade = result.NumberFaces, faces = result.Size });
            //if(string.IsNullOrEmpty(result)) return NotFound();
            //var collection = result.Split('-');
            //return Ok(new { faceFileName = collection[0], quantidade = collection[1] });
        }

        [HttpGet("{fileName}")]
        public IActionResult GetFaceResult([FromRoute] string fileName)
        {
            if (System.IO.File.Exists(fileName) is false) 
            {
                return NotFound();
            }

            var bytes = System.IO.File.ReadAllBytes(fileName);
            return File(bytes, "image/jpeg");
        }
    }
}
