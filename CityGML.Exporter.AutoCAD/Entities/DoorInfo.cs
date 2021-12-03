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
    public class DoorInfoXdata
    {
        public long ID { get; set; }
        public long Handle { get; set; }
        public List<long> RoomIds { get; set; }
        public string Name { get; set; }
        public bool VirtualDoor { get; set; }
        public List<DoorPositionXData> Positions { get; set; }
        //public Point3DXdata Location { get; set; }

        public DoorInfoXdata()
        {

        }

        public DoorInfoXdata(DoorInfo doorInfo)
        {
            this.ID = doorInfo.ID;
            //this.Handles = doorInfo.Handles;
            this.RoomIds = doorInfo.RoomIds;
            this.Name = doorInfo.Name;
            this.VirtualDoor = doorInfo.VirtualDoor;
            this.Positions = doorInfo.Positions.Select(p => new DoorPositionXData(p)).ToList();
            //this.Location = new Point3DXdata(doorInfo.Location);
        }

        public DoorInfo Convert()
        {
            return new DoorInfo()
            {
                ID = this.ID,
                Name = this.Name,
                RoomIds = this.RoomIds,
                Positions = this.Positions.Select(p => new DoorPosition() { SegmentID = p.SegmentID, Position = p.Position.Convert(), Direction = p.Direction.Convert() }).ToList(),
                //Location = new Point3D(this.Location.x, this.Location.y, this.Location.z)
            };
        }
    }
    public class DoorPositionXData
    {
        public int SegmentID { get; set; }
        public Point3DXdata Position { get; set; }
        public Point3DXdata Direction { get; set; }

        public DoorPositionXData()
        {

        }
        public DoorPositionXData(DoorPosition doorPosition)
        {
            this.SegmentID = doorPosition.SegmentID;
            this.Position = new Point3DXdata(doorPosition.Position);
            this.Direction = new Point3DXdata(doorPosition.Direction);
        }
    }
    public class DoorInfo
    {
        public long ID { get; set; }
        public long Handle { get; set; }
        public List<long> RoomIds { get; set; }
        public string Name { get; set; }
        public bool VirtualDoor { get; set; }
        public List<DoorPosition> Positions { get; set; }
        public Arc Arc { get; set; }
        public Line Line { get; set; }
        

        public DoorInfo()
        {
            this.RoomIds = new List<long>();
            this.Positions = new List<DoorPosition>();
        }
    }
    public class DoorPosition
    {
        public int SegmentID { get; set; }
        public Point3D Position { get; set; }
        public Point3D Direction { get; set; }
    }
}
