using System;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace WarpImage
{
    public class MovingLeastSquaresRectGrid
    {
        public int ImgH;
        public int ImgW;
        private int[] IndexX;
        private int[] IndexY;
        // Datastructures nPoint, p, q         

        // use euclidian distance fixed in code  
        // static double alpha = 2.0; 

        public int nPoint;
        public PointF[] p;
        public PointF[] q;
        // We will compute v at points (ix,iy) if (vXCompute[ix] && vYCompute[iy])
        private bool[] vXCompute;
        private bool[] vYCompute;

        private void SetXYCompute(int ImgH, int ImgW, int stepSize)
        {
            // Make sure that all borders vXCompute[] and vYCompute[] is true!!
            // We will use this asumption later...
            vXCompute = new bool[ImgW];
            foreach (var i in IndexX) vXCompute[i] = false;
            for (var i = 0; (i < ImgW); i = i + stepSize)
            {
                vXCompute[i] = true;
            }
            vXCompute[ImgW - 1] = true;

            vYCompute = new bool[ImgH];
            foreach (var i in IndexY) vYCompute[i] = false;
            for (var i = 0; (i < ImgH); i = i + stepSize)
            {
                vYCompute[i] = true;
            }
            vYCompute[ImgH - 1] = true;
        }

        public void InitBeforeComputation(PointF[] _p, PointF[] _q, int _ImgH, int _ImgW, int stepSize = 10)
        {
            p = _p;
            q = _q;
            nPoint = p.Length;

            ImgH = _ImgH;
            ImgW = _ImgW;

            IndexX = NewIntIndexArray(ImgW);
            IndexY = NewIntIndexArray(ImgH);

            // Specifies vXCompute and vYCompute. These specify where MeshPointsV are computed.
            SetXYCompute(_ImgH, _ImgW, stepSize);
        }

        public WriteableBitmap WarpImage(PixelColor[,] Pixels, double DpiX, double DpiY)
        {
            PixelColor[,] PixelsOut;
            PixelsOut = new PixelColor[ImgH, ImgW];

            var outsideImage = new PixelColor();
            outsideImage.Alpha = 100;
            outsideImage.Blue = 100;
            outsideImage.Red = 0;
            outsideImage.Green = 100;

            // .. Used for foreach loops, not so efficient, less error prone? 


            // For each pixel (ix,iy) of result image:
            // - Compute backtransformation coordinates (x,y) in original image 
            // - Use surrounding pixels (Nearest Neighbour, Linear, quadratic interpolation) to compute new RGB

            // - At (ix,iy): if (vYCompute[iy]&&vXCompute[ix]) compute (ix,iy)->(x,y) using MeshPointV
            //   Interpolate for rest of points

            // - Compute MeshPoints+Interpolate foreach ix within a row using ComputeVAndInterpolateRow(int iy)
            // - Interpolate between rows with coordinates computed by MeshPointV by 
            //   using InterpolateVRow(iy,iy1,iy2,Row1,Row2)

            var xyRow1 = new PointF[ImgW];
            var xyRow2 = new PointF[ImgW];
            var xyRow = new PointF[ImgW];
            xyRow2 = ComputeVAndInterpolateXYRow(0);
            var iyRow1 = 0;
            var iyRow2 = 0;

            foreach (var iy in IndexY)
            {
                if (vYCompute[iy])
                {
                    // We reached a Row to compute V's, Row2, this becomes now Row1
                    iyRow1 = iyRow2;
                    foreach (var ix in IndexX) xyRow1[ix] = xyRow2[ix];

                    // Find and set the next Row2
                    var Row2Found = false;
                    while (!Row2Found)
                    {
                        iyRow2++;
                        if ((iyRow2 >= ImgH) || vYCompute[iyRow2]) Row2Found = true;
                        ;
                    }

                    // We compute a new Row.
                    // (x,y) coordinates are computed using MeshPointV where xComputeV[ix] true;
                    // rest of the row is interpolated
                    if ((iyRow2 < ImgH)) xyRow2 = ComputeVAndInterpolateXYRow(iyRow2);
                }

                if (iy == 612)
                {
                    Console.WriteLine("ss");
                }
                // Interpolate the current xyRow using known xyRow1 and xyRow2
                xyRow = InterpolateXYRow(iy, iyRow1, iyRow2, xyRow1, xyRow2);

                // Compute the pixel values at Points of original value
                foreach (var ix in IndexX)
                {
                    // Given coordinates ix,iy get back transformation coodinates (x,y) in coordOrg
                    var coordOrg = xyRow[ix];

                    // "get pixel value using coordinate":
                    // Transformed coordinate double, can be in between int pixel coordinates:
                    // Nearest Neighbour, linear interpolation using 4 nearest integer pixels, Gaussfilter using double
                    // * We use NN solution for now

                    // Round coordOrg to Nearest Neighbour coordinates ixOrg, iyOrg
                    // Alternatives: linear, qubic interpolation

                    var ixOrg = (int) Math.Round(coordOrg.X, MidpointRounding.AwayFromZero);
                    var iyOrg = (int) Math.Round(coordOrg.Y, MidpointRounding.AwayFromZero);

                    // And place this value in PixelsOut[iy,ix]

                    

                    if ((ixOrg < 0) || (ixOrg >= ImgW) || (iyOrg < 0) || (iyOrg >= ImgH))
                    {
                        PixelsOut[iy, ix] = outsideImage;
                        
                    }
                    else
                        PixelsOut[iy, ix] = Pixels[iyOrg, ixOrg];
                }
            }

            var wBitmap = Image2PixelArray.BitmapSourceFromPixelsTopLeft(PixelsOut, DpiX, DpiY);
            return wBitmap;
        }

        private int[] NewIntIndexArray(int l)
        {
            var result = new int[l];
            for (var i = 0; i < l; i++) result[i] = i;
            return result;
        }

        private PointF[] ComputeVAndInterpolateXYRow(int iy)
        {
            var result = new PointF[ImgW];

            var ix1 = 0;
            var ix2 = 0;
            var vX1 = new PointF();
            var vX2 = new PointF();

            // Compute points V specified by vXCompute
            foreach (var ix in IndexX)
                if (vXCompute[ix])
                {
                    var pnt = new Point(ix, iy);

                    // Compute parameters using a meshpoint
                    //  Backtransformation by switching parameters p,q
                    var v = new MeshPointV();
                    v.ComputeTransformationParameters(pnt.X, pnt.Y, nPoint, q, p);
                    result[ix] = v.TransformL(pnt);
                }

            // Interpolate points ix between coordinates computed by v ix values: ix1 and ix2

            ix2 = 0;
            vX2 = result[0]; // we know that borders are computed
            foreach (var ix in IndexX)
            {
                if (vXCompute[ix])
                {
                    // current ix2 reached, ix1<-ix2, find new ix2
                    ix1 = ix2;
                    vX1 = vX2;

                    var X2Found = false;
                    ix2 = ix;
                    while (!X2Found)
                    {
                        ix2++;
                        if ((ix2 >= ImgW) || vXCompute[ix2]) X2Found = true;
                        ;
                    }
                    if (ix2 < ImgW) vX2 = result[ix2];
                }
                else
                // interploate beween computed coordinates vX1 and vX2
                // if ((ix!=ix1)&&(ix!=ix2))
                {
                    // note! (double) needed to switch from int to double computing!!
                    var delta = (ix - ix1)/(double) (ix2 - ix1);
                    var xInterpol = (float) (vX1.X*(1.0 - delta) + vX2.X*delta);
                    var yInterpol = (float) (vX1.Y*(1.0 - delta) + vX2.Y*delta);

                    result[ix] = new PointF(xInterpol, yInterpol);
                }
            }

            return result;
        }

        private PointF[] InterpolateXYRow(int iy, int iy1, int iy2, PointF[] xyRow1, PointF[] xyRow2)
        {
            var result = new PointF[ImgW];

            // note! (double) needed to switch from int to double computing!!
            var delta = (iy - iy1)/(double) (iy2 - iy1);

            foreach (var ix in IndexX)
            {
                // refinement: (iy==iy1) return xyRow1, (iy==iy2) return xyRow2

                float xInterpol = (float) (xyRow1[ix].X*(1.0 - delta) + xyRow2[ix].X*delta);
                float yInterpol = (float) (xyRow1[ix].Y*(1.0 - delta) + xyRow2[ix].Y*delta);

                result[ix] = new PointF(xInterpol, yInterpol);
            }
            return result;
        }

        // Simple but stupid interpolation for testing
        // (1-delta) + delta linear interpolation using bytes is is an art I know nothing about
        private PixelColor BilinearInterpolation
            (PixelColor[,] PixelsOrg, PointF coordOrg)
        {
            // get pixl, set value ousidePixel
            // Note set Alpha=255 or you will be see transparant pixels ...
            var pixl = new PixelColor();
            pixl.Alpha = 0;
            pixl.Blue = 100;
            pixl.Red = 100;
            pixl.Green = 100;

            // We return background on rigorous border test, some cases can be maped more subtle on border 
            var floorX = (int) Math.Floor(coordOrg.X);
            if ((floorX < 0) || (floorX >= ImgW)) return pixl;

            var floorY = (int) Math.Floor(coordOrg.Y);
            if ((floorY < 0) || (floorY >= ImgH)) return pixl;

            var ceilX = floorX + 1;
            if (ceilX >= ImgW) return pixl;

            var ceilY = floorY + 1;
            if (ceilY >= ImgH) return pixl;

            // coodinates [iy,ix]
            var pix1 = PixelsOrg[floorY, floorX];
            var pix2 = PixelsOrg[floorY, ceilX];
            var pix3 = PixelsOrg[ceilY, floorX];
            var pix4 = PixelsOrg[ceilY, ceilX];


            // delta's = fractions 0..1
            // (1-delta)  .. + delta; 1 -delta*  +delta*
            var deltaX = coordOrg.X - floorX;
            var deltaY = coordOrg.Y - floorY;

            // just a try...
            //pixl.ColorBGRA = LinInterpolFast(pix1.ColorBGRA, pix2.ColorBGRA, pix3.ColorBGRA, pix4.ColorBGRA
            //                     , (int)deltaX * 255, (int)deltaY * 255);
            //pixl.Alpha = 255;
            //return pixl;


            double cx1, cx2, c;

            pixl.Alpha = 255; // don't forget that...
            // Blue
            cx1 = (1 - deltaX)*pix1.Blue + deltaX*pix2.Blue;
            cx2 = (1 - deltaX)*pix3.Blue + deltaX*pix4.Blue;
            c = (1 - deltaY)*cx1 + deltaY*cx2;
            pixl.Blue = (byte) c;

            // Green
            cx1 = (1 - deltaX)*pix1.Green + deltaX*pix2.Green;
            cx2 = (1 - deltaX)*pix3.Green + deltaX*pix4.Green;
            c = (1 - deltaY)*cx1 + deltaY*cx2;
            pixl.Green = (byte) c;
            // Red
            cx1 = (1 - deltaX)*pix1.Red + deltaX*pix2.Red;
            cx2 = (1 - deltaX)*pix3.Red + deltaX*pix4.Red;
            c = (1 - deltaY)*cx1 + deltaY*cx2;
            pixl.Red = (byte) c;

            return pixl;
        }

        // fast rbg bytes operations, approximation
        // lin interpolation variants, use tables for multiplication 0..255
        // fixedpt_lerp(t, a, b) = a + ((t * (b-a)) >> 8)
        // http://fgiesen.wordpress.com/2012/08/15/linear-interpolation-past-present-and-future/

        // Use of bytes, shifts >> , masks, next LinInterpolFast (I did not check the code) from: 
        // http://www.java-gaming.org/index.php?topic=22121.0 
        // to do: add Alpha=255
        public UInt32 LinInterpolFast(UInt32 c1, UInt32 c2, UInt32 c3, UInt32 c4, int bX, int bY)
        {
            var f24 = (bX*bY) >> 8;
            var f23 = bX - f24;
            var f14 = bY - f24;
            var f13 = ((256 - bX)*(256 - bY)) >> 8; // this one can be computed faster

            return (UInt32)
                ((((c1 & 0xFF00FF)*f13 + (c2 & 0xFF00FF)*f23 + (c3 & 0xFF00FF)*f14 + (c4 & 0xFF00FF)*f24) & 0xFF00FF00) |
                 (((c1 & 0x00FF00)*f13 + (c2 & 0x00FF00)*f23 + (c3 & 0x00FF00)*f14 + (c4 & 0x00FF00)*f24) & 0x00FF0000)) >>
                   8;
        }
    }
}