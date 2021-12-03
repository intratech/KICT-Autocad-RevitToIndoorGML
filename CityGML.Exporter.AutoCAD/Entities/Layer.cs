using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.GraphicsInterface;
using CityGML.Exporter.AutoCAD.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityGML.Exporter.AutoCAD.Entities
{
    public class LayerInfo
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public LineWeight LineWeight { get; set; }
        public Linetype LineType { get; set; }
        public bool IsOff { get; set; }

        public LayerInfo(ObjectId id, string name, bool isOff = false)
        {
            this.Id = id;
            this.Name = name;
            this.IsOff = IsOff;
        }
        public LayerInfo(string name, Colors corlor, Linetype lineType, LineWeight lineWeight)
        {
            this.Name = name;
            this.Color = Color.FromColorIndex(ColorMethod.ByAci, (short)corlor);
            this.LineType = lineType;
            this.LineWeight = lineWeight;
        }
    }
}
