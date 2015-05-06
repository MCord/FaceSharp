using System;

namespace WarpImage
{
    public static class Conversions
    {
        // refinement: input RGB value, conversion to r,g,b needed?? 
        public static PixelHsv RGBtoHSB(int red, int green, int blue)
        {
            // normalize red, green and blue values
            var r = (red/255.0F);
            var g = (green/255.0F);
            var b = (blue/255.0F);

            // conversion start
            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));

            var h = 0.0F;
            if (max == min)
            {
                // undefined, without this test no error becomes NAN
            }
            else if (max == r && g >= b)
            {
                h = 60*(g - b)/(max - min);
            }
            else if (max == r && g < b)
            {
                h = 60*(g - b)/(max - min) + 360;
            }
            else if (max == g)
            {
                h = 60*(b - r)/(max - min) + 120;
            }
            else if (max == b)
            {
                h = 60*(r - g)/(max - min) + 240;
            }

            var s = (float) ((max == 0) ? 0.0 : (1.0 - (min/max)));

            var result = new PixelHsv();
            result.H = h; // 0..360
            result.S = s; // 0..1
            result.V = max; // 0..1

            return result;
        }
    }
}