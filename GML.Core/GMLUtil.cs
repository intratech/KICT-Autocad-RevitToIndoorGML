using GML.Core.DTO.Json;
using Point3DIntra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GML.Core
{
    public class GMLUtil
    {
        /// <summary>
        /// This method calculates the surface from base line and moving vector.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="moveVec"></param>
        /// <returns></returns>
        public SurfaceMember GetSurface(Point3D startPoint, Point3D endPoint, Point3D moveVec)
        {
            if (startPoint != null && endPoint != null && moveVec != null)
            {
                Point3D point1 = new Point3D(startPoint.x, startPoint.y, startPoint.z);
                Point3D point2 = point1 + moveVec;

                Point3D point4 = new Point3D(endPoint.x, endPoint.y, endPoint.z);
                Point3D point3 = point4 + moveVec;

                SurfaceMember surface = new SurfaceMember();
                surface.points.Add(point1);
                surface.points.Add(point2);
                surface.points.Add(point3);
                surface.points.Add(point4);
                surface.points.Add(point1);
                return surface;
            }

            return null;
        }
        /// <summary>
        /// This method finds all doors and windows attached in the room.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="doors"></param>
        /// <param name="windows"></param>
        /// <returns></returns>
        public Dictionary<string, Door> FindDoorsWindowsInRoom(JRoom room, List<JDoor> doors, List<JWindow> windows)
        {
            if (room != null && doors != null)
            {
                double width = Config.DefaultWidth;
                double heigh = Config.DefaultHeight;

                //Create list of door inside Room
                Dictionary<string, Door> roomDoorWindowList = new Dictionary<string, Door>();

                //Check id of room is exist in door and get door's list
                var segmentsRoom = room.geometry.boundary;

                //Loop in each segment of room
                for (var i = 0; i < segmentsRoom.Count; i++)
                {
                    //Loop all doors
                    foreach (var door in doors)
                    {
                        var doorGeo = door.geometry;

                        if (doorGeo.width > 0)
                            width = (double)doorGeo.width;
                        if (doorGeo.height > 0)
                            heigh = (double)doorGeo.height;

                        //Loop in door position of each segment
                        foreach (var doorPos in door.geometry.positions)
                        {
                            if (doorPos.segment_id == segmentsRoom[i].id)
                            {
                                Door newDoor = new Door();
                                newDoor.door = door;
                                newDoor.room = room;
                                newDoor.id = door.id;
                                newDoor.segment = segmentsRoom[i];
                                newDoor.position = doorPos.position;
                                newDoor.dir = doorPos.direction;
                                roomDoorWindowList[door.id.ToString()] = newDoor;
                            }
                        }
                    }

                    //Loop all windows
                    foreach (var window in windows)
                    {
                        var windowGeo = window.geometry;

                        if (windowGeo.width > 0)
                            width = (double)windowGeo.width;
                        if (windowGeo.height > 0)
                            heigh = (double)windowGeo.height;

                        //Loop in door position of each segment
                        foreach (var doorPos in window.geometry.positions)
                        {
                            if (doorPos.segment_id == segmentsRoom[i].id)
                            {
                                Door newDoor = new Door();
                                newDoor.door = window;
                                newDoor.room = room;
                                newDoor.id = window.id;
                                newDoor.segment = segmentsRoom[i];
                                newDoor.position = doorPos.position;
                                newDoor.dir = doorPos.direction;
                                roomDoorWindowList[window.id.ToString()] = newDoor;
                            }
                        }
                    }
                }
                return roomDoorWindowList;
            }
            return null;
        }
        /// <summary>
        /// This methos gets all DoorSurfaces of door by door/window by window.
        /// </summary>
        /// <param name="doorList"></param>
        /// <returns></returns>
        public Dictionary<string, DoorSurfaces> GetDoorSurfacesInRoom(Dictionary<string, Door> doorList)
        {
            if (doorList != null)
            {
                double width = GML.Core.Config.DefaultWidth;
                double heigh = GML.Core.Config.DefaultHeight;
                Dictionary<string, DoorSurfaces> doorSurfaceList = new Dictionary<string, DoorSurfaces>();

                foreach (var door in doorList)
                {
                    GetDoorSurfaces(door, width, heigh, ref doorSurfaceList);
                }
                return doorSurfaceList;
            }
            return null;
        }
        private void GetDoorSurfaces(KeyValuePair<string, Door> door, double width, double heigh, ref Dictionary<string, DoorSurfaces> doorSurfaceList)
        {
            Point3D doorCenter = door.Value.position;
            Point3D nearestPoint1 = door.Value.segment.points[0];
            Point3D nearestPoint2 = door.Value.segment.points[1];
            Point3D doorWidthDir1 = nearestPoint1 - nearestPoint2;
            doorWidthDir1.Normalize();

            if (door.Value.door.geometry.width > 0)
                width = (double)door.Value.door.geometry.width;
            if (door.Value.door.geometry.height > 0)
                heigh = (double)door.Value.door.geometry.height;

            //Get P1
            Point3D newNearestPoint1 = doorCenter + (doorWidthDir1 * width / 2);

            //Get P2
            Point3D doorWidthDir2 = new Point3D(-doorWidthDir1.x, -doorWidthDir1.y, -doorWidthDir1.z);
            doorWidthDir2.Normalize();
            Point3D newNearestPoint2 = doorCenter + (doorWidthDir2 * width / 2);

            //Get P3
            Point3D thicknessDoor = door.Value.dir;
            thicknessDoor.Normalize();
            double thickn = GML.Core.Config.DefaultThickness;
            if (door.Value.door.geometry.positions.Count == 2)
                thickn = (door.Value.door.geometry.positions[0].position - door.Value.door.geometry.positions[1].position).GetLength() / 2;
            //thicknessDoor = thicknessDoor * 0.5;
            thicknessDoor = thicknessDoor * thickn;
            Point3D newNearestPoint3 = newNearestPoint2 + thicknessDoor;

            //Get P4
            Point3D newNearestPoint4 = newNearestPoint1 + thicknessDoor;

            //get interior surface of door
            Point3D moveVec = new Point3D(0, 0, heigh);
            SurfaceMember interiorDoorSurface = GetSurface(newNearestPoint1, newNearestPoint2, moveVec);

            //get Exterior surfaces of door
            SurfaceMember exteriorDoorSurface1 = GetSurface(newNearestPoint2, newNearestPoint3, moveVec);
            SurfaceMember exteriorDoorSurface2 = GetSurface(newNearestPoint1, newNearestPoint4, moveVec);

            //get Exterior surface of door on roof
            Point3D newNearestPoint5 = newNearestPoint1 + moveVec;
            Point3D newNearestPoint6 = newNearestPoint2 + moveVec;
            SurfaceMember exteriorDoorSurface3 = GetSurface(newNearestPoint5, newNearestPoint6, thicknessDoor);

            //Insert all surfaces of door/Window to list
            DoorSurfaces doorSurfaces = new DoorSurfaces();
            doorSurfaces.id = door.Key;
            doorSurfaces.idWall = door.Value.segment.id.ToString();// + idExtend.ToString();
            doorSurfaces.doorSurfaceList.Add(interiorDoorSurface);
            doorSurfaces.doorSurfaceList.Add(exteriorDoorSurface1);
            doorSurfaces.doorSurfaceList.Add(exteriorDoorSurface2);
            doorSurfaces.doorSurfaceList.Add(exteriorDoorSurface3);


            //If door is Window -> need to add bottom surface
            if (door.Value.door.type == "Window")
            {
                SurfaceMember exteriorDoorSurface4 = GetSurface(newNearestPoint1, newNearestPoint2, thicknessDoor);
                doorSurfaces.doorSurfaceList.Add(exteriorDoorSurface4);
            }
            doorSurfaces.item = door.Value;

            //
            doorSurfaceList.Add(door.Key, doorSurfaces);
        }
        public bool IsHighestRoom(JRoom room, List<JRoom> rooms, List<JLevel> levels)
        {
            IEnumerable<JRoom> anotherRoomOfSameType = null;
            if (room.is_elevator == true)
            {
                anotherRoomOfSameType = rooms.Where(r => r.is_elevator == true);
            }
            else if (room.is_escalator == true)
            {
                anotherRoomOfSameType = rooms.Where(r => r.is_escalator == true);
            }
            else if (room.is_stair == true)
            {
                anotherRoomOfSameType = rooms.Where(r => r.is_stair == true);
            }
            else
            {
                return false;
            }

            var currentRoomLevel = levels.Where(l => l.id == room.level_id).FirstOrDefault();
            if (currentRoomLevel == null)
                return false;
            var currentRoomElevation = currentRoomLevel.elevation;

            var centerX = (room.geometry.bouding_box.min.x + room.geometry.bouding_box.max.x) / 2;
            var centerY = (room.geometry.bouding_box.min.y + room.geometry.bouding_box.max.y) / 2;

            foreach (var another in anotherRoomOfSameType)
            {
                var lv = levels.Where(l => l.id == another.level_id).FirstOrDefault();
                if (lv == null)
                    continue;

                var _centerX = (another.geometry.bouding_box.min.x + another.geometry.bouding_box.max.x) / 2;
                var _centerY = (another.geometry.bouding_box.min.y + another.geometry.bouding_box.max.y) / 2;

                var deltaX = Math.Abs(_centerX - centerX) * Config.Scale;
                var deltaY = Math.Abs(_centerY - centerY) * Config.Scale;
                if (lv.elevation > currentRoomElevation && deltaX < 1000 && deltaY < 1000)
                    return false;
            }

            return true;
        }
        public bool IsLowestRoom(JRoom room, List<JRoom> rooms, List<JLevel> levels)
        {
            IEnumerable<JRoom> anotherRoomOfSameType = null;
            if (room.is_elevator == true)
            {
                anotherRoomOfSameType = rooms.Where(r => r.is_elevator == true);
            }
            else if (room.is_escalator == true)
            {
                anotherRoomOfSameType = rooms.Where(r => r.is_escalator == true);
            }
            else if (room.is_stair == true)
            {
                anotherRoomOfSameType = rooms.Where(r => r.is_stair == true);
            }
            else
            {
                return false;
            }



            var currentRoomLevel = levels.Where(l => l.id == room.level_id).FirstOrDefault();
            if (currentRoomLevel == null)
                return false;
            var currentRoomElevation = currentRoomLevel.elevation;

            var centerX = (room.geometry.bouding_box.min.x + room.geometry.bouding_box.max.x) / 2;
            var centerY = (room.geometry.bouding_box.min.y + room.geometry.bouding_box.max.y) / 2;

            foreach (var another in anotherRoomOfSameType)
            {
                var lv = levels.Where(l => l.id == another.level_id).FirstOrDefault();
                if (lv == null)
                    continue;

                var _centerX = (another.geometry.bouding_box.min.x + another.geometry.bouding_box.max.x) / 2;
                var _centerY = (another.geometry.bouding_box.min.y + another.geometry.bouding_box.max.y) / 2;

                var deltaX = Math.Abs(_centerX - centerX) * Config.Scale;
                var deltaY = Math.Abs(_centerY - centerY) * Config.Scale;
                if (lv.elevation < currentRoomElevation && deltaX < 1000 && deltaY < 1000)
                    return false;
            }

            return true;
        }
    }
}
