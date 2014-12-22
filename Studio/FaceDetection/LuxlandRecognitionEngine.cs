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
            Assert(FSDK.ActivateLibrary(@"jMmpgjZQzxx+fFWCk+G9EiSKGw8H62oGHNemGuRsGaaTfSPqJncN0KTPlu3WnVh9mqrWJWp2E2pAbT+L7licvpIQTW12R17rto/LNUPf+B8eMuVYhvJC4T5A74f4DyoQhWxwgNkLzjgz4RtrvHD1sUZ+M3F/WnMQXP0xZ0Z6d+U="));
            Assert(FSDK.InitializeLibrary());
        }


        private static void Assert(int returnCode)
        {
            if (returnCode == FSDK.FSDKE_NOT_ACTIVATED)
            {
                throw new Exception("You Luxand license has expired.");
            }

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

            if (points == null)
            {
                return Enumerable.Empty<FacialFeature>();
            }

            return points.Select((t, i) => new FacialFeature(i, GetPointName(i), new Point(t.x, t.y)));
        }
    }
}