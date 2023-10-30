using System.Drawing;

namespace FaceDetectApi.Models
{
    public class FaceDetection
    {
        //public Rectangle[] Rectangles { get; set; }
        public int NumberFaces { get; set; }
        public string FileName { get; set; }
        public Size Size { get; set; }

        public FaceDetection(Rectangle[] rectangles, int numberFaces, string filename)
        {
            //Rectangles = new Rectangle[rectangles.Length];
            //Rectangles = rectangles;
            NumberFaces = numberFaces;
            FileName = filename;
            Size = new Size(rectangles);
        }
    }

    public class Size
    {
        public int MinSize { get; set; }
        public int MaxSize { get; set; }

        public Size(Rectangle[] rectangles)
        {
            MinSize = rectangles.Min(x => x.Width); 
            MaxSize = rectangles.Max(x => x.Width);
        }
    }
}
