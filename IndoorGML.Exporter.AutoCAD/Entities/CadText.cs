using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.AutoCAD.Entities
{
    public class CadText
    {
        public string Text;
        public Point3d Position;
        public CadText(string textString, Point3d position)
        {
            Text = textString;
            Position = position;
        }
    }
}
