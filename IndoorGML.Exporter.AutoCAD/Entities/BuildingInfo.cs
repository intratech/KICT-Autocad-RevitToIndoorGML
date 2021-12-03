using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGML.Exporter.AutoCAD.Entities
{
    public class BuildingInfo
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public LevelInfo Level { get; set; }
    }
}
