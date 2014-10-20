namespace Studio
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    interface IFaceAnalyzer
    {
        IEnumerable<IFacialFeature> ExtractFacialFeatures(ImageSource image);
    }

    interface IFacialFeature
    {
        int Id { get; }
        string Name { get; }
        Point Location { get; }
    }
}
