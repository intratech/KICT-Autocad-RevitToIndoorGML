using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGML.Exporter.AutoCAD.Entities
{
    class VirtualDoorInfo
    {
        public int ID { get; set; }
        public List<long> Handles { get; set; }
        public List<long> RoomIds { get; set; }
        public string Name { get; set; }
        public bool VirtualDoor { get; set; }
        public List<DoorPosition> Positions { get; set; }
        

        public VirtualDoorInfo()
        {
            //this.RoomIds = new List<long>();
            //this.Curves = new List<Curve>();
            //this.Positions = new List<DoorPosition>();
            //this.Handles = new List<long>();
        }
    }
}
