using DlibDotNet;
using DlibDotNet.Dnn;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using FaceDetectApi.Models;
using FaceRecognitionDotNet.Extensions;
using FaceRecognitionDotNet;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using Image = FaceDetectApi.Models.Image;
using DlibDotNet.Extensions;
using Emgu.CV.Reg;
using System.Drawing.Drawing2D;

namespace FaceDetectApi.Services
{
    public class DetectFaceService : IFaceDetectionService
    {
        private readonly CascadeClassifier _classifier;
        public DetectFaceService()
        {
            _classifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
        }

        public async Task<FaceDetection> DetectFaceEmguCV(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var bytes = stream.ToArray();
                var localFileName = "inputFace.jpg";
                if (File.Exists(localFileName) is false)
                {
                    File.Create(localFileName).Dispose();
                }
                await File.WriteAllBytesAsync(localFileName, bytes);
                Image<Bgr, Byte> grayFrame = new Image<Bgr, byte>(localFileName);

                // Imagem people01
                //System.Drawing.Rectangle[] faces = _classifier.DetectMultiScale(grayFrame, 1.1, 5, new System.Drawing.Size(60, 60));

                // Imagem people02
                System.Drawing.Rectangle[] faces = _classifier.DetectMultiScale(grayFrame, 1.1, 5, new System.Drawing.Size(34, 34), new System.Drawing.Size(80, 80));

                //if (!faces.Any())
                //{
                //    //return string.Empty;                    
                //    Mat img = grayFrame.Mat;
                //    CvInvoke.Resize(img, img, new Size(800, 600), 0, 0, Inter.Linear);
                //    faces = _classifier.DetectMultiScale(img, 1.00009, 5, new Size(20,20), new Size(30, 30));
                //}

                foreach (var face in faces)
                {
                    grayFrame.Draw(face, new Bgr(255, 255, 0), 3);
                }

                var result = grayFrame.ToJpegData();
                var resultFileName = "faceResult.jpg";

                if (File.Exists(resultFileName) is false)
                {
                    File.Create(resultFileName).Dispose();
                }

                await File.WriteAllBytesAsync(resultFileName, result);

                //return $"{resultFileName}-{faces.Length}";
                return new FaceDetection(faces, faces.Length, resultFileName);
            }
        }

