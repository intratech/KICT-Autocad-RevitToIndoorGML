using IndoorGML.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IndoorGML.Adapter.Entity
{
    public class IndoorModel
    {
        public string ModelName { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public List<IndoorRoom> Rooms { get; set; }
        public List<IndoorConnectionSpace> Doors { get; set; }

        public List<Connection> ConnectionRoomToDoor { get; set; }

        public IndoorModel(string modelName)
        {
            ModelName = "M_" + modelName.GetHashCode();
            Rooms = new List<IndoorRoom>();
            Doors = new List<IndoorConnectionSpace>();
            ConnectionRoomToDoor = new List<Connection>();
        }

        internal int AddRoom(IndoorRoom cellSpace)
        {
            int id = Rooms.Count;
            Rooms.Add(cellSpace);
            return id;
        }

        internal int AddDoor(IndoorConnectionSpace door)
        {
            int id = Doors.Count;
            Doors.Add(door);
            return id;
        }

        public void AddConnection(int fromRoom, int toDoor, ConnectionType type)
        {
            Connection connection = new Connection();
            connection.From = fromRoom;
            connection.To = toDoor;
            connection.ConnectionType = type;
            ConnectionRoomToDoor.Add(connection);
        }

        internal void Clear()
        {
            ConnectionRoomToDoor.Clear();
            Rooms.Clear();
            Doors.Clear();
        }

        public List<IndoorRoom> GetconenctedRoom(int doorID)
        {

            //Get room connected to door
            var rooms = ConnectionRoomToDoor.Where(x =>
                    x.ConnectionType == ConnectionType.RoomToDoor &&
                    x.To == doorID
            ).Select(y => Rooms[y.From]).ToList();
            return rooms;
        }
        public IndoorFeatures Convert(float scale)
        {
            IndoorEntity.Scale = scale;


            IndoorFeatures indoor = new IndoorFeatures();
            indoor.Name = Name;
            indoor.Description = Description;

            foreach (var room in Rooms)
            {
                var cell = indoor.AddCellSpace(room.ToCellSpaceMember());
                var state = indoor.AddStateMember(room.ToStateMember());

                cell.GeneralSpace.Name = room.DisplayName;
                cell.GeneralSpace.Description = $"function = \"{room.Function}\" : class =\"{room.Type}\"";// {room.Type} of {room.Name}";
                cell.GeneralSpace.Level = room.Level ;
                cell.GeneralSpace.Duality = new Duality() { Href = $"#{state.State.Id}" };

                cell.GeneralSpace.Class = room.Type;
                cell.GeneralSpace.Function = room.Function;
                cell.GeneralSpace.Usage = room.Usage;
                
                //if(room.SpaceType != null)
                //{
                //    cell.GeneralSpace.Class = room.Type;
                //    cell.GeneralSpace.Function = room.Function;
                //    cell.GeneralSpace.Usage = room.Usage;
                //}
                //else if (room.Type == "Stair")
                //{
                //    cell.GeneralSpace.Class = "1010";
                //    cell.GeneralSpace.Function = "1120";
                //    cell.GeneralSpace.Usage = "1120";                    
                //}
                //else if(room.Type =="Elevator")
                //{
                //    cell.GeneralSpace.Class = "1010";
                //    cell.GeneralSpace.Function = "1110";
                //    cell.GeneralSpace.Usage = "1110";
                //}
                //else if(room.Type =="Room")
                //{
                //    cell.GeneralSpace.Class = "1020";
                //    cell.GeneralSpace.Function = "2550";
                //    cell.GeneralSpace.Usage = "2550";
                //}

                state.State.Duality = new Duality() { Href = $"#{cell.GeneralSpace.Id}" };

            }

            for (int i = 0; i < Doors.Count; i++)
            {
                var door = Doors[i];
                var connectedRooms = GetconenctedRoom(i);
                var backDoor = door.GetBackDoor();
                if (backDoor != null && !door.IsSelfOverlap(0.01f))
                {
                    //Create cell
                    CreateCellSpaceForDoor(door, indoor, connectedRooms);
                }
                else
                {
                    for (int j = 0; j < connectedRooms.Count - 1; j++)
                    {
                        var fromRoom = connectedRooms[j];
                        var toRoom = connectedRooms[j + 1];
                        CreateConnectionRoomToRoom(fromRoom, toRoom, door, indoor);

                    }
                }
                continue;
                //No need to export partial boundary
                #region CellSpaceBoundary for wall , or thin door

                var cellBoundaryFront = indoor.AddCellSpaceBoundarMember(door.GetFrontDoor());
                //cell.GeneralSpace.AddPartialbounded(cellBoundaryFront.ConnectionBoundary);


                if (backDoor != null)
                {
                    indoor.AddCellSpaceBoundarMember(backDoor);
                }
                //cell.GeneralSpace.AddPartialbounded(cellBoundaryBack.ConnectionBoundary);

                foreach (var room in connectedRooms)
                {
                    var space = indoor.GetGeneralSpace(room.Id);
                    space?.GeneralSpace.AddPartialbounded(cellBoundaryFront.ConnectionBoundary);

                    if (backDoor != null)
                    {
                        space?.GeneralSpace.AddPartialbounded(backDoor.ConnectionBoundary);
                    }
                }

                
                #endregion
                //indoor.AddConnectionSpaceToDoor(connection.FromRoom, connection.ToDoor);
            }

            foreach (var connection in ConnectionRoomToDoor)
            {
                //Create path from  room to door 
                switch (connection.ConnectionType)
                {
                    case ConnectionType.RoomToDoor:
                        //CreateConnectionRooToDoor(connection,indoor);
                        break;
                    case ConnectionType.RoomToRoom:
                        CreateConnectionRoomToRoom(connection, indoor);
                        break;
                    default:
                        break;
                }

            }

            return indoor;

        }

        private void CreateCellSpaceForDoor(IndoorConnectionSpace door, IndoorFeatures indoor, List<IndoorRoom> connectedRooms)
        {

            var cell = indoor.AddCellSpace(door.ToCellSpaceMember());
            cell.GeneralSpace.Name = door.DisplayName;
            cell.GeneralSpace.Description = $"function = \"{door.Function}\" : class =\"{door.Type}\"";// {room.Type} of {room.Name}";//$"{door.Type} of {door.Name}";
            var state = indoor.AddStateMember(door.ToStateMember());
            cell.GeneralSpace.Duality = new Duality() { Href = $"#{state.State.Id}" };
            state.State.Duality = new Duality() { Href = $"#{cell.GeneralSpace.Id}" };

            cell.GeneralSpace.Usage = door.Usage;
            cell.GeneralSpace.Class = door.Type;
            cell.GeneralSpace.Function = door.Function;
            cell.GeneralSpace.Level = door.Level;
            
            foreach (var room in connectedRooms)
            {
                CreateConnectionRoomToDoor(room, door, indoor);
            }
        }

        private void CreateConnectionRoomToRoom(Connection connection, IndoorFeatures indoor)
        {
            var room = Rooms[connection.From];
            var nextRoom = Rooms[connection.To];
            var state = indoor.GetState(room.StateID);
            var stateDoor = indoor.GetState(nextRoom.StateID);
            if (state != null && stateDoor != null)
            {
                Transition transation = new Transition();
                transation.Id = $"T{room.Id}-{nextRoom.Id}";
                transation.AddConnect(room.DualityToState);
                transation.AddConnect(nextRoom.DualityToState);
                //transation.Duality = new Duality() { Href = $"#{door.BoundaryID}" };
                transation.Geometry = ConvertGeo(state, stateDoor, $"TG-{transation.Id}");

                try
                {
                    var middlePoint = FindTheMiddlePointOfPath(state.State.Geometry.Point.Pos, stateDoor.State.Geometry.Point.Pos, room);
                    if (middlePoint != null)
                    {
                        transation.Geometry.LineString.Pos.Insert(1, new Pos(middlePoint.Value.X, middlePoint.Value.Y, middlePoint.Value.Z));
                    }
                }
                catch(Exception ex)
                {

                }

                indoor.AddTransition(transation);

                state.State.AddConnect(transation);
                stateDoor.State.AddConnect(transation);
            }
        }

        private void CreateConnectionRoomToRoom(IndoorRoom room, IndoorRoom nextRoom, IndoorConnectionSpace connect, IndoorFeatures indoor)
        {
            var state = indoor.GetState(room.StateID);
            var nextState = indoor.GetState(nextRoom.StateID);
            if (state != null && nextState != null)
            {
                Transition transation = new Transition();
                transation.Id = $"T{room.Id}-{connect.Id}-{nextRoom.Id}";
                transation.AddConnect(room.DualityToState);
                transation.AddConnect(nextRoom.DualityToState);
                //transation.Duality = new Duality() { Href = $"#{door.BoundaryID}" };
                transation.Geometry = ConvertGeo(state, nextState, $"TG-{transation.Id}");
                transation.Geometry.LineString.Pos.Insert(1, connect.GetCenterPos());

                //0,1,2

                var point1 = FindTheMiddlePointOfPath(transation.Geometry.LineString.Pos[0], transation.Geometry.LineString.Pos[1], room);
                var point2 = FindTheMiddlePointOfPath(transation.Geometry.LineString.Pos[1], transation.Geometry.LineString.Pos[2], nextRoom);
                if (point2 != null)
                {
                    transation.Geometry.LineString.Pos.Insert(2, new Pos(point2.Value.X, point2.Value.Y, point2.Value.Z));
                }
                if (point1 != null)
                {
                    transation.Geometry.LineString.Pos.Insert(1, new Pos(point1.Value.X, point1.Value.Y, point1.Value.Z));
                }

                indoor.AddTransition(transation);

                state.State.AddConnect(transation);
                nextState.State.AddConnect(transation);
            }
        }

        private void CreateConnectionRooToDoor(Connection connection, IndoorFeatures indoor)
        {
            var room = Rooms[connection.From];
            var door = Doors[connection.To];
            var state = indoor.GetState(room.StateID);
            var stateDoor = indoor.GetState(door.StateID);
            if (state != null && stateDoor != null)
            {
                Transition transation = new Transition();
                transation.Id = $"T{room.Id}-{door.Id}";
                transation.AddConnect(room.DualityToState);
                transation.AddConnect(door.DualityToState);
                transation.Duality = new Duality() { Href = $"#{door.BoundaryID}" };
                transation.Geometry = ConvertGeo(state, stateDoor, $"TG-{transation.Id}");

                var point1 = FindTheMiddlePointOfPath(transation.Geometry.LineString.Pos[0], transation.Geometry.LineString.Pos[1], room);
                if (point1 != null)
                {
                    transation.Geometry.LineString.Pos.Insert(1, new Pos(point1.Value.X, point1.Value.Y, point1.Value.Z));
                }
                indoor.AddTransition(transation);
            }
        }

        private void CreateConnectionRoomToDoor(IndoorRoom room, IndoorConnectionSpace door, IndoorFeatures indoor)
        {
            var state = indoor.GetState(room.StateID);
            var stateDoor = indoor.GetState(door.StateID);
            if (state != null && stateDoor != null)
            {
                Transition transation = new Transition();
                transation.Id = $"T{room.Id}-{door.Id}";
                transation.AddConnect(room.DualityToState);
                transation.AddConnect(door.DualityToState);
                transation.Duality = new Duality() { Href = $"#{door.BoundaryID}" };
                transation.Geometry = ConvertGeo(state, stateDoor, $"TG-{transation.Id}");
                var point1 = FindTheMiddlePointOfPath(transation.Geometry.LineString.Pos[0], transation.Geometry.LineString.Pos[1], room);
                
                if (point1 != null)
                {
                    transation.Geometry.LineString.Pos.Insert(1, new Pos(point1.Value.X, point1.Value.Y, point1.Value.Z));
                }
                indoor.AddTransition(transation);
            }
        }

        private Geometry ConvertGeo(StateMember state, StateMember stateDoor, string id)
        {
            Geometry geo = new Geometry();
            geo.LineString = new LineString();
            geo.LineString.Id = id;
            geo.LineString.Pos.Add(state.State.Geometry.Point.Pos);
            geo.LineString.Pos.Add(stateDoor.State.Geometry.Point.Pos);

            return geo;
        }

        private Vector3? FindTheMiddlePointOfPath(Pos from, Pos to, IndoorEntity room)
        {
            try
            {
                var start = new Vector3((float)from.x, (float)from.y, (float)from.z);
                var end = new Vector3((float)to.x, (float)to.y, (float)to.z);
                Vector3 middle;
                if (FindTheMiddlePointOfPath(start, end, room.Boundary.Points, out middle))
                {
                    middle.Z = (start.Z+end.Z)/2;
                    return middle;
                }
            }
            catch { }

            return null;
        }

        public bool FindTheMiddlePointOfPath(Vector3 p1, Vector3 p2, IList<Vector3> polygon, out Vector3 middle)
        {
            middle = new Vector3();

            if (polygon == null || polygon.Count < 3)
            {
                return false;
            }
            double epsilon = 0.0001;

            var min = new Vector3();
            min.X = Math.Min(p1.X, p2.X);
            min.Y = Math.Min(p1.Y, p2.Y);

            Vector3 max = new Vector3();
            max.X = Math.Max(p1.X, p2.X);
            max.Y = Math.Max(p1.Y, p2.Y);

            var points = new Vector3[4]
            {
                min,new Vector3(min.X,max.Y,0),new Vector3(max.X,min.Y,0),max
            };

            for (int i = 0; i < points.Length; i++)
            {
                var p = points[i];
                if (DistanceXY(p,p1) <= epsilon)
                    continue;
                if (DistanceXY(p, p2) <= epsilon)
                    continue;

                if (PolyUtility.PointInPolygon(p, polygon))
                {
                    middle = p;
                    return true;
                }
            }
            middle = new Vector3(0, 0, 0);
            return false;
        }

        private double DistanceXY(Vector3 p, Vector3 p1)
        {
            var size = (p - p1);
            size.Z = 0;
            return size.Length();
        }
    }


    public static class PolyUtility
    {
        public static bool PointInPolygon(Vector3 point, IList<Vector3> poly)
        {
            double X = point.X;
            double Y = point.Y;

            // Get the angle between the point and the
            // first and last vertices.
            int max_point = poly.Count - 1;
            var p = poly[max_point];
            var p0 = poly[0];
            double total_angle = GetAngle(p.X, p.Y, X, Y, p0.X, p0.Y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++)
            {
                p = poly[i];
                var p1 = poly[i + 1];
                total_angle += GetAngle(p.X, p.Y, X, Y, p1.X, p1.Y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            // The following statement was changed. See the comments.
            //return (Math.Abs(total_angle) > 0.000001);
            return (Math.Abs(total_angle) > 1);
        }



        public static double CrossProductLength(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }

        public static double GetAngle(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the dot product.
            double dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            double cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return Math.Atan2(cross_product, dot_product);
        }
        private static double DotProduct(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
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
