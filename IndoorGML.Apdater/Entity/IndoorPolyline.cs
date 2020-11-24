using GML.Core.DTO.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace IndoorGML.Adapter.Entity
{
    public class InDoorPolyline
    {
        public List<Vector3> Points = new List<Vector3>();
        public List<int> Segments = new List<int>();

        public int NumberOfVertices
        {
            get
            {
                return Points.Count;
            }
        }

        public void Standard()
        {
            Vector3 normal = Vector3.Zero;
            for(int i=0;i<Points.Count;i+=1)
            {
                var pos = Points[i];
                var next = Points[(i + 1)%Points.Count];

                normal.X += (pos.Y - next.Y) * (pos.Z + next.Z);
                normal.Y += (pos.Z - next.Z) * (pos.X + next.X);
                normal.Z += (pos.X - next.X) * (pos.Y + next.Y);
            }

            normal = Vector3.Normalize(normal);
            if(Vector3.Dot(normal,Vector3.UnitZ) < 0)
            {
                Points.Reverse();
            }
        }

        public void AddRange(List<Point3DIntra.Point3D> points)
        {
            Segments.Add(points.Count);
            points.ForEach(x =>
            {
                Points.Add(new Vector3((float)x.x, (float)x.y, (float)x.z));
            });
        }

        public void Add(Vector3 v)
        {
            if (Points == null)
                Points = new List<Vector3>();
            Points.Add(v);
        }

        private Vector3? center;
        internal Vector3 GetCenter()
        {
            if (center != null)
                return center.Value;

            if (Points.Count == 0)
                return Vector3.Zero;

            var _center = CalculateCentroid(Points);

            if (IsPointInPolygon(Points,_center))
            {
                center = _center;
                return center.Value;
            }

            for (int i = 0; i < Points.Count; i++)
            {
                for (int j = i + 2; j < Points.Count; j++)
                {
                    _center = (Points[i] + Points[j]) / 2;
                    if (IsPointInPolygon(Points, _center))
                    {
                        center = _center;
                        return center.Value;
                    }
                }
            }

            center = new Vector3(
            (Points.Min(x => x.X) + Points.Max(x => x.X)) / 2,
            (Points.Min(x => x.Y) + Points.Max(x => x.Y)) / 2,
            Points.Min(x => x.Z));
            return center.Value;
        }

        public static Vector3 CalculateCentroid(List<Vector3> points)
        {
            if (points == null || points.Count == 0)
                return Vector3.Zero;

            var z = points[0].Z;
            float area = 0.0f;
            float Cx = 0.0f;
            float Cy = 0.0f;
            float tmp = 0.0f;
            int k;

            int lastPointIndex = points.Count - 1;

            for (int i = 0; i <= lastPointIndex; i++)
            {
                k = (i + 1) % (lastPointIndex + 1);
                tmp = points[i].X * points[k].Y -
                      points[k].X * points[i].Y;
                area += tmp;
                Cx += (points[i].X + points[k].X) * tmp;
                Cy += (points[i].Y + points[k].Y) * tmp;
            }
            area *= 0.5f;
            Cx *= 1.0f / (6.0f * area);
            Cy *= 1.0f / (6.0f * area);

            return new Vector3(Cx, Cy,z);
        }



        internal Vector3 GetPoint3dAt(int i)
        {
            if (i >= Points.Count)
            {
                return Points[0];
            }
            return Points[i];
        }

        public static bool IsPointInPolygon(List<Vector3> polygon, Vector3 testPoint)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
    }
}
