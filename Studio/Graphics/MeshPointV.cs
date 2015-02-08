namespace Studio.Graphics
{
    using System;
    using System.Windows;

    public class MeshPointV
    {
        // Matrix representation M, 
        // Mt M == I, so m21=-m12 and m22 = m11
        public double m11, m12;
        // Note:
        // If all Meshpoints are computed and stored, consider making nPoint, p and q static
        // then in common for all intances of p,q
        public int nPoint;
        public Point[] p;
        private Point[] pHat;
        private double pStarX, pStarY;
        public Point[] q;
        private Point[] qHat;
        private double qStarX, qStarY;
        private double[] w;
        private double wSum;
        // Point v
        public double x, y;
        private readonly double ToSmallHeuristic = double.Epsilon;
        // If meshpoints are reused, reset by parametersComputed=false
        //public Boolean parametersComputed = false;
        public void ComputeTransformationParameters(double _x, double _y, int _nPoint, Point[] _p, Point[] _q)
        {
            // Given the complexity(=very simple) of datastructures I use here variables internal in class 
            // and internal functions without parameters
            x = _x;
            y = _y;

            // Note: nPoint, p, q defined static, we can remove following statements,
            // remove then from procedure call and call once for al GridPointsV once 
            nPoint = _nPoint;
            p = _p;
            q = _q;

            //if (parametersComputed) return;
            //parametersComputed = true;

            // Very simple procedures ...

            ComputeW();

            Compute_pStar_qStar();

            Compute_pHat_qHat();

            ComputeM();

            // who needs pHat and qHat now ...
            pHat = null;
            qHat = null;
        }

        private void ComputeW()
        {
            // Note: other weighting schemes seem possible
            w = new double[nPoint];
            wSum = 0;

            for (var i = 0; (i < nPoint); i++)
            {
                double dx, dy;
                dx = (p[i].X - x);
                dy = (p[i].Y - y);

                // Use implicit alpha=2
                // refinement: try do some other distances like Gaussian round stroke p[i]-q[i]
                // shortcut computation on 
                var d = dx*dx + dy*dy;


                // Note: how do we deal p[i]=v(x,y)?
                if (d < ToSmallHeuristic) d = ToSmallHeuristic;

                w[i] = 1.0/d;
                wSum = wSum + w[i];
            }
            if (wSum < ToSmallHeuristic) wSum = ToSmallHeuristic;
        }

        private void Compute_pStar_qStar()
        {
            pStarX = 0.0;
            pStarY = 0.0;
            for (var i = 0; (i < nPoint); i++)
            {
                pStarX += w[i]*p[i].X;
                pStarY += w[i]*p[i].Y;
            }
            pStarX = pStarX/wSum;
            pStarY = pStarY/wSum;

            qStarX = 0.0;
            qStarY = 0.0;
            for (var i = 0; (i < nPoint); i++)
            {
                qStarX += w[i]*q[i].X;
                qStarY += w[i]*q[i].Y;
            }
            qStarX = qStarX/wSum;
            qStarY = qStarY/wSum;
        }

        private void Compute_pHat_qHat()
        {
            pHat = new Point[nPoint];
            qHat = new Point[nPoint];

            for (var i = 0; (i < nPoint); i++)
            {
                pHat[i].X = p[i].X - pStarX;
                pHat[i].Y = p[i].Y - pStarY;

                qHat[i].X = q[i].X - qStarX;
                qHat[i].Y = q[i].Y - qStarY;
            }
        }

        // We do no precomputation but compute and normalise M directly
        private void ComputeM()
        {
            m11 = 0;
            m12 = 0;
            for (var i = 0; (i < nPoint); i++)
            {
                var a = pHat[i].X;
                var b = pHat[i].Y;
                var c = qHat[i].X;
                var d = qHat[i].Y;

                //                         a   b     c   d
                // M = MuNorm* Sum w[i] (        ) (       )    (eq. 6) from article
                //                         b  -a     d  -c

                m11 = m11 + w[i]*(a*c + b*d);
                m12 = m12 + w[i]*(a*d + b*-c);

                // m21 = m21 + b*c - a*d;  = -m12
                // m22 = m22 + b*d + a*c;  = m11
            }

            // Norm, Mt M = I so muNorm is
            var muNorm = Math.Sqrt(m11*m11 + m12*m12);

            // If we don't have a valid transformation, use Identity for transformation
            // (nPoint==1) ==> M = (a,b,c,d) == (0,0,0,0) or should be, but fails in muNorm test
            // Only errors found (blobs) nPoint==1, so for now extra test
            if ((muNorm < ToSmallHeuristic) || (nPoint == 1))
            {
                m12 = 0.0;
                m11 = 1.0;
            }
            else
            {
                m11 = m11/muNorm;
                m12 = m12/muNorm;
            }
        }

        // Transform a point using the transformation parameters of this MeshPoint
        public Point TransformL(Point p)
        {
            var pos = new Point();

            if (nPoint <= 0)
            {
                pos = p; // identety transf
                return pos;
            }

            var xt = (p.X - pStarX);
            var yt = (p.Y - pStarY);

            // Matrix M, pos.Y= m12*x+ m22*y =..; use m21=-m12 and m22 = m11
            pos.X = m11*xt - m12*yt + qStarX;
            pos.Y = m12*xt + m11*yt + qStarY;

            return pos;
        }
    }
}