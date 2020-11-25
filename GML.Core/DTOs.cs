using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Point3DIntra;

namespace GML.Core.DTO.Json
{
    public class JBaseItem
    {
        public long id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public int level_id { get; set; }

        public int JID = -1;
    }


    [Serializable]
    public class JModel
    {
        public string description;

        public string model_name { get; set; }
        public JBoundingBox bounding_box { get; set; }
        public double scale { get; set; }
        public List<JLevel> levels { get; set; }
        public List<JRoom> rooms { get; set; }
        public List<JDoor> doors { get; set; }
        public List<JWindow> windows { get; set; }
    }
    [Serializable]
    public class JLevel
    {
        public string name { get; set; }
        public int id { get; set; }
        public double elevation { get; set; }
    }
    [Serializable]
    public class JBoundingBox
    {
        public Point3D min { get; set; }
        public Point3D max { get; set; }

        public JBoundingBox() { }
        public JBoundingBox(Point3D min, Point3D max)
        {
            this.min = min;
            this.max = max;
        }
    }
    [Serializable]
    public class JRoom : JBaseItem
    {



        public bool? is_elevator { get; set; }
        public bool? is_escalator { get; set; }
        public bool? is_stair { get; set; }
        public JRoomGeometry geometry { get; set; }
        public List<JProp> properties { get; set; }
        public float RoomHeight
        {
            get
            {
                return geometry != null ? (float)(geometry.bouding_box.max.z - geometry.bouding_box.min.z) : 0;
            }
        }

        public bool IsNextElevation(JRoom nextStair)
        {
            return nextStair.Elevation > this.Elevation;// < Config.ElevationTolerance;
        }

      

        public double Elevation
        {
            get
            {
                if (geometry != null)
                    return geometry.bouding_box.min.z;
                return 0;
            }
        }


        public bool SameXY(JRoom nextStair, double tolerance)
        {
            var diff = GetCenterPoint() - nextStair.GetCenterPoint();
            return (Math.Abs(diff.x) < tolerance && Math.Abs(diff.y) < tolerance);

        }

        public virtual Floor GetFloor()
        {
            Floor floor = new Floor();
            var boundary = this.geometry.boundary;
            if (boundary != null)
            {
                for (var i = 0; i < boundary.Count; i++)
                {
                    Point3D startPoint = new Point3D((float)boundary[i].points[0].x, (float)boundary[i].points[0].y, (float)boundary[i].points[0].z);
                    Point3D endPoint = new Point3D((float)boundary[i].points[1].x, (float)boundary[i].points[1].y, (float)boundary[i].points[1].z);
                    if (i == 0)
                    {
                        floor.surface.points.Add(startPoint);
                    }
                    floor.surface.points.Add(endPoint);
                }
                floor.id = ObjName.Floor.ToString() + this.id.ToString();
                return floor;
            }
            return null;
        }

        public List<JWindow> GetWindows(JModel model)
        {
            if (model.windows != null)
            {
                //Create list of door inside Room

                //Check id of room is exist in door and get door's list
                var segmentsRoom = geometry.boundary;

                return model.windows.Where(x => x.geometry.HasSegment(geometry.boundary)).ToList();

            }
            return null;
        }

        public List<JDoor> GetDoors(JModel model)
        {
            if (model.doors != null)
            {
                //Create list of door inside Room

                //Check id of room is exist in door and get door's list
                var segmentsRoom = geometry.boundary;

                return model.doors.Where(x => x.geometry.HasSegment(geometry.boundary)).ToList();

            }
            return null;
        }

        public virtual Ceiling GetCeiling()
        {
            Ceiling ceiling = new Ceiling();
            var boundary = this.geometry.boundary;
            if (boundary != null)
            {
                for (var i = 0; i < boundary.Count; i++)
                {
                    Point3D startPoint = new Point3D((float)boundary[i].points[0].x, (float)boundary[i].points[0].y, (float)boundary[i].points[0].z);
                    Point3D endPoint = new Point3D((float)boundary[i].points[1].x, (float)boundary[i].points[1].y, (float)boundary[i].points[1].z);
                    if (i == 0)
                    {
                        Point3D newStartPoint = new Point3D(startPoint.x, startPoint.y, 0);
                        newStartPoint.z = this.geometry.bouding_box.max.z;
                        ceiling.surface.points.Add(newStartPoint);
                    }
                    Point3D newEndPoint = new Point3D(endPoint.x, endPoint.y, 0);
                    newEndPoint.z = this.geometry.bouding_box.max.z;
                    ceiling.surface.points.Add(newEndPoint);
                }
                ceiling.id = ObjName.Ceiling.ToString() + this.id.ToString();
                return ceiling;
            }
            return null;
        }
        public virtual List<string> GetWallIDListExcludeCS()
        {
            List<string> wallIDList = new List<string>();
            var boundaries = this.geometry.boundary;
            foreach (var seg in boundaries)
            {
                if (seg.isClosureSurface != true)
                    wallIDList.Add(seg.id.ToString());
            }
            return wallIDList;
        }

