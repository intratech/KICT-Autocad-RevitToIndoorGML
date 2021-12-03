using Point3DIntra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGML.Exporter.AutoCAD.Entities
{
    public class PointOnSegment
    {
        public string ID { get; set; }
        public Point3D Point { get; set; }
        public bool IsOnClosureSurface { get; set; }
        public double Distance { get; set; }

        public PointOnSegment(Point3D point, double distance, bool isOnClosureSurface)
        {
            this.ID = Guid.NewGuid().ToString();
            this.Point = point;
            this.Distance = distance;
            this.IsOnClosureSurface = isOnClosureSurface;
        }
    }
}
