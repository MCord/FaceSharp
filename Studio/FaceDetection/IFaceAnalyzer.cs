namespace Studio
{
    using System.Collections.Generic;
    using System.Drawing;

    interface IFaceAnalyzer
    {
        IEnumerable<FacialFeature> ExtractFacialFutures(Image image);
    }
}
