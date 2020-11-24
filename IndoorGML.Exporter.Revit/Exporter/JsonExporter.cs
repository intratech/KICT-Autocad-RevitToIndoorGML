using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Newtonsoft.Json.Linq;
using Point3DIntra;
using IndoorGML.Exporter.Revit.Entities;
using IndoorGML.Exporter.Revit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IndoorGML.Exporter.Revit.Config;
using System.Windows;
using GML.Core;
using GML.Core.DTO.Json;

namespace IndoorGML.Exporter.Revit.Exporter
{
    public class JsonExporter
    {
        AppSettings _settings;

        public JsonExporter()
        {
            //wt = new StreamWriter(@"G:\xml.xml");
            _settings = AppSettings.Instance;
        }

        public List<JRoom> ExportRoomsEx(IEnumerable<Element> rooms, Document doc, out BoundingBoxXYZ box)
        {
            var j_rooms = new List<JRoom>();
            var j_type = "Room";

            var extendedBox = new BoundingBoxXYZ();
            var isFirst = true;
            foreach (var e in rooms)
            {
                var room = e as Room;
                if (room == null)
                    continue;


                //room type
                bool? isElevator = null;
                if (_settings.ElevatorLabels.Any(lb => room.Name.Contains(lb)))
                {
                    j_type = "Elevator";
                    isElevator = true;
                }

                bool? isEscalator = null;
                if (_settings.EscaplatorLabels.Any(lb => room.Name.Contains(lb)))
                {
                    isEscalator = true;
                }

                bool? isStair = null;
                if (_settings.StairLabels.Any(lb => room.Name.Contains(lb)))
                {
                    j_type = "Stair";
                    isStair = true;
                }

                //Boundary
                var boundary = room.GetBoundarySegments(new SpatialElementBoundaryOptions()
                {
                    SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish
                });
                var boundingBox = ElementUtils.GetBoudningBox(e);


                var j_walls = new List<JLineSegment>();
                foreach (var segArray in boundary)
                {
                    foreach (var seg in segArray)
                    {                        
                        var curve = seg.GetCurve();
                        var points = curve.Tessellate();

                        Point3D j_startPoint = null;
                        Point3D j_endPoint = null;

                        var wall = doc.GetElement(seg.ElementId) as Wall;
                        bool? j_closureSurface = null;
                        if (wall != null)
                        {
                            var wallType = doc.GetElement(wall.GetTypeId());
                            if (wallType.Name.Contains("cs"))
                            {
                                j_closureSurface = true;
                            }
                        }

                        for (int i = 0; i < points.Count - 1; i++)
                        {
                            int uniqueId;
                            if (j_closureSurface == true)
                                uniqueId = wall.Id.IntegerValue;
                            else
                                uniqueId = MathUtils.GetRandomID();

                            j_startPoint = new Point3D(points[i].X, points[i].Y, boundingBox.Min.Z);
                            j_endPoint = new Point3D(points[i + 1].X, points[i + 1].Y, boundingBox.Min.Z);


                            j_walls.Add(new JLineSegment
                            {
                                id = uniqueId,
                                //id2 = seg.ElementId.IntegerValue,
                                isClosureSurface = j_closureSurface,
                                points = new List<Point3D> { j_startPoint, j_endPoint }
                            });
                        }
                    }

                    break;
                }
                

                //Center
                //var centerOfElement = ElementUtils.GetCenter(e);
                //var j_center = new Point3D(centerOfElement.X, centerOfElement.Y, centerOfElement.Z);

                //BoundingBox
               
                var j_box = new JBoundingBox(new Point3D(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Min.Z), new Point3D(boundingBox.Max.X, boundingBox.Max.Y, boundingBox.Max.Z));
                if (isFirst)
                {
                    extendedBox = boundingBox;
                    isFirst = false;
                }
                extendedBox = extendedBox.ExtendBy(boundingBox);

                //Element Level
                var levelName = string.Empty;
                if (e.LevelId != null)
                {
                    var level = doc.GetElement(e.LevelId) as Level;
                    if (level != null)
                        levelName = level.Name;
                }

                //Properties
                var props = ElementUtils.GetProperties(e);

                var j_room = new JRoom() {
                    id = e.Id.IntegerValue,
                    type = j_type,
                    is_elevator = isElevator,
                    is_escalator = isEscalator,
                    is_stair = isStair,
                    name = e.Name,
                    level_id = e.LevelId != null ? e.LevelId.IntegerValue : -1,
                    geometry = new JRoomGeometry {
                        //position = j_center,
                        bouding_box = j_box,
                        boundary = j_walls
                    },
                    properties = props.Select(p => new JProp(p.Name, p.Value)).ToList()
                };

                j_rooms.Add(j_room);
            }
            box = extendedBox;

            //wt.Flush();
            //wt.Close();

            return j_rooms;
        }
        public List<JDoor> ExportDoorsEx(IEnumerable<Element> doors, List<JRoom> jRooms, List<Phase> phases)
        {
            var jDoors = new List<JDoor>();
            var jType = "Door";

            foreach (var e in doors)
            {                
                var fIns = e as FamilyInstance;
                if (fIns == null)
                    continue;

                var roomIds = new List<long>();               

                //Width & Height
                double j_width = -1;
                double j_height = -1;
                var doorWH = ElementUtils.GetDoorWidthHeight(fIns, true);
                if (doorWH.Item1 > 0)
                    j_width = doorWH.Item1;
                if (doorWH.Item2 > 0)
                    j_height = doorWH.Item2;

               

                List<Pos> j_positions = new List<Pos>();
                foreach (var room in jRooms)
                {
                    var sId = -1;
                    XYZ point = null;
                    XYZ direction = null;
                    if(ElementUtils.IsDoorBelongToRoom(fIns, room, out sId, out point, out direction))
                    {
                        j_positions.Add(new Pos {
                            direction = new Point3D(direction.X, direction.Y,direction.Z),
                            position = new Point3D(point.X, point.Y, point.Z),
                            segment_id = sId
                        });
                        roomIds.Add(room.id);
                    }
                }
             

                var jDoor = new JDoor()
                {
                    id = e.Id.IntegerValue,
                    type = jType,
                    name = e.Name,
                    level_id = e.LevelId.IntegerValue,
                    roomIds = roomIds.ToList(),
                    geometry = new JDoorGeometry {
                        positions = j_positions,
                        width = j_width,
                        height = j_height
                    }
                };

                jDoors.Add(jDoor);
            }
            return jDoors;
        }
        public List<JDoor> ExportModelLineToDoor(IEnumerable<Element> modelLines, List<JRoom> rooms,Phase phase)
        {
            var jDoors = new List<JDoor>();
            var jType = "RoomSeperator";
            foreach(var line in modelLines)
            {
                if(line is ModelLine mLine)
                {
                    var startPoint = mLine.GeometryCurve.GetEndPoint(0);
                    var endPoint = mLine.GeometryCurve.GetEndPoint(1);
                    var centerPiont = (startPoint + endPoint) / 2;
                    JDoor door = new JDoor();

                    float height = 0;
                    List<long> roomIds = new List<long>();
                    rooms.ForEach(room =>
                    {
                        if (room.geometry.boundary.Any(wall => wall.Contains(centerPiont.ToPoint3D())))
                        {
                            roomIds.Add(room.id);
                            height = room.RoomHeight;
                        }
                    });

                    //making sure it has more 2 room connect by model line
                    if(roomIds.Count >1)
                    {
                        //creating the door
                        var jdoor = new JDoor()
                        {
                            id = line.Id.IntegerValue,
                            type = jType,
                            name = line.Name,
                            level_id = line.LevelId.IntegerValue,
                            roomIds = roomIds.ToList(),
                            geometry = new JDoorGeometry
                            {
                                positions = new List<Pos>(),
                                width = mLine.GeometryCurve.Length,
                                height = height
                            }                            
                        };
                        Pos pos = new Pos();
                        pos.position = centerPiont.ToPoint3D();
                        pos.direction = (endPoint - startPoint).CrossProduct(XYZ.BasisZ).Normalize().ToPoint3D();
                        jdoor.geometry.positions.Add(pos);
                        jDoors.Add(jdoor);
                    }
                }

                
            }
            return jDoors;
        }

