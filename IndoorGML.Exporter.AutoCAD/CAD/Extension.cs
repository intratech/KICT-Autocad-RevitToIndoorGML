using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace IndoorGML.Exporter.AutoCAD.CAD
{
    static class Extension
    {
        public static bool IsOverLap(this LineSegment3d seg, LineSegment3d line3d)
        {
            
            return (seg.GetDistanceTo(line3d) < 100 && IsCollinear(seg.StartPoint, line3d.MidPoint, seg.EndPoint, 50));
        }

        public static LineSegment3d GetLineIntersect(this Polyline polyline, Entity other)
        {
            if(other is Line line)
            {
               Point3dCollection collection = new Point3dCollection();
               polyline.IntersectWith(other, Intersect.OnBothOperands, collection, IntPtr.Zero, IntPtr.Zero);
               if (collection.Count == 2)
                   return new LineSegment3d(line.StartPoint, line.EndPoint);

                LineSegment3d door = new LineSegment3d();
                door.Set(line.StartPoint, line.EndPoint);
                Tolerance tolerance = new Tolerance(0.01, 0.01);

                foreach(var wall in polyline.GetEdge())
                {
                    if(wall.Direction.IsParallelTo(door.Direction, tolerance))
                    {
                        if (wall.GetDistanceTo(door) <100 && IsCollinear(door.StartPoint,wall.MidPoint, door.EndPoint,50))
                        {
                            return door;
                        }
                    }
                }
            }
            else if(other is Polyline otherPolyline)
            {
                var virtualDoor = otherPolyline.GetEdge().ToList();
                var roomEdge = polyline.GetEdge().ToList();

                foreach (var door in virtualDoor)
                {
                    if (roomEdge.Any(wall =>
                        {
                            return door.IsOverLap(wall);
                        }))
                    {
                        return door;
                    }
                    
                }
            }

            return null;
        }

        private static bool IsCollinear(Point3d start, Point3d mid, Point3d end, float tolerance)
        {
            return ((end-start).Length-((mid-start).Length+(end-mid).Length)<tolerance);
        }

        public static IEnumerable<LineSegment3d> GetEdge(this Polyline polyline)
        {
            int num = polyline.NumberOfVertices;
            for(int i=0;i<num-1;i++)
            {
                yield return polyline.GetLineSegmentAt(i);
            }

            if(polyline.Closed)
            {
                yield return polyline.GetLineSegmentAt(num - 1);
            }
        }
    }
}
