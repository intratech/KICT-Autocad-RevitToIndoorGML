using IndoorGML.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void AddConnection(int fromRoom, int toDoor,ConnectionType type)
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
                cell.GeneralSpace.Description = $"{room.Type} of {room.Name}";

                cell.GeneralSpace.Duality = new Duality() { Href = $"#{state.State.Id}" };

                if (room.Type == "Stair")
                {
                    cell.GeneralSpace.Class = "1010";
                    cell.GeneralSpace.Function = "1120";
                    cell.GeneralSpace.Usage = "1120";                    
                }
                else if(room.Type =="Elevator")
                {
                    cell.GeneralSpace.Class = "1010";
                    cell.GeneralSpace.Function = "1110";
                    cell.GeneralSpace.Usage = "1110";
                }
                else if(room.Type =="Room")
                {
                    cell.GeneralSpace.Class = "1020";
                    cell.GeneralSpace.Function = "2550";
                    cell.GeneralSpace.Usage = "2550";
                }

                state.State.Duality = new Duality() { Href = $"#{cell.GeneralSpace.Id}" };

            }

            for(int i=0;i<Doors.Count;i++)
            {
                var door = Doors[i];

                //var cell = indoor.AddCellSpace(door.ToCellSpaceMember());
                //cell.GeneralSpace.Name = door.DisplayName;
                //cell.GeneralSpace.Description = $"{door.Type} of {door.Name}";

                //var state = indoor.AddStateMember(door.ToStateMember());
                //cell.GeneralSpace.Duality = new Duality() { Href = $"#{state.State.Id}" };
                //state.State.Duality = new Duality() { Href = $"#{cell.GeneralSpace.Id}" };
                var rooms = ConnectionRoomToDoor.Where(x =>
                        x.ConnectionType == ConnectionType.RoomToDoor &&
                        x.To == i
                ).Select(y => Rooms[y.From]).ToList();


                var cellBoundaryFront = indoor.AddCellSpaceBoundarMember(door.GetFrontDoor());
                //cell.GeneralSpace.AddPartialbounded(cellBoundaryFront.ConnectionBoundary);

                var backDoor = door.GetBackDoor();
                if (backDoor != null)
                {
                    indoor.AddCellSpaceBoundarMember(backDoor);
                }
                //cell.GeneralSpace.AddPartialbounded(cellBoundaryBack.ConnectionBoundary);

                foreach(var room in rooms)
                {
                    var space = indoor.GetGeneralSpace(room.Id);
                    space?.GeneralSpace.AddPartialbounded(cellBoundaryFront.ConnectionBoundary);
                   
                    if (backDoor != null)
                    {
                        space?.GeneralSpace.AddPartialbounded(backDoor.ConnectionBoundary);
                    }
                }

                for(int j =0;j<rooms.Count-1;j++)
                {
                    var fromRoom = rooms[j];
                    var toRoom = rooms[j + 1];
                    CreateConnectionRoomToRoom(fromRoom, toRoom, door, indoor);

                }
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
                indoor.AddTransition(transation);

                state.State.AddConnect(transation);
                stateDoor.State.AddConnect(transation);
            }
        }

        private void CreateConnectionRoomToRoom(IndoorRoom room, IndoorRoom nextRoom, IndoorConnectionSpace connect, IndoorFeatures indoor)
        {
            var state = indoor.GetState(room.StateID);
            var stateDoor = indoor.GetState(nextRoom.StateID);
            if (state != null && stateDoor != null)
            {
                Transition transation = new Transition();
                transation.Id = $"T{room.Id}-{connect.Id}-{nextRoom.Id}";
                transation.AddConnect(room.DualityToState);
                transation.AddConnect(nextRoom.DualityToState);
                //transation.Duality = new Duality() { Href = $"#{door.BoundaryID}" };
                transation.Geometry = ConvertGeo(state, stateDoor, $"TG-{transation.Id}");
                transation.Geometry.LineString.Pos.Insert(1, connect.GetCenterPos());
                indoor.AddTransition(transation);

                state.State.AddConnect(transation);
                stateDoor.State.AddConnect(transation);
            }
        }

        private void CreateConnectionRooToDoor(Connection connection,IndoorFeatures indoor)
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
    }

}
