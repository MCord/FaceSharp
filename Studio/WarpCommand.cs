namespace Studio
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using Graphics;

    public class WarpCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var project = (Project)parameter;
            project.CurrentImage = WarpImage(project.OriginalImage,project).ToBitmapImage();
        }
        private WriteableBitmap WarpImage(BitmapImage originalImage, Project project)
        {
            var mlsAlgo = new MovingLeastSquaresRectGrid();

            var originalPoints = new[]
            {
                project.Features[0].Location,
                project.Features[24].Location,
                project.Features[23].Location,
                project.Features[38].Location,
                project.Features[27].Location,
                project.Features[37].Location,
                project.Features[35].Location,
                project.Features[28].Location,
                project.Features[36].Location,
                project.Features[29].Location,
                project.Features[30].Location,
            };
            var transformPoint = ScaleVector(0.5f, originalPoints).Select(p => new System.Windows.Point(p.X, p.Y)).ToArray();

            var pixels = Image2PixelArray.GetPixelsTopLeft(originalImage);

            int h = Image2PixelArray.GetH(pixels);
            int w = Image2PixelArray.GetW(pixels);


            mlsAlgo.InitBeforeComputation(originalPoints.Select(p => new System.Windows.Point(p.X, p.Y)).ToArray(), transformPoint, h, w, 20);

            return mlsAlgo.WarpImage(pixels, originalImage.DpiX, originalImage.DpiY);
        }

        private Point[] ScaleVector(float percent, params Point[] vector)
        {
            var average = Average(vector);

            var translated = vector.Select(v => new Point(v.X - average.X, v.Y - average.Y));
            var scaled = translated.Select(t => new PointF(t.X * percent, t.Y * percent));
            return scaled.Select(t => new Point((int)(t.X + average.X), (int)(t.Y + average.Y))).ToArray();
        }

        private Point Average(Point[] vector)
        {
            return new Point(vector.Sum(v => v.X) / vector.Length, vector.Sum(v => v.Y) / vector.Length);
        }


        public event EventHandler CanExecuteChanged;
    }
}