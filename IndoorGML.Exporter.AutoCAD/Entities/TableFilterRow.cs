using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGML.Exporter.AutoCAD.Entites
{
    public class TableFilterRow
    {
        public string LayerName { get; set; }

        public TableFilterRow(string name)
        {
            this.LayerName = name;
        }
    }
}
