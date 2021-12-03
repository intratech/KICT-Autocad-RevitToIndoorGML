using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Point3DIntra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityGML.Exporter.AutoCAD.Utils
{
    public static class MathUtils
    {
        private static int increaseNumber = 1;

        public static int GetRandomID()
        {
            //byte[] buffer = Guid.NewGuid().ToByteArray();
            //var FormNumber = BitConverter.ToUInt32(buffer, 0) ^ BitConverter.ToUInt32(buffer, 4) ^ BitConverter.ToUInt32(buffer, 8) ^ BitConverter.ToUInt32(buffer, 12);
            //return FormNumber.ToString("X");
            //Thread.Sleep(100);
            //return int.Parse(DateTime.Now.ToString("ddHHmmssf"));
            return increaseNumber++;
        }
        public static int GetUniqueId()
        {
            Thread.Sleep(100);
            return int.Parse(DateTime.Now.ToString("ddHHmmssf"));
        }

        public static BoundingBox GetRoomBoundingBox(Polyline boundary, double roomHeight, double levelElevation)
        {
            var min = new Point3D();
            var max = new Point3D();

            for (int i = 0; i < boundary.NumberOfVertices; i++)
            {
                var vertex = boundary.GetPoint3dAt(i);

                min.x = Math.Min(min.x, vertex.X);
                min.y = Math.Min(min.y, vertex.Y);
                min.z = levelElevation;

                max.x = Math.Max(max.x, vertex.X);
                max.y = Math.Max(max.y, vertex.Y);
                max.z = roomHeight + levelElevation;
            }

            return new BoundingBox() { min = min, max = max };
        }
        private static Point3D GetMin(Point3D p1, Point3D p2)
        {
            return new Point3D()
            {
                x = Math.Min(p1.x, p2.x),
                y = Math.Min(p1.y, p2.y),
                z = Math.Min(p1.z, p2.z)
            };
        }
        private static Point3D GetMax(Point3D p1, Point3D p2)
        {
            return new Point3D()
            {
                x = Math.Max(p1.x, p2.x),
                y = Math.Max(p1.y, p2.y),
                z = Math.Max(p1.z, p2.z)
            };
        }
        public static BoundingBox GetBlockBoundingBox(BlockReference block)
        {
            var blockItems = new DBObjectCollection();
            block.Explode(blockItems);

            var min = new Point3D();
            var max = new Point3D();

            foreach (var item in blockItems)
            {
                var _line = item as Curve;
                if (_line == null)
                    continue;

                var minStartEnd = GetMin(new Point3D(_line.StartPoint.X, _line.StartPoint.Y, _line.StartPoint.Z), new Point3D(_line.EndPoint.X, _line.EndPoint.Y, _line.EndPoint.Z));
                min = GetMin(min, minStartEnd);

                var maxStartEnd = GetMax(new Point3D(_line.StartPoint.X, _line.StartPoint.Y, _line.StartPoint.Z), new Point3D(_line.EndPoint.X, _line.EndPoint.Y, _line.EndPoint.Z));
                max = GetMax(max, maxStartEnd);

                //if (item is Line)
                //{
                //    var _line = item as Line;

                //    var minStartEnd = GetMin(new Point3D(_line.StartPoint.X, _line.StartPoint.Y, _line.StartPoint.Z), new Point3D(_line.EndPoint.X, _line.EndPoint.Y, _line.EndPoint.Z));
                //    min = GetMin(min, minStartEnd);

                //    var maxStartEnd = GetMax(new Point3D(_line.StartPoint.X, _line.StartPoint.Y, _line.StartPoint.Z), new Point3D(_line.EndPoint.X, _line.EndPoint.Y, _line.EndPoint.Z));
                //    max = GetMax(max, maxStartEnd);
                //}
                //else if (item is Arc)
                //{
                //    var arc = item as Arc;

                //    var minStartEnd = GetMin(new Point3D(arc.StartPoint.X, arc.StartPoint.Y, arc.StartPoint.Z), new Point3D(arc.EndPoint.X, arc.EndPoint.Y, arc.EndPoint.Z));
                //    min = GetMin(min, minStartEnd);

                //    var maxStartEnd = GetMax(new Point3D(arc.StartPoint.X, arc.StartPoint.Y, arc.StartPoint.Z), new Point3D(arc.EndPoint.X, arc.EndPoint.Y, arc.EndPoint.Z));
                //    max = GetMax(max, maxStartEnd);
                //}
            }

            return new BoundingBox() { min = min, max = max };
        }
        public static Point3D GetCenter(Point3D p1, Point3D p2)
        {
            var center = new Point3D();
            center.x = (p1.x + p2.x) / 2;
            center.y = (p1.y + p2.y) / 2;
            center.z = (p1.z + p2.z) / 2;
            return center;
        }
        public static Point3D ProjectOnLine(this Point3D A, Point3D P1, Point3D P2)
        {
            try
            {
                //Tim hinh chieu H cua A len P1P2
                Point3D P1P2 = P2 - P1;
                var x = P1P2.x * (-P1.x * P1P2.x + A.x * P1P2.x - P1.y * P1P2.y + A.y * P1P2.y - P1P2.z * P1.z + A.z * P1P2.z) / (P1P2.x * P1P2.x + P1P2.y * P1P2.y + P1P2.z * P1P2.z) + P1.x;
                var y = P1P2.y * (-P1.x * P1P2.x + A.x * P1P2.x - P1.y * P1P2.y + A.y * P1P2.y - P1P2.z * P1.z + A.z * P1P2.z) / (P1P2.x * P1P2.x + P1P2.y * P1P2.y + P1P2.z * P1P2.z) + P1.y;
                var z = P1P2.z * (-P1.x * P1P2.x + A.x * P1P2.x - P1.y * P1P2.y + A.y * P1P2.y - P1P2.z * P1.z + A.z * P1P2.z) / (P1P2.x * P1P2.x + P1P2.y * P1P2.y + P1P2.z * P1P2.z) + P1.z;
                Point3D H = new Point3D(x, y, z);
                return H;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static Point3D MultiplyScalar(this Point3D vector, double value)
        {
            vector.x *= value;
            vector.y *= value;
            vector.z *= value;

            return vector;
        }
        public static Point3d ConvertToAutoCADPoint(this Point3D vector)
        {
            return new Point3d(vector.x, vector.y, vector.z);
        }
        //public static Point3D GetMin(Point3D p1, Point3D p2)
        //{
        //    var x = Math.Min(p1.x, p2.x);
        //    var y = Math.Min(p1.y, p2.y);
        //    var z = Math.Min(p1.z, p2.z);

        //    return new Point3D(x, y, z);
        //}
        //public static Point3D GetMax(Point3D p1, Point3D p2)
        //{
        //    var x = Math.Max(p1.x, p2.y);
        //    var y = Math.Max(p1.y, p2.y);
        //    var z = Math.Max(p1.z, p2.z);

        //    return new Point3D(x, y, z);
        //}
        public static BoundingBox ExtendBy(this BoundingBox box1, BoundingBox box2)
        {
            var min = GetMin(box1.min, box2.min);
            var max = GetMax(box1.max, box2.max);

            return new BoundingBox() { min = min, max = max };
        }
        public static Point3D ToIntraPoint(this Point3d point,double scale=1)
        {
            return new Point3D(point.X* scale, point.Y * scale, point.Z * scale);
        }

        public static Point3D ToIntraPoint(this Vector3d point)
        {
            return new Point3D(point.X, point.Y, point.Z);
        }

        public static bool IsSameLine(LineSegment3d line1, LineSegment3d line2)
        {
            var mid1 = (line1.StartPoint.ToIntraPoint() + line1.EndPoint.ToIntraPoint()) / 2;
            var mid2 = (line2.StartPoint.ToIntraPoint() + line2.EndPoint.ToIntraPoint()) / 2;

            return mid1.distance(mid2) < 1;
        }

        public static void test()
        {
            Polyline pl = new Polyline();
            
        }
        public static bool IsPointOnSegment(Point3d point, LineSegment3d segment, double toleren = 1)
        {
            var dis = segment.GetClosestPointTo(point).Point.DistanceTo(point);
            return dis <= toleren;
        }
        public static Point3D TranslateByVector(this Point3D point, Point3D vector)
        {
            var length = vector.GetLength();
            return point + (vector * length);
        }
    }
}
