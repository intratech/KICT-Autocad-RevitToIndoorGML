using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Point3DIntra;

namespace CityGMLPublisher
{
    public class BoudingBox
    {
        public Point3D min { get; set; }
        public Point3D max { get; set; }
    }

    public class Geometry
    {
        public BoudingBox bouding_box { get; set; }
        public Point3D position { get; set; }
        public List<Point3D> boundary { get; set; }
        public Point3D direction { get; set; }
        public double? width { get; set; }
        public double? height { get; set; }
    }

    public class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class RevitData
    {
        public int id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string level { get; set; }
        public Geometry geometry { get; set; }
        public List<Property> properties { get; set; }
        public int? room { get; set; }
        public int? to_room { get; set; }
    }

    public class RevitDataList
    {
        public string model_name { get; set; }
        public BoudingBox bounding_box { get; set; }
        public List<RevitData> items { get; set; }
    }


    public class Box3
    {
        public Point3D min;
        public Point3D center;
        public Point3D max;
        public Box3() {}
        public Box3(Point3D min, Point3D center, Point3D max)
        { this.min = min; this.center = center; this.max = max; }
    }

    public class DoorSurfaces
    {
        public List<Surface4> doorSurfaceList;
        public string id;
        public string idWall;
        //public RevitData Room;
        public Door item;
        public DoorSurfaces()
        {
            this.doorSurfaceList = new List<Surface4>();
        }
        public DoorSurfaces(List<Surface4> doorSurfaceList, string id, string idWall, RevitData Room, Door item)
        {
            this.doorSurfaceList = doorSurfaceList;
            this.id = id;
            this.idWall = idWall;
            this.item = item;
        }
    }

    public class Door
    {
        public Point3D position;
        public int id;
        public RevitData Room;
        public double? width;
        public double? heigh;
        public Point3D direction;
        public string type;
        public Door(Point3D position, int id, RevitData Room, double? width, double? heigh, Point3D direction, string type)
        {
            this.position = position;
            this.id = id;
            this.Room = Room;
            this.width = width;
            this.heigh = heigh;
            this.direction = direction;
            this.type = type;
        }
    }

    public class Surface4
    {
        public Point3D point1;
        public Point3D point2;
        public Point3D point3;
        public Point3D point4;
        public Surface4(Point3D point1, Point3D point2, Point3D point3, Point3D point4)
        { this.point1 = point1; this.point2 = point2; this.point3 = point3; this.point4 = point4; }
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
        schemaLocation
    }
}
