using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using IndoorGML.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using TransitionMember = IndoorGML.Core.TransitionMember;

namespace CityGML.Exporter.AutoCAD.IndoorData
{
    public class IndoorRoom : IndoorEntity
    {
        public string Name { get; set; }
        public Polyline Boundary { get; set; }
        
        public float Elevation { get; set; }
        public float Height { get; set; }

        private List<Point3d> points = new List<Point3d>();
        
        

        public string DualityToState
        {
            get => $"#{StateID}";
        }

        public string StateID
        {
            get => $"S{Name}";
        }
        public string Id
        {
            get => $"C{Name}";
        }
        public string DualityToCellSpace
        {
            get => $"#{Id}";
        }
        public string StateGeoId
        {
            get => $"SG-{StateID}";
        }
        internal CellSpaceMember ToCellSpaceMember()
        {
            CellSpaceMember member = new CellSpaceMember();
            member.GeneralSpace = new CellSpace();
            member.GeneralSpace.Id = Id;
            member.GeneralSpace.Duality = new Duality() { Href = DualityToState };
            if(points.Count ==0)
                member.GeneralSpace.CellSpaceGeometry = ConvertGeo(Boundary, Id, Elevation, Height);
            else
                member.GeneralSpace.CellSpaceGeometry = ConvertGeo(points, Id, Elevation, Height);
            return member;
        }

     

        internal StateMember ToStateMember()
        {
            StateMember state = new StateMember();
            state.State.Id = StateID;
            state.State.Duality = new Duality() { Href= DualityToCellSpace};
            state.State.Geometry = ConvertPoint();
            return state;

        }

        private Geometry ConvertPoint()
        {
            Geometry geo = new Geometry();
            geo.Point = new Point();
            geo.Point.Id = StateGeoId;
            geo.Point.Pos = GetCenterPos();
            return geo;
        }

        private Pos GetCenterPos()
        {
            if(Boundary.Bounds.HasValue)
            {
                var minPos = Boundary.Bounds.Value.MinPoint;
                var maxPos = Boundary.Bounds.Value.MaxPoint;



                return new Pos((minPos.X + maxPos.X) * 0.5f * API.Scale,
                                (minPos.Y + maxPos.Y) * 0.5f * API.Scale,
                                Elevation);

            }
            double x=0, y=0;
            for (int i=0;i< Boundary.NumberOfVertices;i++)
            {
                var p = Boundary.GetPoint2dAt(i);
                x += p.X * API.Scale;
                y += p.Y * API.Scale;
            }

            return new Pos(x / Boundary.NumberOfVertices, y / Boundary.NumberOfVertices, Elevation);

        }

        internal TransitionMember ToTransition(IndoorConnection door)
        {
            TransitionMember transition = new TransitionMember();
            transition.Transition.Duality = new Duality() { Href = $"#{door.Id}" };
            transition.Transition.Id = door.TransationId;
            transition.Transition.Connects = new List<Connects>();
            transition.Transition.Connects.Add(new Connects() { Href = $"#{StateID}" });
            transition.Transition.Geometry = GetTransationGeoToDoor(door);
            return transition;
        }

        private Geometry GetTransationGeoToDoor(IndoorConnection door)
        {
            Geometry geo = new Geometry();
            geo.LineString = new LineString();
            geo.LineString.Id = $"TG-{door.TransationId}";
            geo.LineString.Pos = new List<Pos>();
            geo.LineString.Pos.Add(GetCenterPos());
            geo.LineString.Pos.Add(door.GetCenterPos());
            return geo;
        }

        private void InitPoints()
        {
            for (int i = 0; i < Boundary.NumberOfVertices; i++)
            {
                points.Add(Boundary.GetPoint3dAt(i));
            }
        }
        internal void UpdateBoundary(Autodesk.AutoCAD.Geometry.Point3dCollection polyline)
        {
           if(points.Count == 0)
            {
                InitPoints();
            }
            for (int i = 1; i < polyline.Count - 1; i++)
            {                
                int numVertex = points.Count;
                for(int j=numVertex-1;j>=0;j--)
                {
                    if (points[j].DistanceTo(polyline[i]) <= 100)
                        points.RemoveAt(j);
                }
              
            }
            
        }

        internal Point3dCollection GetPoints()
        {
            if(points != null && points.Count >0)
            {
                Point3dCollection collection = new Point3dCollection();
                points.ForEach(x => collection.Add(x));
                return collection;
            }
            else
            {
                Point3dCollection collection = new Point3dCollection();
                for(int i=0;i<Boundary.NumberOfVertices;i++)
                {
                    collection.Add(Boundary.GetPoint3dAt(i));
                    
                }
                return collection;
            }
        }
    }

    public class IndoorConnection : IndoorEntity
    {
        public string Name { get; set; }
        public Line Boundary { get; set; }

        public float Elevation { get; set; }
        public float Height { get; set; }
        public double[] Box { get; internal set; }

        public string Id
        {
            get => $"B{Name}";
        }

        public string DualityToTransation
        {
            get => $"#T{Name}";
        }
        public string TransationId
        {
            get => $"T{Name}";
        }
        public Autodesk.AutoCAD.Geometry.Point3dCollection Polyline { get; internal set; }

        public Pos GetCenterPos()
        {
            var start = Boundary.StartPoint;
            var end = Boundary.EndPoint;
            return new Pos((start.X + end.X)*0.5f*API.Scale, (start.Y + end.Y) * 0.5f * API.Scale, Elevation);
        }

        internal CellSpaceBoundaryMember ToCellSpaceBoundaryMember()
        {
            CellSpaceBoundaryMember cellBoundary = new CellSpaceBoundaryMember();
            cellBoundary.ConnectionBoundary = new ConnectionBoundary();
            cellBoundary.ConnectionBoundary.Id = Id;
            cellBoundary.ConnectionBoundary.Description = "No description";
            cellBoundary.ConnectionBoundary.CellSpaceBoundaryGeometry = ConvertBoundaryGeometry(Boundary, Id, Elevation, Height);
            cellBoundary.ConnectionBoundary.Duality = new Duality() { Href = DualityToTransation };

            return cellBoundary;
        }
    }

    public class IndoorModel
    {
        private string modelName = "";
        public string ModelName
        {
            get
            {
                if (string.IsNullOrEmpty(modelName))
                    modelName = API.DocName;
                return modelName;
            }
        }
        public List<IndoorRoom> Rooms { get; set; }
        public List<IndoorConnection> Doors { get; set; }

        public List<Connection> Connections { get; set; }

        public IndoorModel()
        {
            Rooms = new List<IndoorRoom>();
            Doors = new List<IndoorConnection>();
            Connections = new List<Connection>();
        }

        internal int AddRoom(IndoorRoom cellSpace)
        {
            int id = Rooms.Count;
            cellSpace.Name = ModelName+"_Room_" + id.ToString();
            Rooms.Add(cellSpace);
            return id;
        }

        internal int AddDoor(IndoorConnection window)
        {
            int id = Doors.Count;
            window.Name = ModelName + "_Door_" + id.ToString();
            Doors.Add(window);
            return id;
        }

        public void AddConnection(int fromRoom, int toDoor)
        {
            Connection connection = new Connection();
            connection.FromRoom = fromRoom;
            connection.ToDoor = toDoor;
            Connections.Add(connection);
        }

        internal void Clear()
        {
            Connections.Clear();
            Rooms.Clear();
            Doors.Clear();
        }
    }

    public class Connection
    {
        public int FromRoom { get; set; }
        public int ToDoor { get; set; }
    }
}
