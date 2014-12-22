namespace Studio
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Media.Imaging;

    public interface INormalization
    {
        BitmapImage Apply(BitmapImage image, List<FacialFeature> features);
    }
}