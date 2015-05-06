using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WarpImage
{
    public static class Image2PixelArray
    {
        // NOTE: IMAGE CONVENTION PixelColor[iy,ix] with origin TOP LEFT, iy downwards, ix to the right
        // We can choose any array defined by the copy action in CopyPixelsTopLeft2 
        // However to transform back to a WritebleBitmap we use the PixelsTopLeft[iy,ix] convention 

        // Construct a PixelColorTopLeft[iy,ix] array for 2D processing the RGB Pixels
        public static PixelColor[,] GetPixelsTopLeftFromFilename(string _name, int DecodeHW = 0)
        {
            // First given filename to Bgra
            // refinement: Add bool UseBgra to class MyBitmap, move this part to that class 
            var image1 = new BitmapImage();
            image1.BeginInit();
            image1.CreateOptions = BitmapCreateOptions.IgnoreColorProfile; // (BitmapCreateOptions.DelayCreation |
            image1.CacheOption = BitmapCacheOption.OnLoad;
            image1.UriSource = new Uri(_name);

            // For histograms: make squared, smaller images
            if (DecodeHW != 0)
            {
                image1.DecodePixelWidth = DecodeHW;
                image1.DecodePixelHeight = DecodeHW;
            }
            image1.EndInit();

            // Use FormatConvertedBitmap for the encoding
            // .. we could do this in GetPixels           
            BitmapSource imageBgra = new FormatConvertedBitmap(image1, PixelFormats.Bgra32, null, 0);

            return GetPixelsTopLeft(imageBgra);
        }

        // from http://stackoverflow.com/questions/1176910/finding-specific-pixel-colors-of-a-bitmapimage


        //usage
        // var pixels = GetPixels(image);
        // if(pixels[7, 3].Red > 4)
        public static PixelColor[,] GetPixelsTopLeft(BitmapSource source)
        {
            // note: we have done this already.
            // I suppose that if you can read the image, you can convert it to Bgra32
            if (source.Format != PixelFormats.Bgra32)
                source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);

            var width = source.PixelWidth;
            var height = source.PixelHeight;
            var result = new PixelColor[height, width];

            //source.CopyPixels1(result, width * 4, 0);
            source.CopyPixelsTopLeft2(result);
            return result;
        }

        private static void CopyPixelsTopLeft2(this BitmapSource source, PixelColor[,] pixels)
        {
            var height = source.PixelHeight;
            var width = source.PixelWidth;

            var sizeInBytes = source.PixelWidth*((source.Format.BitsPerPixel + 7)/8);

            // Step 1. Dump to pixels to 1D array (CopyPixels works also on rectangles) ...
            var pixelsUint = new UInt32[height*width];
            source.CopyPixels(pixelsUint, sizeInBytes, 0);

            // Step 2. To 2D array if we want 2D indexing
            // Our convention in the copy action is here PixelsTopLeft
            // these steps minor processing time in comparision of reading image
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    pixels[y, x] = new PixelColor {ColorBGRA = pixelsUint[y*width + x]};
                }
            }
        }

        public static int GetH(PixelColor[,] PixelsTopLeft)
        {
            return PixelsTopLeft.GetLength(0);
        }

        public static int GetW(PixelColor[,] PixelsTopLeft)
        {
            return PixelsTopLeft.GetLength(1);
        }

        // Return a bitmapsource using a writable bitmap
        public static WriteableBitmap BitmapSourceFromPixelsTopLeft
            (PixelColor[,] PixelsTopLeft, double DpiX = 96.0, double DpiY = 96.0)
        {
            // Note: normally WritebleBitmap is used with lock, write to BackBuffer and unlock
            // Maybe for the way we use it here we could use newly created othr type of bitmap??

            var ImgH = PixelsTopLeft.GetLength(0);
            var ImgW = PixelsTopLeft.GetLength(1);

            // DPI must be same as original image, otherwise rescaling display can occur (NoScaling)
            var wBitmap = new WriteableBitmap(ImgW, ImgH, DpiX, DpiY, PixelFormats.Bgra32, null);

            // 1) copy PixelsOut to bitmap
            // qq to do: ImgW or ImgW-1 in rect??; stride=0 seems to work ...
            var sourceRect = new Int32Rect(0, 0, ImgW, ImgH);

            wBitmap.WritePixels(sourceRect, PixelsTopLeft, ImgW*4, 0);

            // 2) set Image1.Source=wBitmap; e.g. handle change source in Bitmap.
            return wBitmap;
        }
    }
}