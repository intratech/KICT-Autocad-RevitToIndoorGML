using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CityGML.Core.DTO.Json;
using Point3DIntra;

namespace CityGMLExporter.Utils
{
    public static class MathUtils
    {
        //***********************************
        //Function content: Project P on Plane of n(normal vector) and A(point)
        //Developer: Donny
        //Last modifier:
        //Modification content:
        //Updated on: 4-Sep-19
        //***********************************
        public static Point3D ProjectOnPlane(this Point3D P, Point3D A, Point3D n)
        {
            try
            {
                Point3D v = P - A;
                double distance = v * n;
                Point3D moveP = n * distance;
                Point3D projectP = P - moveP;
                return projectP;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //***********************************
        //Function content: Calculate distance from 1 point line(Distance from A to MH)
        //Developer: Donny
        //Last modifier:
        //Modification content:
        //Updated on: 30-Aug-19
        //***********************************
        public static double? DistancePointToLine(Point3D A, Point3D M, Point3D H)
        {
            if (A != null && M != null && H != null)
            {
                //Calculate coordinate of H_ (perpendiculaire:hinh chieu vuong goc)
                Point3D H_ = M.ProjectOnLine(H, A);

                //Calculate distance
                Point3D AH_ = A - H_;
                double d = AH_.GetLength();
                return d;
            }
            return null;
        }

        //***********************************
        //Function content: Tim hinh chieu H cua A len P1P2
        //Developer: Donny
        //Last modifier:
        //Modification content:
        //Updated on: 30-Aug-19
        //***********************************
        public static Point3D ProjectOnLine(this Point3D A, Point3D P1, Point3D P2)
        {
            try
            {
                //Tim hinh chieu H cua A len P1P2
                Point3D P1A = A - P1;
                Point3D v = P1 - P2;
                v.Normalize();
                var dis = P1A * v;
                Point3D P1H = v * dis;
                Point3D H = P1 + P1H;

                return H;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //Separate room from big room and closureSurface
        public static List<JRoom> SeparateRoom(JRoom room, List<JLineSegment> closureSurface)
        {
            try
            {
                //Tim hinh chieu H cua A len P1P2
                List<JRoom> RoomList = new List<JRoom>();

                List<JLineSegment> newBoundary = new List<JLineSegment>();

                //Loop all segments of room
                var boundaries = room.geometry.boundary;
                foreach (var seg in boundaries)
                {
                    var point1 = seg.points[0];
                    var point2 = seg.points[0];
                    bool tole = false;

                    //Loop in closureSurface list
                    for (var i = 0; i<closureSurface.Count;i++)
                    {
                        var segClosureSF = closureSurface[i].points;
                        for (var j = 0; j< segClosureSF.Count; j++)
                        {
                            tole = IsOnAndBetween(segClosureSF[j], point1, point2);

                            if (tole == true)
                            {
                                //Create new segment
                                JLineSegment newSeg = new JLineSegment();
                                newSeg.id = seg.id;
                                //newSeg.id2 = seg.id + i + j;
                                newSeg.points = new List<Point3D>();
                                newSeg.points[0].Set(point1);
                                newSeg.points[1].Set(segClosureSF[j]);

                                //Create new boundaries
                                newBoundary.Add(newSeg);
                                newBoundary.Add(closureSurface[i]);

                                //Find remaining boundary
                                var point = new Point3D();
                                if (j == 0)
                                    point = segClosureSF[j+1];
                                else point = segClosureSF[j - 1];
                                FindRemainBoundary(boundaries, point , ref newBoundary);
                                break;
                            }
                        }
                    }
                    if(tole == false)
                        newBoundary.Add(seg);
                }

                return RoomList;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static void FindRemainBoundary(List<JLineSegment> boundaries, Point3D point , ref List<JLineSegment> newBoundary)
        {
            bool startNewSeg = false;
            foreach (var seg in boundaries)
            {
                var point1 = seg.points[0];
                var point2 = seg.points[0];

                var tole = IsOnAndBetween(point, point1, point2);

                if(tole == true)
                {
                    //Create new segment
                    JLineSegment newSeg = new JLineSegment();
                    newSeg.id = seg.id;
                    //newSeg.id2 = seg.id;
                    newSeg.points = new List<Point3D>();
                    newSeg.points[0].Set(point);
                    newSeg.points[1].Set(point2);

                    newBoundary.Add(newSeg);

                    startNewSeg = true;
                }
                if(startNewSeg == true)
                {
                    newBoundary.Add(seg);
                }
            }
        }

        public static bool IsOnAndBetween(this Point3D pointC, Point3D pointA, Point3D pointB)
        {
            var AC = (pointC - pointA).GetLength();
            var CB = (pointB - pointC).GetLength();
            var AB = (pointB - pointA).GetLength();
            return (AC + CB) - AB <= 0.01;
        }
    }
}
