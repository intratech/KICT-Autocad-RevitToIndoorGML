using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGML.Exporter.AutoCAD.Entities
{
    [Serializable]
    public class LevelInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Elevation { get; set; }

        public LevelInfo() { }
        public LevelInfo(int id, string name, double elevation)
        {
            this.ID = id;
            this.Name = name;
            this.Elevation = elevation;
        }
    }
}