        public List<SurfaceMember> GetRoomSurfaces()
        {
            var ceiling = this.GetCeiling();
            var floor = this.GetFloor();
            var surfaces = new List<SurfaceMember>();
            if (ceiling != null && ceiling.surface != null && ceiling.surface.points != null && ceiling.surface.points.Count > 2)
                surfaces.Add(ceiling.surface);
            if (floor != null && floor.surface != null && floor.surface.points != null && floor.surface.points.Count > 2)
                surfaces.Add(floor.surface);

            var gmlUtil = new GMLUtil();

            //walls surfaces
            if (geometry != null && geometry.boundary != null)
            {
                var Jbox = geometry.bouding_box;
                Point3D moveVec = new Point3D(0, 0, Jbox.max.z - Jbox.min.z);

                var boundary = geometry.boundary;
                if (boundary != null)
                {
                    //Write wall surfaces
                    for (var i = 0; i < boundary.Count; i++)
                    {
                        Point3D startPoint = new Point3D((float)boundary[i].points[0].x, (float)boundary[i].points[0].y, (float)boundary[i].points[0].z);
                        Point3D endPoint = new Point3D((float)boundary[i].points[1].x, (float)boundary[i].points[1].y, (float)boundary[i].points[1].z);

                        //Create surfaces of walls from points
                        SurfaceMember surface = gmlUtil.GetSurface(startPoint, endPoint, moveVec);
                        if (surface != null)
                            surfaces.Add(surface);
                    }
                }
            }

            return surfaces;
        }

        public Point3D GetCenterPoint()
        {
            var box = geometry.bouding_box;
            return new Point3D(box.min.x / 2 + box.max.x / 2, box.min.y / 2 + box.max.y / 2, box.min.z);
        }

        public bool HasConnect(JDoor jDoor)
        {
            return jDoor.roomIds.Contains(id);
            //return true;
            //return jDoor.geometry.HasSegment(geometry.boundary);
        }
    }

    public class Ceiling
    {
        public string id;
        public SurfaceMember surface = new SurfaceMember();
        public Ceiling(Ceiling ceiling)
        {
            id = ceiling.id;
            surface = ceiling.surface;
        }
        public Ceiling() { }
    }
    public class Floor : Ceiling
    {
    }

    [Serializable]
    public class JProp
    {
        public string name { get; set; }
        public string value { get; set; }

        public JProp(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }

    [Serializable]
    public class JRoomGeometry
    {
        public JBoundingBox bouding_box { get; set; }
        public Point3D position { get; set; }
        public List<JLineSegment> boundary { get; set; }

        public void CaculatorBox(double height)
        {
            if (boundary != null)
            {
                bouding_box = new JBoundingBox();
                bouding_box.min = new Point3D();
                bouding_box.min.x = boundary.Min(x => x.points.Min(y => y.x));
                bouding_box.min.y = boundary.Min(x => x.points.Min(y => y.y));
                bouding_box.min.z = boundary.Min(x => x.points.Min(y => y.z));


                bouding_box.max = new Point3D();
                bouding_box.max.x = boundary.Max(x => x.points.Max(y => y.x));
                bouding_box.max.y = boundary.Max(x => x.points.Max(y => y.y));
                bouding_box.max.z = boundary.Max(x => x.points.Max(y => y.z)) + height;
            }
        }
    }
    [Serializable]
    public class JRoomBoundary
    {
        public List<JLineSegment> segments { get; set; }
        //public List<JCurve> walls { get; set; }
    }
    [Serializable]
    public class JLineSegment
    {
        public int id { get; set; }
        //public int? id2 { get; set; }
        public bool? isClosureSurface { get; set; }
        public List<Point3D> points { get; set; }
        public string gmlID { get; set; }
        public double Length
        {
            get
            {
                double length = 0;
                for(int i=0;i<points.Count-1;i++)
                {
                    length += points[i].distance(points[i + 1]);
                }
                return length;
            }
        }

        public JLineSegment()
        {
            this.gmlID = $"GML_{Guid.NewGuid().ToString()}";
        }
        public JLineSegment(JLineSegment segment)
        {
            id = segment.id;
            isClosureSurface = segment.isClosureSurface;
            points = segment.points;
            gmlID = segment.gmlID;
        }

