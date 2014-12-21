namespace Studio
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Luxand;

    internal class LuxlandRecognitionEngine : IFaceAnalyzer
    {
        public LuxlandRecognitionEngine()
        {
            Assert(FSDK.ActivateLibrary(@"Vb0WOZCv5EEleDfQtp7PVQizy6dtlaQfvkiBBHSynzcZDOF038lr/uF8mEC2O+BPivfsWwubTdUDBklKhEI22sZpsQPxusxhJHT9aIqsUtS+Oufq/hFKdipAS6eU2Jj5Ikkdob3d4CYF3ttFuaf/I/R4MTQVkEYxOJYMtefKyP8="));
            Assert(FSDK.InitializeLibrary());
        }


        private static void Assert(int returnCode)
        {
            if (returnCode != 0)
            {
                throw new Exception("Luxland sdk failed with error:" + returnCode);
            }
        }

        private string GetPointName(int id)
        {
            return string.Format("{0}", (FSDK.FacialFeatures) id);
        }

        public IEnumerable<FacialFeature> ExtractFacialFutures(Image image)
        {
            var img = 0;
            Assert(FSDK.LoadImageFromCLRImage(ref img, image));

            FSDK.TPoint[] points;
            FSDK.DetectFacialFeatures(img, out points);
            FSDK.FreeImage(img);

            return points.Select((t, i) => new FacialFeature(i, GetPointName(i), new Point(t.x, t.y)));
        }
    }
}