        public List<JWindow> ExportWindowsEx(IEnumerable<Element> windows, List<JRoom> jRooms, Phase phase)
        {
            var jWindows = new List<JWindow>();
            var jType = "Window";

            foreach (var e in windows)
            {
                var fIns = e as FamilyInstance;
                if (fIns == null)
                    continue;


                List<Pos> j_positions = new List<Pos>();
                var roomIds = new List<long>();
                foreach (var room in jRooms)
                {

                    var sId = -1;
                    XYZ point = null;
                    XYZ direction = null;
                    if (ElementUtils.IsWindowBelongToRoom(fIns, room, out sId, out point, out direction))
                    {
                        j_positions.Add(new Pos
                        {
                            direction = new Point3D(direction.X, direction.Y, direction.Z),
                            position = new Point3D(point.X, point.Y, point.Z),
                            segment_id = sId
                        });
                        roomIds.Add(room.id);
                    }
                }

                //Direction
                //var j_direction = new Point3D(fIns.FacingOrientation.X, fIns.FacingOrientation.Y, fIns.FacingOrientation.Z);

                //Width & Height
                double j_width = -1;
                double j_height = -1;
                var doorWH = ElementUtils.GetDoorWidthHeight(fIns, true);
                if (doorWH.Item1 > 0)
                    j_width = doorWH.Item1;
                if (doorWH.Item2 > 0)
                    j_height = doorWH.Item2;

                //Center
                var centerOfElement = ElementUtils.GetLocation(e);
               

                var jWindow = new JWindow()
                {
                    id = e.Id.IntegerValue,
                    type = jType,
                    name = e.Name,
                    level_id = e.LevelId.IntegerValue,
                    roomIds = roomIds.ToList(),
                    geometry = new JDoorGeometry
                    {
                        positions = j_positions,
                        width = j_width,
                        height = j_height
                    }
                };

                jWindows.Add(jWindow);
            }
            return jWindows;
        }
        public List<JWall> ExportWallEx(IList<Element> walls, List<JRoom> rooms, Document doc)
        {
            var jWalls = new List<JWall>();
            var j_type = "Wall";

            //Options optCompRef = doc.Application.Create.NewGeometryOptions();
            //var count = 1;

            foreach (var _wall in walls)
            {
                var wall = _wall as Wall;
                if (wall == null)
                    continue;

                //Position
                //var center = ElementUtils.GetCenterEx(wall);
                //var j_center = new Point3D(center.X, center.Y, center.Z);

                //Bounding box
                JBoundingBox j_box = null;
                var box = ElementUtils.GetBoudningBox(wall);
                if (box != null)
                {
                    j_box = new JBoundingBox(new Point3D(box.Min.X, box.Min.Y, box.Min.Z), new Point3D(box.Max.X, box.Max.Y, box.Max.Z));
                }

                //Location curve
                var location = wall.Location as LocationCurve;
                List<Point3D> j_locationCurve = null;
                if (location != null)
                {
                    var curve = location.Curve;
                    var startPoint = curve.GetEndPoint(0);
                    var endPoint = curve.GetEndPoint(1);
                    j_locationCurve = new List<Point3D> {
                        new Point3D(startPoint.X, startPoint.Y, startPoint.Z),
                        new Point3D(endPoint.X, endPoint.Y, endPoint.Z)
                    };
                }

                //Direction
                Point3D j_direction = null;
                if (wall.Orientation != null)
                {
                    j_direction = new Point3D(wall.Orientation.X, wall.Orientation.Y, wall.Orientation.Z);
                }

                var vertices = new List<Point3D>();
                var exteralVertices = new List<Point3D>();
                var faces = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Interior);
                var facesExternal = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior);