        public bool Contains(Point3D p, double tolerance = 0.001)
        {
            if(points.Count >=2)
            {
                return points.First().distance(p) + points.Last().distance(p) - points.First().distance(points.Last()) < tolerance;
            }
            return false;
        }
    }

    public class JPoint3D
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public JPoint3D(double _x, double _y, double _z)
        {
            this.x = _x;
            this.y = _y;
            this.z = _z;
        }
    }

    [Serializable]
    public class JDoor : JBaseItem
    {
        public List<long> roomIds { get; set; }
        public JDoorGeometry geometry { get; set; }
        public float DoorHeight
        {
            get
            {
                return (float)(geometry != null ? geometry.height : 0);
            }
        }

        public Point3D GetCenterPoint()
        {
            Point3D doorPos = geometry.positions[0].position;
            if (geometry.positions.Count == 2)
            {
                doorPos = (geometry.positions[0].position + geometry.positions[1].position) / 2;
            }
            return doorPos;
        }

        public Point3D GetEndPoint()
        {
            var point = GetCenterPoint();
            var dir = geometry.positions[0].direction;
            dir = Point3D.CrossProduct(dir, Point3D.Zaxis).GetNormalize();

            return point + dir * this.geometry.width / 2;
        }

        public List<JRoom> GetRooms(JModel model)
        {
            if (model.rooms == null)
                return null;
            return model.rooms.Where(room =>
                room.geometry.boundary.Any(segment =>
                geometry.positions.Any(pos => pos.segment_id == segment.id))).ToList();

        }
        
        public Point3D GetStartPoint()
        {
            var point = GetCenterPoint();
            var dir = geometry.positions[0].direction;
             dir = Point3D.CrossProduct(dir, Point3D.Zaxis).GetNormalize();

            return point - dir * this.geometry.width/2;
        }

        public virtual SurfaceMember GetSurfaceGeo ()
        {
            if (geometry == null)
                return null;

            if (geometry.positions.Count == 0)
                return null;

            var width = geometry.width < 0 ? Config.DefaultWidth : geometry.width;
            var height = geometry.height < 0 ? Config.DefaultHeight : geometry.height;

            Point3D doorPos = geometry.positions[0].position;
            if (geometry.positions.Count== 2)
            {
                doorPos = (geometry.positions[0].position + geometry.positions[1].position) / 2;                
            }

            var segment = geometry.positions[0];
            var dir = Point3D.CrossProduct(segment.direction, Point3D.Zaxis).GetNormalize();
            
            var pos1 = doorPos + dir * width / 2;
            var pos2 = pos1 + Point3D.Zaxis * height;
            var pos3 = pos2 - dir * width;
            var pos4 = doorPos - dir * width / 2;

            SurfaceMember surfaceN = new SurfaceMember(new List<Point3D> { pos1, pos2, pos3, pos4, pos1 });

            return surfaceN;
        }

        public IEnumerable<Point3D>  GetBoundaryPolyline()
        {
            if (geometry != null && geometry.positions.Count > 0)
            {
                
                if(geometry.positions.Count <2)
                {
                    //yield return segment.position - dir * width / 2 - segment.direction* Config.DefaultThickness/2;                   
                    //yield return segment.position + dir * width / 2 - segment.direction * Config.DefaultThickness / 2;
                    var segment = geometry.positions[0];
                    var width = segment.width;
                    if (width == 0)
                    {
                        width = geometry.width < 0 ? Config.DefaultWidth : geometry.width;
                    }

                    var dir = Point3D.CrossProduct(segment.direction, Point3D.Zaxis).GetNormalize();

                    yield return segment.position + dir * width / 2 ;
                    yield return segment.position - dir * width / 2;
                }
                else
                {
                    var segment = geometry.positions[0];
                    var width = segment.width;
                    if (width == 0)
                    {
                        width = geometry.width < 0 ? Config.DefaultWidth : geometry.width;
                    }
                    var dir = Point3D.CrossProduct(segment.direction, Point3D.Zaxis).GetNormalize();

                    yield return segment.position + dir * width / 2;
                    yield return segment.position - dir * width / 2;

                    segment = geometry.positions[1];
                     width = segment.width;
                    if (width == 0)
                    {
                        width = geometry.width < 0 ? Config.DefaultWidth : geometry.width;
                    }
                    dir = Point3D.CrossProduct(segment.direction, Point3D.Zaxis).GetNormalize();
                    yield return segment.position + dir * width / 2;
                    yield return segment.position - dir * width / 2;
                }
            }
        }
    }

    [Serializable]
    public class JWindow : JDoor
    {

    }

