using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Point3DIntra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGML.Exporter.AutoCAD.Entities
{
    [Serializable]
    public class RoomInfoXData {
        public int Index { get; set; }
        public long ID { get; set; }
        public RoomInfoLevelXData Level { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        //public List<BoundarySegmentXData> Boundary { get; set; }
        //public BoundingBoxXData BoundingBox { get; set; }

        public RoomInfoXData()
        {

        }
        public RoomInfoXData(RoomInfo room)
        {
            this.Index = room.Index;
            this.ID = room.ID;
            this.Level = new RoomInfoLevelXData(room.Level.ID, room.Level.Name, room.Level.Elevation);
            this.Name = room.Name;
            this.Number = room.Number;
            //this.Boundary = room.Boundary.Select(b => new BoundarySegmentXData(b)).ToList();
            //this.BoundingBox = new BoundingBoxXData(room.BoundingBox);
        }
        public RoomInfo Convert()
        {
            return new RoomInfo()
            {
                Index = this.Index,
                ID = this.ID,
                Level = new LevelInfo(this.Level.ID, this.Level.Name, this.Level.Elevation),
                Name = this.Name,
                Number = this.Number,
                //Boundary = this.Boundary.Select(b => new BoundarySegment()
                //{
                //    Id = b.Id,
                //    StartPoint = b.StartPoint.Convert(),
                //    EndPoint = b.EndPoint.Convert()
                //}).ToList(),
                //BoundingBox = this.BoundingBox.Convert()
            };
        }
    }
    public class RoomInfoLevelXData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Elevation { get; set; }

        public RoomInfoLevelXData(int id, string name, double elevation)
        {
            this.ID = id;
            this.Name = name;
            this.Elevation = elevation;
        }
    }
    [Serializable]
    public class BoundarySegmentXData {
        public int Id { get; set; }
        public Point3DXdata StartPoint { get; set; }
        public Point3DXdata EndPoint { get; set; }

        public BoundarySegmentXData(BoundarySegment boundary)
        {
            this.Id = boundary.Id;
            this.StartPoint = new Point3DXdata(boundary.StartPoint);
            this.EndPoint = new Point3DXdata(boundary.EndPoint);
        }
    }
    [Serializable]
    public class Point3DXdata {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public Point3DXdata(Point3D point)
        {
            this.x = point.x;
            this.y = point.y;
            this.z = point.z;
        }
        public Point3DXdata(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Point3DXdata()
        {

        }

        public Point3D Convert()
        {
            return new Point3D(this.x, this.y, this.z);
        }
    }
    [Serializable]
    public class BoundingBoxXData
    {
        public Point3DXdata min { get; set; }
        public Point3DXdata max { get; set; }

        public BoundingBoxXData()
        {

        }
        public BoundingBoxXData(Point3DXdata min, Point3DXdata max)
        {
            this.min = min;
            this.max = max;
        }
        public BoundingBoxXData(BoundingBox box)
        {
            this.min = new Point3DXdata(box.min);
            this.max = new Point3DXdata(box.max);
        }

        public BoundingBox Convert()
        {
            return new BoundingBox()
            {
                min = this.min.Convert(),
                max = this.max.Convert()
            };
        }
    }


    [Serializable]
    public class RoomInfo
    {
        public int Index { get; set; }
        public long ID { get; set; }
        public LevelInfo Level { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public List<BoundarySegment> Boundary { get; set; }
        public BoundingBox BoundingBox { get; set; }
        public bool IsSet { get; set; }
        public bool HasClosureSurface { get { return this.Boundary != null && this.Boundary.Count > 0 && this.Boundary.Any(b => b.IsClosureSurface); } }

        public RoomInfo()
        {

        }
        public RoomInfo(int id, LevelInfo level, string name, string number)
        {
            this.ID = id;
            this.Level = level;
            this.Name = name;
            this.Number = number;
        }
    }

    
    public class BoundarySegment {
        public int Id { get; set; }
        public LineSegment3d Segment { get; set; }
        public Point3D StartPoint { get; set; }
        public Point3D EndPoint { get; set; }
        public bool IsClosureSurface { get; set; }

        public BoundarySegment()
        {

        }
        public BoundarySegment(int _id, LineSegment3d seg)
        {
            this.Id = _id;
            this.Segment = seg;
            this.StartPoint = new Point3D(seg.StartPoint.X, seg.StartPoint.Y, seg.StartPoint.Z);
            this.EndPoint = new Point3D(seg.EndPoint.X, seg.EndPoint.Y, seg.EndPoint.Z);
        }
    }

}
