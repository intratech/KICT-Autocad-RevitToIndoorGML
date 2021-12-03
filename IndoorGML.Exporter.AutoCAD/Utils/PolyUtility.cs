using Autodesk.AutoCAD.DatabaseServices;
using Point3DIntra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.AutoCAD.Utils
{
    public static class PolyUtility
    {
        public static bool PointInPolygon(Vector3 point,Vector3[] poly)
        {
            double X = point.x;
            double Y = point.y;

            // Get the angle between the point and the
            // first and last vertices.
            int max_point = poly.Length - 1;
            var p = poly[max_point];
            var p0 = poly[0];
            double total_angle = GetAngle(p.x, p.y, X, Y, p0.x, p0.y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++)
            {
                p = poly[i];
                var p1 = poly[i + 1];
                total_angle += GetAngle(p.x, p.y, X, Y, p1.x, p1.y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            // The following statement was changed. See the comments.
            //return (Math.Abs(total_angle) > 0.000001);
            return (Math.Abs(total_angle) > 1);
        }

        public static bool PointInPolygon(double X, double Y,Polyline poly)
        {
            // Get the angle between the point and the
            // first and last vertices.
            int max_point = poly.NumberOfVertices - 1;
            var p = poly.GetPoint2dAt(max_point);
            var p0 = poly.GetPoint2dAt(0);
            double total_angle = GetAngle(p.X, p.Y,X, Y, p0.X, p0.Y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++)
            {
                p = poly.GetPoint2dAt(i);
                var p1 = poly.GetPoint2dAt(i+1);
                total_angle += GetAngle(p.X, p.Y,X, Y,p1.X, p1.Y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            // The following statement was changed. See the comments.
            //return (Math.Abs(total_angle) > 0.000001);
            return (Math.Abs(total_angle) > 1);
        }

        public static double CrossProductLength(double Ax, double Ay,double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }

        public static double GetAngle(double Ax, double Ay,double Bx, double By, double Cx, double Cy)
        {
            // Get the dot product.
            double dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            double cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return Math.Atan2(cross_product, dot_product);
        }
        private static double DotProduct(double Ax, double Ay,double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the dot product.
            return (BAx * BCx + BAy * BCy);
        }
    }
}