                if (faces != null)
                {
                    foreach (var face in faces)
                    {
                        Face wallFace = wall.GetGeometryObjectFromReference(face) as Face;
                        if (wallFace == null)
                            continue;

                        EdgeArrayArray faceBoundary = wallFace.EdgeLoops;
                        if (faceBoundary == null)
                            continue;

                        var lines = new List<object>();

                        foreach (EdgeArray edgeArr in faceBoundary)
                        {
                            foreach (Edge edge in edgeArr)
                            {
                                var curve = edge.AsCurve();
                                var startPoint = curve.GetEndPoint(0);
                                var endPoint = curve.GetEndPoint(1);

                                vertices.Add(new Point3D(startPoint.X, startPoint.Y, startPoint.Z));
                                vertices.Add(new Point3D(endPoint.X, endPoint.Y, endPoint.Z));
                            }
                        }
                    }
                }

                foreach (var face in facesExternal)
                {
                    Face wallFace = wall.GetGeometryObjectFromReference(face) as Face;
                    if (wallFace == null)
                        continue;

                    EdgeArrayArray faceBoundary = wallFace.EdgeLoops;
                    if (faceBoundary == null)
                        continue;

                    var lines = new List<object>();
                    foreach (EdgeArray edgeArr in faceBoundary)
                    {
                        foreach (Edge edge in edgeArr)
                        {
                            var curve = edge.AsCurve();
                            var startPoint = curve.GetEndPoint(0);
                            var endPoint = curve.GetEndPoint(1);

                            exteralVertices.Add(new Point3D(startPoint.X, startPoint.Y, startPoint.Z));
                            exteralVertices.Add(new Point3D(endPoint.X, endPoint.Y, endPoint.Z));
                        }
                    }
                }

                

                var jWall = new JWall {
                    id = _wall.Id.IntegerValue,
                    type = j_type,
                    name = _wall.Name,
                    level_id = _wall.LevelId.IntegerValue,
                    //room_ids = j_roomIds,
                    geometry = new JWallGeometry {
                        direction = j_direction,
                        location_curve = j_locationCurve.ToArray(),
                        external_faces = exteralVertices.ToArray(),
                        internal_faces = vertices.ToArray()
                    }
                };

                jWalls.Add(jWall);
            }
            
            return jWalls;
        }
    }
}
