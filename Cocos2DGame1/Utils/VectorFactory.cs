using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Windows.Forms;

namespace VenLight.Utils
{
    static class VectorFactory
    {
        private const double dAngel45=Math.PI/2;

        public static Point GetPointFromRound(Point center,double radius, int count, int position)
        {
            double angle = (2 * Math.PI * position / count) + dAngel45;
            int dx = (int)Math.Round(radius * Math.Cos(angle), 0);
            int dy = (int)Math.Round(radius * Math.Sin(angle), 0);
            return new Point(center.X-dx, center.Y-dy);
        }
        //--- возвращает вектор движения курсора ------------------------------------------------------------
        public static Point RetVectPoint(float x0, float y0, float x1, float y1, Point MIN, Point MAX)
        {
            Point p; p.X = -1; p.Y = -1;
            if (y1 == y0) y1++;
            float d = (x1 - x0) / (y1 - y0);
            int LIM;
            if (y1 < y0) LIM = MIN.Y; else LIM = MAX.Y;
            int x = (int)(x1 + (LIM - y1) * d);
            if ((x > MIN.X) && (x < MAX.X)) { p.X = x; p.Y = LIM; return p; }
            if (x1 < x0) LIM = MIN.X; else LIM = MAX.X;
            p.Y = (int)(y1 + (LIM - x1) / d);
            p.X = LIM;
            return p;
        }
        //--- возвращает угол вектора курсора -------------------------------------------------------
        public static double RetAngel(float x0, float y0, float x1, float y1)
        {
            float pi = (float)3.1415926;
            if (x1 == x0) if (y1 > y0) return pi / 2; else return -pi / 2;
            float res = (float)Math.Atan((y1 - y0) / (x1 - x0));
            if (x1 < x0) res += pi;
            return res;
        }
    }
}
