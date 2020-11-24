using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using GML.Core.DTO.Json;
using IndoorGML.Exporter.Revit.Config;
using IndoorGML.Exporter.Revit.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Utils
{
    public static class ElementUtils
    {
        public static IList<Element> QueryElementsOfCategory(Document doc, BuiltInCategory category)
        {
            try
            {
                var elements = new FilteredElementCollector(doc).OfCategory(category).ToElements();
                return elements;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static XYZ GetCenter(Element e)
        {
            BoundingBoxXYZ box = e.get_BoundingBox(null);
            
            if (box == null)
                return new XYZ();

            double zOffSet = 0x00;
            LocationPoint loc = null;
            if (e.Location != null)
            {
                loc = e.Location as LocationPoint;
            }
            if (loc != null)
            {
                zOffSet = loc.Point.Z;
            }

            var _center = (box.Max + box.Min) * 0.5;



            var center = new XYZ(_center.X, _center.Y, zOffSet);

            if (box.Transform != Transform.Identity)
                center = box.Transform.OfPoint(center);

            return center;
        }
        public static XYZ GetCenterEx(Element e)
        {
            BoundingBoxXYZ box = e.get_BoundingBox(null);
            var center = new XYZ();
            var loc = e.Location as LocationPoint;
            if (loc != null)
                center = loc.Point;
            else
            {
                center = GetCenter(e);
            }

            if(box != null && box.Transform != null && box.Transform != Transform.Identity)
            {
                center = box.Transform.OfPoint(center);
            }

            return center;
        }
        public static XYZ GetBottomCenter(Element e)
        {
            BoundingBoxXYZ box = e.get_BoundingBox(null);
            if (box == null)
                return null;
            
            var center = (box.Min + box.Max) * 0.5;
            var bottomCenter = new XYZ(center.X, center.Y, box.Min.Z);

            if (box != null && box.Transform != null && box.Transform != Transform.Identity)
            {
                bottomCenter = box.Transform.OfPoint(bottomCenter);
            }

            return bottomCenter;
        }
        
        public static BoundingBoxXYZ GetBoudningBox(Element e)
        {
            var box = e.get_BoundingBox(null);
            if (box == null)
            {
                return new BoundingBoxXYZ
                {
                    Min = new XYZ(),
                    Max = new XYZ()
                };
            }

            if (box.Transform != null && box.Transform != Transform.Identity)
            {
                box = MathUtils.TransformBox(box);
            }
            return new BoundingBoxXYZ { Min = box.Min * box.Transform.Scale, Max = box.Max * box.Transform.Scale };
        }
        public static XYZ GetLocation(Element e)
        {
            LocationPoint loc = null;
            if (e.Location != null)
            {
                loc = e.Location as LocationPoint;
            }
            if (loc == null)
            {
                return GetBottomCenter(e);
            }
                

            return loc.Point;
        }
        public static Tuple<double, double> GetDoorWidthHeight(Element door)
        {
            //var fIns = door as FamilyInstance;
            //if (fIns == null)
            //    return new Tuple<int, int>(-1,-1);

            //var pW = fIns.Symbol.get_Parameter(BuiltInParameter.DOOR_WIDTH);
            //var pH = fIns.Symbol.get_Parameter(BuiltInParameter.DOOR_HEIGHT);

            double width = -1.0;
            double height = -1.0;

            
            var pW = door.GetParameters("Width").FirstOrDefault();
            var pH = door.GetParameters("Height").FirstOrDefault();

            var typeID = door.GetTypeId();
            Element type = null;
            if(typeID != null)
            {
                type = door.Document.GetElement(typeID);
            }


            if (pW == null)
            {
                if (type != null)
                    pW = type.GetParameters("Width").FirstOrDefault();
            }

            if (pH == null)
            {
                if(type != null)
                    pH = type.GetParameters("Height").FirstOrDefault();
            }

            if (pW != null)
            {
                var _width = pW.AsString();
                if (string.IsNullOrEmpty(_width))
                {
                    _width = pW.AsValueString() ?? "";
                }
                if (!double.TryParse(_width, out width))
                {
                    width = -1;
                }
            }
            if (pH != null)
            {
                var _height = pH.AsString();
                if (string.IsNullOrEmpty(_height))
                {
                    _height = pH.AsValueString() ?? "";
                }
                if (!double.TryParse(_height, out height))
                {
                    height = -1;
                }
            }

            //return new Tuple<double, double>(width/1000, height/1000);
            return new Tuple<double, double>(width, height);
        }
        public static Tuple<double, double> GetDoorWidthHeight(FamilyInstance door, bool box)
        {
            var direction = door.FacingOrientation;
            var doorBox = GetBoudningBox(door);
            var xAxis = new XYZ(1,0,0);
            var yAxix = new XYZ(0,1,0);
            var squareX = -0.01 < direction.DotProduct(xAxis) && direction.DotProduct(xAxis) < 0.01;
            var squareY = -0.01 < direction.DotProduct(yAxix) && direction.DotProduct(yAxix) < 0.01;
            var width = 0.0;
            var height = 0.0;

            if (squareX)
            {
                width = Math.Abs(doorBox.Max.X - doorBox.Min.X);
            }
            else if (squareY)
            {
                width = Math.Abs(doorBox.Max.Y - doorBox.Min.Y);
            }
            else
            {
                var xBox = Math.Abs(doorBox.Max.X - doorBox.Min.X);
                var yBox = Math.Abs(doorBox.Max.Y - doorBox.Min.Y);
                width = Math.Sqrt(xBox * xBox + yBox * yBox);
            }

            height = Math.Abs(doorBox.Max.Z - doorBox.Min.Z);

            return new Tuple<double, double>(width, height);
        }


        public static List<Property> GetProperties(Element e)
        {
            var properties = new List<Property>();
            foreach (Parameter p in e.Parameters)
            {
                properties.Add(new Property(p.Definition.Name, p.AsValueString()));
            }
            var typeId = e.GetTypeId();
            if(typeId != null && typeId != ElementId.InvalidElementId)
            {
                var type = e.Document.GetElement(typeId);
                if(type != null)
                {
                    foreach (Parameter tp in type.Parameters)
                    {
                        properties.Add(new Property(tp.Definition.Name, tp.AsValueString()));
                    }
                }
            }
            return properties;
        }
        public static bool IsWindowBelongToRoom(FamilyInstance window, JRoom room, out int segmentId, out XYZ point, out XYZ direction)
        {
            segmentId = -1;
            point = null;
            direction = null;

            //Door location
            var windowPos = ElementUtils.GetLocation(window);
            var doorDirection = window.FacingOrientation;

            //check all segments to get which is:
            // - square with door direction
            // - contain projection point of door position
            // - distance to door position less then 30mm
            var wallSegments = room.geometry.boundary;//.Where(w => w.id2 == door.Host.Id.IntegerValue);
            var minDistance = double.MaxValue;
            JLineSegment minSeg = null;
            XYZ minPoint = null;
            foreach (var segment in wallSegments)
            {
                var startPoint = new XYZ(segment.points[0].x, segment.points[0].y, segment.points[0].z);
                var endPoint = new XYZ(segment.points[1].x, segment.points[1].y, segment.points[1].z);

                var axisZ = new XYZ(0,0,1);

                var projectionPointToSegment = windowPos.ProjectOnLine(startPoint, endPoint);//windowPos.GetPerpendiculaire(startPoint, endPoint);//
                var projectionPointToPlane = windowPos.ProjectOnPlane(startPoint, doorDirection);
                var newPointAfterProject = new XYZ(projectionPointToSegment.X, projectionPointToSegment.Y, windowPos.Z);

                //is square
                var rad = MathUtils.DegreesToRadians(20);
                var isSquare = MathUtils.IsSquare(doorDirection, startPoint.Subtract(endPoint), rad);
                if (!isSquare)
                    continue;

                //is between
                var isBetween = projectionPointToSegment.IsOnAndBetween(startPoint, endPoint);
                if (!isBetween)
                    continue;

                //distance vertical
                var distance = Math.Abs(windowPos.DistanceTo(projectionPointToPlane));
                var distanceMillimeter = UnitUtils.ConvertFromInternalUnits(distance, DisplayUnitType.DUT_MILLIMETERS);
                if (distanceMillimeter > AppSettings.Instance.WallThickness)
                    continue;

                //distance along Z
                if (windowPos.Z < room.geometry.bouding_box.min.z || windowPos.Z > room.geometry.bouding_box.max.z)
                    continue;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    minSeg = segment;
                    minPoint = newPointAfterProject;
                }
            }

            if (minSeg == null || minPoint == null)
                return false;

            point = minPoint;
            segmentId = minSeg.id;
            direction = windowPos.Subtract(point);

            return true;
        }
        public static bool IsDoorBelongToRoom(FamilyInstance door, JRoom room, out int segmentId, out XYZ point, out XYZ direction)
        {            
            segmentId = -1;
            point = null;
            direction = null;

            if (door.Host == null)
                return false;

            var doc = door.Document;
            if (doc == null)
                return false;

            //Door location
            var doorPos = ElementUtils.GetLocation(door);
            if (doorPos == null)
                return false;
            var doorDirection = door.FacingOrientation;

            //check all segments to get which is:
            // - square with door direction
            // - contain projection point of door position
            // - distance to door position less then 30mm
            var wallSegments = room.geometry.boundary;//.Where(w => w.id2 == door.Host.Id.IntegerValue);
            var minDistance = double.MaxValue;
            JLineSegment minSeg = null;
            XYZ minPoint = null;
            
            foreach (var segment in wallSegments)
            {
                var startPoint = new XYZ(segment.points[0].x, segment.points[0].y, segment.points[0].z);
                var endPoint = new XYZ(segment.points[1].x, segment.points[1].y, segment.points[1].z) ;

                var projectionPointToSegment = doorPos.ProjectOnLine(startPoint, endPoint); //doorPos.GetPerpendiculaire(startPoint, endPoint);//
                //var zOffsetDoorToSegment = doorPos.Z - projectionPointToSegment.Z;
                var projectionPointToSquare = new XYZ(projectionPointToSegment.X, projectionPointToSegment.Y, doorPos.Z);

                //is square
                var rad = MathUtils.DegreesToRadians(20);
                var isSquare = MathUtils.IsSquare(doorDirection, startPoint.Subtract(endPoint), rad);
                if (!isSquare)
                    continue;

                //is between
                var isBetween = projectionPointToSegment.IsOnAndBetween(startPoint, endPoint);
                if (!isBetween)
                    continue;

                //distance vertical
                var distance = Math.Abs(doorPos.DistanceTo(projectionPointToSegment));
                var distanceMillimeter = UnitUtils.ConvertFromInternalUnits(distance, DisplayUnitType.DUT_MILLIMETERS);
                if (distanceMillimeter > AppSettings.Instance.WallThickness)
                    continue;

                //z offset is within room lower and upper bound
                //var tol = 100;
                //if (MathUtils.ModelUnitToMillimeter(doorPos.Z) - MathUtils.ModelUnitToMillimeter(room.geometry.bouding_box.min.z) - tol < 0 
                //    || MathUtils.ModelUnitToMillimeter(doorPos.Z) - MathUtils.ModelUnitToMillimeter(room.geometry.bouding_box.max.z) + tol > 0)
                //    continue;


                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minSeg = segment;
                    minPoint = projectionPointToSquare;
                }
            }

            if (minSeg == null || minPoint == null)
                return false;

            point = minPoint;
            segmentId = minSeg.id;
            direction = doorPos.Subtract(point);

            return true;
        }

        public static bool DeleteElements(Document doc, IEnumerable<ElementId> iDs)
        {
            try
            {                
                using (var trans = new Transaction(doc))
                {
                    trans.Start("Delete Room");
                    doc.Delete(iDs.ToArray());
                    trans.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
