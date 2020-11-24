using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Utils
{
    public static class MathUtils
    {
        private static int increaseNumber = 1;
        public static BoundingBoxXYZ TransformBox(BoundingBoxXYZ box)
        {
            XYZ min = box.Min;
            XYZ max = box.Max;

            XYZ point1 = min;
            XYZ point2 = new XYZ(max.X, min.Y, min.Z);
            XYZ point3 = new XYZ(max.X, max.Y, min.Z);
            XYZ point4 = new XYZ(min.X, max.Y, min.Z);

            XYZ point5 = new XYZ(min.X, min.Y, max.Z);
            XYZ point6 = new XYZ(max.X, min.Y, max.Z);
            XYZ point7 = max;
            XYZ point8 = new XYZ(min.X, max.Y, max.Z);

            XYZ[] p = new XYZ[8];

            p[0] = box.Transform.OfPoint(point1);
            p[1] = box.Transform.OfPoint(point2);
            p[2] = box.Transform.OfPoint(point3);
            p[3] = box.Transform.OfPoint(point4);
            p[4] = box.Transform.OfPoint(point5);
            p[5] = box.Transform.OfPoint(point6);
            p[6] = box.Transform.OfPoint(point7);
            p[7] = box.Transform.OfPoint(point8);

            double minX = p[0].X;
            double minY = p[0].Y;
            double minZ = p[0].Z;

            for (int i = 0; i < 7; i++)
            {
                minX = Math.Min(minX, p[i].X);
                minY = Math.Min(minY, p[i].Y);
                minZ = Math.Min(minZ, p[i].Z);
            }

            double maxX = p[0].X;
            double maxY = p[0].Y;
            double maxZ = p[0].Z;
            for (int i = 0; i < 7; i++)
            {
                maxX = Math.Max(maxX, p[i].X);
                maxY = Math.Max(maxY, p[i].Y);
                maxZ = Math.Max(maxZ, p[i].Z);
            }

            BoundingBoxXYZ newBox = new BoundingBoxXYZ();
            newBox.Min = new XYZ(minX, minY, minZ);
            newBox.Max = new XYZ(maxX, maxY, maxZ);

            return newBox;
        }
        public static XYZ GetMin(XYZ p1, XYZ p2)
        {
            var x = Math.Min(p1.X, p2.X);
            var y = Math.Min(p1.Y, p2.Y);
            var z = Math.Min(p1.Z, p2.Z);

            return new XYZ(x,y,z);
        }
        public static XYZ GetMax(XYZ p1, XYZ p2)
        {
            var x = Math.Max(p1.X, p2.X);
            var y = Math.Max(p1.Y, p2.Y);
            var z = Math.Max(p1.Z, p2.Z);

            return new XYZ(x, y, z);
        }
        public static BoundingBoxXYZ ExtendBy(this BoundingBoxXYZ box1, BoundingBoxXYZ box2) {
            var min = GetMin(box1.Min, box2.Min);
            var max = GetMax(box1.Max, box2.Max);

            return new BoundingBoxXYZ() { Min = min, Max = max };
        }
        public static bool IsOnAndBetween(this XYZ pointC, XYZ pointA, XYZ pointB)
        {
            //var vectorZero = new XYZ(0,0,0);
            //var isOn = (pointA - pointC).CrossProduct(pointB - pointC).IsAlmostEqualTo(vectorZero);
            //if (!isOn)
            //    return false;

            var AC = (pointC - pointA).GetLength();
            var CB = (pointB - pointC).GetLength();
            var AB = (pointB - pointA).GetLength();

            return (AC + CB) - AB <= 0.01;
        }
        public static XYZ ProjectOnLine(this XYZ A, XYZ P1, XYZ P2)
        {
            try
            {
                //Tim hinh chieu H cua A len P1P2
                XYZ P1P2 = P2 - P1;
                var x = P1P2.X * (-P1.X * P1P2.X + A.X * P1P2.X - P1.Y * P1P2.Y + A.Y * P1P2.Y - P1P2.Z * P1.Z + A.Z * P1P2.Z) / (P1P2.X * P1P2.X + P1P2.Y * P1P2.Y + P1P2.Z * P1P2.Z) + P1.X;
                var y = P1P2.Y * (-P1.X * P1P2.X + A.X * P1P2.X - P1.Y * P1P2.Y + A.Y * P1P2.Y - P1P2.Z * P1.Z + A.Z * P1P2.Z) / (P1P2.X * P1P2.X + P1P2.Y * P1P2.Y + P1P2.Z * P1P2.Z) + P1.Y;
                var z = P1P2.Z * (-P1.X * P1P2.X + A.X * P1P2.X - P1.Y * P1P2.Y + A.Y * P1P2.Y - P1P2.Z * P1.Z + A.Z * P1P2.Z) / (P1P2.X * P1P2.X + P1P2.Y * P1P2.Y + P1P2.Z * P1P2.Z) + P1.Z;
                XYZ H = new XYZ(x,y,z);
                return H;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static XYZ GetPerpendiculaire(this XYZ A, XYZ P1, XYZ P2)
        {
            try
            {
                //Tim hinh chieu H cua A len P1P2

                XYZ P1A = A - P1;
                XYZ v = P1 - P2;
                v.Normalize();
                var dis = P1A.DotProduct(v);
                XYZ P1H = v * dis;
                XYZ H = P1 + P1H;
                
                return H;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        //public static XYZ ProjectOnPlane(this XYZ point, XYZ pointOnPlane, XYZ planeNormal)
        //{
        //    var v = point.Subtract(pointOnPlane);
        //    var distance = v.DotProduct(planeNormal.Normalize());
        //    return point.Subtract(distance * planeNormal);
        //}
        public static XYZ ProjectOnPlane(this XYZ point, XYZ pointOnPlane, XYZ planeNormal)
        {
            XYZ v = point - pointOnPlane;
            double distance = v.DotProduct(planeNormal);
            XYZ moveP = planeNormal * distance;
            XYZ projectP = point - moveP;
            return projectP;
        }
        public static int GetRandomID()
        {
            //byte[] buffer = Guid.NewGuid().ToByteArray();
            //var FormNumber = BitConverter.ToUInt32(buffer, 0) ^ BitConverter.ToUInt32(buffer, 4) ^ BitConverter.ToUInt32(buffer, 8) ^ BitConverter.ToUInt32(buffer, 12);
            //return FormNumber.ToString("X");
            //Thread.Sleep(100);
            //return int.Parse(DateTime.Now.ToString("ddHHmmssf"));
            return increaseNumber++;
        }
        public static bool IsSquare(XYZ v1, XYZ v2, double tol)
        {
            if (v1 == null || v2 == null)
                return false;

            var angle = v1.AngleTo(v2);
            
            return Math.Abs(angle - tol) < Math.PI/2 && Math.PI / 2 < Math.Abs(angle + tol);
        }
        public static XYZ Nearest(XYZ pointToCheck, XYZ[] points)
        {
            var minDistance = double.MaxValue;
            XYZ pointMin = null;
            foreach (var point in points)
            {
                var distance = pointToCheck.DistanceTo(point);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    pointMin = point;
                }
            }

            return pointMin;
        }
        public static double DegreesToRadians(int degrees)
        {
            var pi = Math.PI;
            return degrees * (pi / 180);
        }
        public static double ModelUnitToMillimeter(double value)
        {
            return UnitUtils.ConvertFromInternalUnits(value, DisplayUnitType.DUT_MILLIMETERS);
        }
    }
}
