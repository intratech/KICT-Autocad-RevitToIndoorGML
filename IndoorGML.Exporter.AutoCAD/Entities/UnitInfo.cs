using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGML.Exporter.AutoCAD.Entities
{
    public class UnitInfo
    {
        public Unit Id { get; set; }
        public string Name { get; set; }

        public UnitInfo(Unit id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