    [Serializable]
    public class JDoorBaseGeometry
    {
        public JBoundingBox bouding_box { get; set; }
        public Point3D position { get; set; }
        public Point3D direction { get; set; }
        public double width { get; set; }
        public double height { get; set; }
    }
    [Serializable]
    public class JDoorGeometry
    {
        
        public List<Pos> positions { get; set; }
        public double width { get; set; }
        public double height { get; set; }

        
        internal bool HasSegment(List<JLineSegment> boundary)
        {
            if (positions == null)
                return false;
            return positions.Any(x=> boundary.Any(y=>y.id == x.segment_id));
        }
    }
    [Serializable]
    public class Pos
    {
        public int segment_id { get; set; }
        public Point3D position { get; set; }
        public Point3D direction { get; set; }

        public double width { get; set; }
    }
  
    public class Box3
    {
        public Point3D min;
        public Point3D center;
        public Point3D max;
        public Box3() { }
        public Box3(Point3D min, Point3D center, Point3D max)
        { this.min = min; this.center = center; this.max = max; }
    }


    public class DoorSurfaces
    {
        public List<SurfaceMember> doorSurfaceList;
        public string id;
        public string idWall;
        public Door item;
        public DoorSurfaces()
        {
            this.doorSurfaceList = new List<SurfaceMember>();
        }
        public DoorSurfaces(List<SurfaceMember> doorSurfaceList, string id, string idWall, Door item)
        {
            this.doorSurfaceList = doorSurfaceList;
            this.id = id;
            this.idWall = idWall;
            this.item = item;
        }
    }

    public class Door
    {
        public long id;
        public JLineSegment segment;
        public JRoom room;
        public JDoor door;
        public Point3D position;
        public Point3D dir;
        public Door(int id, JLineSegment segment, JRoom room, JDoor door, Point3D position, Point3D dir)
        {
            this.id = id;
            this.segment = segment;
            this.room = room;
            this.door = door;
            this.position = position;
            this.dir = dir;
        }
        public Door()
        { }
    }


    public class JWall
    {
        public int id;
        public string type;
        public string name;
        public int level_id;
        public JWallGeometry geometry;
    }

    public class JWallGeometry
    {
        public Point3D direction;
        public Point3D[] location_curve;
        public Point3D[] external_faces;
        public Point3D[] internal_faces;
    }

    public class SurfaceMember
    {
        public string Id { get; set; }
        public List<Point3D> points = new List<Point3D>();
        public SurfaceMember(List<Point3D> points)
        { this.points = points; }

        public SurfaceMember()
        { }
    }

    public class ExteriorDoor: JWindow
    {
        List<SurfaceMember> surface = new List<SurfaceMember>();
        //private JDoor door { get; }
        public ExteriorDoor(JDoor door)
        {
            //this.door = door;
            this.roomIds = door.roomIds;
            this.geometry = door.geometry;
            //public JDoorGeometry geometry { get; set; }
        }
    }

    public enum ObjName
    {
        gml,
        bldg,
        id,
        xsi,
        xAL,
        xlink,
        dem,
        CityModel,
        boundedBy,
        Envelope,
        lowerCorner,
        upperCorner,
        cityObjectMember,
        Building,
        interiorRoom,
        Room,
        InteriorWallSurface,
        name,
        lod4MultiSurface,
        MultiSurface,
        surfaceMember,
        CompositeSurface,
        Polygon,
        exterior,
        interior,
        LinearRing,
        posList,
        opening,
        Door,
        OrientableSurface,
        baseSurface,
        orientation,
        href,
        Window,
        srsDimension,
        srsName,
        schemaLocation,
        Floor,
        Ceiling,
        FloorSurface,
        CeilingSurface,
        ClosureSurface,
        WallSurface,
        lod4Solid,
        Solid,
        IndoorFeatures,
        core,
        navi,
        nil,
        primalSpaceFeatures,
        multiLayeredGraph,
        PrimalSpaceFeatures,
        MultiLayeredGraph,
        spaceLayers,
        spaceLayerMember,
        SpaceLayer,
        nodes,
        edges,
        cellSpaceMember,
        TransitionSpace,
        description,
        cellSpaceGeometry,
        duality,
        partialboundedBy,
        function,
        usage,
        Geometry3D,
        Shell,
        pos,
        CellSpace,
        cellSpaceBoundaryMember,
        CellSpaceBoundary,
        cellSpaceBoundaryGeometry,
        geometry3D,
        stateMember,
        State,
        connects,
        geometry,
        Point,
        transitionMember,
        Transition,
        weight,
        LineString,
        GeneralSpace
    }
}
