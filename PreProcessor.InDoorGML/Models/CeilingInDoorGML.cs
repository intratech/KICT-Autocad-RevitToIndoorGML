using CityGML.Core.DTO.Json;
using Point3DIntra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreProcessor.InDoorGML.Models
{
    public class CeilingInDoorGML:Ceiling
    {
        private JRoomInDoorGML room;
        public CeilingInDoorGML(Ceiling ceiling, JRoomInDoorGML room) : base(ceiling)
        {
            this.room = room;
        }

        public Point3D GetCenter()
        {
            if (surface == null || surface.points == null || surface.points.Count == 0)
                return null;

            Point3D min = new Point3D(double.MaxValue,double.MaxValue,double.MaxValue);
            Point3D max = new Point3D(double.MinValue, double.MinValue, double.MinValue);
            foreach (var p in surface.points)
            {
                if (min.x > p.x)
                    min.x = p.x;
                if (min.y > p.y)
                    min.y = p.y;
                if (min.z > p.z)
                    min.z = p.z;

                if (max.x < p.x)
                    max.x = p.x;
                if (max.y < p.y)
                    max.y = p.y;
                if (max.z < p.z)
                    max.z = p.z;
            }

            return new Point3D(max.x/2+min.x/2, max.y/2+min.y/2, max.z/2+min.z/2);
        }

        public List<JRoomInDoorGML> GetRooms()
        {
            return new List<JRoomInDoorGML> { room };
        }
    }
}