        public async Task<string> DetectFaceHog(IFormFile file)
        {
            string localFileName;
            var resultFileName = "faceResult.jpg";
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var bytes = stream.ToArray();
                localFileName = "inputFace.jpg";
                if (File.Exists(localFileName) is false)
                {
                    File.Create(localFileName).Dispose();
                }
                await File.WriteAllBytesAsync(localFileName, bytes);
                using (var detector = Dlib.GetFrontalFaceDetector())
                using (var img = Dlib.LoadImage<RgbPixel>(localFileName))
                {
                    Dlib.PyramidUp(img);

                    var faces = detector.Operator(img, 0.99);

                    foreach (var face in faces)
                    {
                        Dlib.DrawRectangle(img, face, color: new RgbPixel(0, 255, 255), thickness: 4);
                    }

                    if (File.Exists(resultFileName) is false)
                    {
                        File.Create(resultFileName).Dispose();
                    }

                    Dlib.SaveJpeg(img, resultFileName);

                    return $"{resultFileName}-{faces.Length}"; ;
                }
            }
        }

        public async Task<string> DetectFaceCNNMmod(IFormFile file)
        {
            string localFileName;
            var resultFileName = "faceResult.jpg";
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var fileNameByte = stream.ToArray();
                //var fileNameByte = Dlib.Encoding.GetBytes(filename);

                localFileName = "inputFace.jpg";
                if (File.Exists(localFileName) is false)
                {
                    File.Create(localFileName).Dispose();
                }
                await File.WriteAllBytesAsync(localFileName, fileNameByte);

                //var imageCV = new Image<Rgb, byte>(localFileName);
                //var matrix = imageCV.Mat;
                //var array = new byte[matrix.Width * matrix.Height * matrix.ElementSize];
                //matrix.CopyTo(array);

                //using (var detector = LossMmod.Deserialize(fileNameByte, 1))
                var directory = Path.GetFullPath("Recursos");
                var fullPath = Path.Combine(directory, "mmod_human_face_detector.dat");
                using (var detector = LossMmod.Deserialize(fullPath))
                using (var matrix = Dlib.LoadImageAsMatrix<RgbPixel>(localFileName))
                //using (var bitmap = (Bitmap)System.Drawing.Image.FromFile(localFileName))
                {                    
                    Dlib.PyramidUp(matrix);
                    //var gray = ToGrayscale(bitmap).ToMatrix<RgbPixel>();

                    //CvInvoke.CvtColor(img, imgGrey, ColorConversion.Bgr2Gray);

                    //IEnumerable<MModRect> Detectfaces = new List<MModRect>();
                   
                    var faces = detector.Operator(matrix).SelectMany(x => x);

                    //foreach (var faces in labels)
                    //{
                    //    Detectfaces.ToList().AddRange(faces);
                        foreach (var face in faces)
                        {
                            Dlib.DrawRectangle(matrix, face, color: new RgbPixel(0, 255, 255), thickness: 3);
                        }

                    //}

                    if (File.Exists(resultFileName) is false)
                    {
                        File.Create(resultFileName).Dispose();
                    }

                    Dlib.SaveJpeg(matrix, resultFileName);

                    return $"{resultFileName}-{faces.Count()}";
                }
            }
        }

        public async Task<Location> DetectFaceRecognition(IFormFile file)
        {
            //var imageFile = "DianaPrincessOfWales_1997_36.jpg";

            string localFileName;
            var resultFileName = "faceResult.jpg";
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                var fileNameByte = stream.ToArray();
                //var fileNameByte = Dlib.Encoding.GetBytes(filename);

                localFileName = "inputFace.jpg";
                if (File.Exists(localFileName) is false)
                {
                    File.Create(localFileName).Dispose();
                }
                await File.WriteAllBytesAsync(localFileName, fileNameByte);
            }

            var directory = Path.GetFullPath("Recursos");
            //var fullPath = Path.Combine(directory);
            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"Please check whether model directory '{directory}' exists");
                Directory.CreateDirectory(directory);
            }

            using (var fr = FaceRecognition.Create(directory))
            using (var image = FaceRecognition.LoadImageFile(localFileName, mode: Mode.Greyscale))
            using (var bitmap = (Bitmap)System.Drawing.Image.FromFile(localFileName))
            using (var g = Graphics.FromImage(bitmap))
            {                
                var boxes = fr.FaceLocations(image, model: Model.Cnn);

                using (var p = new Pen(Color.Red, bitmap.Width / 200f))
                    foreach (var box in boxes)
                    {
                        g.DrawRectangle(p, box.Left, box.Top, box.Right - box.Left, box.Bottom - box.Top);
                    }                

                // load custom estimator
                //using (var ageEstimator = new SimpleAgeEstimator(Path.Combine(directory, "adience-age-network.dat")))
                //using (var genderEstimator = new SimpleGenderEstimator(Path.Combine(directory, "utkface-gender-network.dat")))
                //{
                //    fr.CustomAgeEstimator = ageEstimator;
                //    fr.CustomGenderEstimator = genderEstimator;

                //    var ageRange = ageEstimator.Groups.Select(range => $"({range.Start}, {range.End})").ToArray();
                //    var age = ageRange[fr.PredictAge(image, box)];
                //    var gender = fr.PredictGender(image, box);

                //    var agePos = new PointF(box.Left + 10, box.Top + 10);
                //    var genderPos = new PointF(box.Left + 10, box.Bottom - 50);
                //    g.DrawString(gender.ToString(), SystemFonts.CaptionFont, Brushes.Blue, agePos);
                //    g.DrawString(age, SystemFonts.CaptionFont, Brushes.Green, genderPos);

                    bitmap.Save(resultFileName);

                    return boxes.FirstOrDefault();
                //}
            }
        }

        private Bitmap ToGrayscale(Bitmap bitmap)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);

                    int r = c.R;
                    int g = c.G;
                    int b = c.B;
                    int avg = (r + g + b) / 3;
                    bitmap.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                }
            }

            return bitmap;
        }
    }
}
