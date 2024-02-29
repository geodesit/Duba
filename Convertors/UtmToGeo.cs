using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DubaProject.Convertors
{
    internal class UtmToGeo
    {
        public static  double[] convertUTMToGeo(double east, double north)
        {
            double[] geo = new double[2];
            double lam0 = (33 * Math.PI) / 180;
            double k0 = 0.9996;
            double e = 0.08181919084;
            double a = 6378137;
            double north0 = 0;
            double east0 = 500000;
            double sm0 = 0;

            double ca = 1 + (3 * Math.Pow(e, 2)) / 4 +
                (45 * Math.Pow(e, 4)) / 64 +
                (175 * Math.Pow(e, 6)) / 256 +
                (11025 * Math.Pow(e, 8)) / 16384 +
                (43659 * Math.Pow(e, 10)) / 65536;

            double cb = (double)1 / 2 *
                ((3 * Math.Pow(e, 2)) / 4 +
                (15 * Math.Pow(e, 4)) / 16 +
                (525 * Math.Pow(e, 6)) / 512 +
                (2205 * Math.Pow(e, 8)) / 2048 +
                (72765 * Math.Pow(e, 10)) / 65536);

            double cc = (double)1 / 4 *
                ((15 * Math.Pow(e, 4)) / 64 +
                (105 * Math.Pow(e, 6)) / 256 +
                (2205 * Math.Pow(e, 8)) / 4096 +
                (10395 * Math.Pow(e, 10)) / 16384);

            double cd = (double)1 / 6 *
                ((35 * Math.Pow(e, 6)) / 512 +
                (315 * Math.Pow(e, 8)) / 2048 +
                (31185 * Math.Pow(e, 10)) / 131072);

            double phi1 = 0;

            for (int i = 0; i < 9; i++)
            {
                phi1 = (sm0 + north - north0) / (k0 * a * (1 - Math.Pow(e, 2)) * ca) +
                    (cb * Math.Sin(2 * phi1) - cc * Math.Sin(4 * phi1) + cd * Math.Sin(6 * phi1)) / ca;
            }

            double w = Math.Sqrt(1 - Math.Pow(e, 2) * Math.Pow(Math.Sin(phi1), 2));
            double n = (k0 * a) / w;
            double m = (k0 * a * (1 - Math.Pow(e, 2))) / Math.Pow(w, 3);
            double t = Math.Tan(phi1);
            double ni = (e / Math.Sqrt(1 - Math.Pow(e, 2))) * Math.Cos(phi1);
            double dEast = east - east0;

            // Latitude
            double phi = (180 / Math.PI) *
                (phi1 - (Math.Pow(dEast, 2) / (2 * m * n)) *
                t *
                (1 -
                (Math.Pow(dEast, 2) / (12 * Math.Pow(n, 2))) *
                (5 + 3 * Math.Pow(t, 2) + Math.Pow(ni, 2) - 9 * Math.Pow(t, 2) * Math.Pow(ni, 2)) +
                (Math.Pow(dEast, 4) / (360 * Math.Pow(n, 4))) *
                (61 + 90 * Math.Pow(t, 2) + 45 * Math.Pow(t, 4))));

            // Longtitude
            double lam = (180 / Math.PI) *
                (lam0 +
                (dEast / (n * Math.Cos(phi1))) *
                (1 -
                (Math.Pow(dEast, 2) / (6 * Math.Pow(n, 2))) *
                (1 + 2 * Math.Pow(t, 2) + Math.Pow(ni, 2)) +
                (Math.Pow(dEast, 4) / (120 * Math.Pow(n, 4))) *
                (5 +
                28 * Math.Pow(t, 2) +
                24 * Math.Pow(t, 4) +
                6 * Math.Pow(ni, 2) +
                8 * Math.Pow(ni, 2) * Math.Pow(t, 2))));

            geo[0] = phi;
            geo[1] = lam;

            return geo;
        }
    }
}
